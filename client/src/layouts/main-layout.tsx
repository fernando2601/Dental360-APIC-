import { useState, useEffect } from "react";
import { Sidebar } from "@/components/ui/sidebar";
import { ChatBot } from "@/components/chat-bot";
import { MobileNav } from "@/components/ui/mobile-nav";
import { useMobile } from "@/hooks/use-mobile";
import { Bell, Settings, Menu } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { Separator } from "@/components/ui/separator";
import { ThemeToggle } from "@/components/theme-toggle";

interface MainLayoutProps {
  children: React.ReactNode;
}

export default function MainLayout({ children }: MainLayoutProps) {
  const [isMobileNavOpen, setIsMobileNavOpen] = useState(false);
  const isMobile = useMobile();

  useEffect(() => {
    if (!isMobile) {
      setIsMobileNavOpen(false);
    }
  }, [isMobile]);

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      {/* Sidebar - hidden on mobile */}
      {!isMobile && <Sidebar />}

      {/* Mobile Sidebar */}
      {isMobile && (
        <Sheet open={isMobileNavOpen} onOpenChange={setIsMobileNavOpen}>
          <SheetTrigger asChild>
            <Button 
              variant="ghost" 
              size="icon" 
              className="absolute top-3 left-3 z-50 md:hidden"
            >
              <Menu className="h-6 w-6" />
              <span className="sr-only">Toggle Menu</span>
            </Button>
          </SheetTrigger>
          <SheetContent side="left" className="p-0">
            <Sidebar />
          </SheetContent>
        </Sheet>
      )}

      {/* Main Content */}
      <div className="flex flex-col flex-1 overflow-hidden">
        {/* Top Navigation */}
        <header className="border-b bg-card z-10">
          <div className="flex h-16 items-center px-4 sm:px-6">
            <div className={isMobile ? "ml-10" : ""}>
              <h1 className="text-xl font-heading font-bold">Clínica DentalSpa</h1>
            </div>

            <div className="ml-auto flex items-center space-x-4">
              <Button variant="ghost" size="icon">
                <Bell className="h-5 w-5" />
                <span className="sr-only">Notificações</span>
              </Button>
              <Button variant="ghost" size="icon">
                <Settings className="h-5 w-5" />
                <span className="sr-only">Configurações</span>
              </Button>
              <Separator orientation="vertical" className="h-8" />
              <ThemeToggle />
            </div>
          </div>
        </header>

        {/* Page Content */}
        <main className="flex-1 overflow-y-auto bg-background p-4 md:p-6">
          {children}
        </main>
      </div>

      {/* ChatBot - floats on all pages */}
      <ChatBot />
    </div>
  );
}
