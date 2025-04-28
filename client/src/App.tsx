import { Switch, Route, Redirect, useLocation } from "wouter";
import { queryClient } from "./lib/queryClient";
import { QueryClientProvider } from "@tanstack/react-query";
import { TooltipProvider } from "@/components/ui/tooltip";
import { ThemeProvider } from "./lib/theme-provider";
import { AuthProvider, useAuth } from "@/hooks/use-auth";
import { useEffect } from "react";
import { Loader2 } from "lucide-react";
import MainLayout from "./layouts/main-layout";
import Dashboard from "@/pages/dashboard";
import Appointments from "@/pages/appointments";
import Clients from "@/pages/clients";
import Inventory from "@/pages/inventory";
import Finances from "@/pages/finances";
import Services from "@/pages/services";
import Staff from "@/pages/staff";
import Settings from "@/pages/settings";
import WhatsApp from "@/pages/whatsapp";
import Packages from "@/pages/packages";
import Subscriptions from "@/pages/subscriptions";
import ClinicInfo from "@/pages/clinic-info";
import BeforeAfter from "@/pages/before-after";
import NotFound from "@/pages/not-found";
import Login from "@/pages/login";
import ResetPassword from "@/pages/reset-password";

function AuthenticatedRoutes() {
  return (
    <MainLayout>
      <Switch>
        <Route path="/" component={Dashboard} />
        <Route path="/appointments" component={Appointments} />
        <Route path="/clients" component={Clients} />
        <Route path="/inventory" component={Inventory} />
        <Route path="/finances" component={Finances} />
        <Route path="/services" component={Services} />
        <Route path="/staff" component={Staff} />
        <Route path="/whatsapp" component={WhatsApp} />
        <Route path="/packages" component={Packages} />
        <Route path="/subscriptions" component={Subscriptions} />
        <Route path="/clinic-info" component={ClinicInfo} />
        <Route path="/before-after" component={BeforeAfter} />
        <Route path="/settings" component={Settings} />
        <Route component={NotFound} />
      </Switch>
    </MainLayout>
  );
}

function Router() {
  const { user, isLoading } = useAuth();
  const [location, setLocation] = useLocation();

  useEffect(() => {
    // Se o usuário não estiver autenticado e não estiver na página de login, redirecionar
    if (!isLoading && !user && 
        location !== '/login' && 
        location !== '/reset-password' && 
        !location.startsWith('/reset-password?token=')) {
      setLocation('/login');
    }
  }, [user, isLoading, location, setLocation]);

  // Durante o carregamento, mostrar indicador de loading
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  // Se não estiver autenticado, mostrar apenas as rotas públicas
  if (!user) {
    return (
      <Switch>
        <Route path="/login" component={Login} />
        <Route path="/reset-password" component={ResetPassword} />
        <Route path="*">
          <Redirect to="/login" />
        </Route>
      </Switch>
    );
  }

  // Se estiver autenticado, mostrar todas as rotas protegidas
  return <AuthenticatedRoutes />;
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <TooltipProvider>
        <ThemeProvider>
          <AuthProvider>
            <Router />
          </AuthProvider>
        </ThemeProvider>
      </TooltipProvider>
    </QueryClientProvider>
  );
}

export default App;