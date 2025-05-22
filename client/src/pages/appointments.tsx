import { useState } from "react";
import { Calendar, BarChart3, FileText, Clock } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import AppointmentCalendar from "@/components/appointment-calendar";

type ViewType = "agenda" | "visao-geral" | "relatorios";

const menuItems = [
  {
    id: "agenda" as ViewType,
    label: "Agenda",
    icon: Calendar,
    description: "Visualizar e gerenciar agendamentos"
  },
  {
    id: "visao-geral" as ViewType,
    label: "Visão geral",
    icon: BarChart3,
    description: "Dashboard com estatísticas"
  },
  {
    id: "relatorios" as ViewType,
    label: "Relatório de agendamentos", 
    icon: FileText,
    description: "Relatórios detalhados"
  }
];

function VisaoGeral() {
  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold">Visão Geral</h2>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center space-x-2">
              <Clock className="h-5 w-5 text-primary" />
              <span className="text-sm font-medium">Agendamentos Hoje</span>
            </div>
            <p className="text-2xl font-bold mt-2">0</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center space-x-2">
              <Calendar className="h-5 w-5 text-primary" />
              <span className="text-sm font-medium">Esta Semana</span>
            </div>
            <p className="text-2xl font-bold mt-2">0</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center space-x-2">
              <BarChart3 className="h-5 w-5 text-primary" />
              <span className="text-sm font-medium">Este Mês</span>
            </div>
            <p className="text-2xl font-bold mt-2">0</p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function RelatorioAgendamentos() {
  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold">Relatório de Agendamentos</h2>
      <Card>
        <CardContent className="p-6">
          <p className="text-muted-foreground">Relatórios detalhados em desenvolvimento...</p>
        </CardContent>
      </Card>
    </div>
  );
}

export default function Appointments() {
  const [currentView, setCurrentView] = useState<ViewType>("agenda");

  const renderContent = () => {
    switch (currentView) {
      case "agenda":
        return <AppointmentCalendar />;
      case "visao-geral":
        return <VisaoGeral />;
      case "relatorios":
        return <RelatorioAgendamentos />;
      default:
        return <AppointmentCalendar />;
    }
  };

  return (
    <div className="flex gap-6">
      {/* Sidebar de navegação */}
      <div className="w-64 shrink-0">
        <Card>
          <CardContent className="p-4">
            <h3 className="font-semibold mb-4">Agenda</h3>
            <nav className="space-y-2">
              {menuItems.map((item) => {
                const Icon = item.icon;
                return (
                  <Button
                    key={item.id}
                    variant={currentView === item.id ? "default" : "ghost"}
                    className={cn(
                      "w-full justify-start gap-2 h-auto p-3",
                      currentView === item.id && "bg-primary text-primary-foreground"
                    )}
                    onClick={() => setCurrentView(item.id)}
                  >
                    <Icon className="h-4 w-4" />
                    <div className="text-left">
                      <div className="font-medium">{item.label}</div>
                    </div>
                  </Button>
                );
              })}
            </nav>
          </CardContent>
        </Card>
      </div>

      {/* Conteúdo principal */}
      <div className="flex-1">
        {renderContent()}
      </div>
    </div>
  );
}