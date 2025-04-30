import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import Calendar from "./calendar";

type ViewType = "agenda" | "visao-geral" | "relatorio";

export default function Agenda() {
  const [activeView, setActiveView] = useState<ViewType>("agenda");
  
  // Fetch data
  const { data: appointments = [] } = useQuery({
    queryKey: ['/api/appointments'],
  });

  // Dados para o relatório de agendamentos
  const todaysAppointments = appointments.filter((a: any) => {
    const appointmentDate = new Date(a.startTime);
    const today = new Date();
    return appointmentDate.getDate() === today.getDate() && 
           appointmentDate.getMonth() === today.getMonth() && 
           appointmentDate.getFullYear() === today.getFullYear();
  });
  
  const pendingAppointments = appointments.filter((a: any) => a.status === 'scheduled');
  
  const completedThisMonth = appointments.filter((a: any) => {
    const appointmentDate = new Date(a.startTime);
    const today = new Date();
    return a.status === 'completed' && 
           appointmentDate.getMonth() === today.getMonth() && 
           appointmentDate.getFullYear() === today.getFullYear();
  });

  // Render appropriate view based on active selection
  const renderView = () => {
    switch (activeView) {
      case "agenda":
        return <Calendar />;
      case "visao-geral":
        return (
          <div className="p-4">
            <h2 className="text-xl font-semibold mb-4">Visão Geral</h2>
            <p className="text-muted-foreground">
              Esta seção fornece uma visão geral do sistema de agendamento.
            </p>
          </div>
        );
      case "relatorio":
        return (
          <div className="p-4">
            <h2 className="text-xl font-semibold mb-4">Relatório de Agendamentos</h2>
            <div className="space-y-4">
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div className="bg-blue-50 p-4 rounded-lg">
                  <h3 className="text-blue-800 font-medium">Agendamentos Hoje</h3>
                  <p className="text-2xl font-bold">{todaysAppointments.length}</p>
                </div>
                <div className="bg-yellow-50 p-4 rounded-lg">
                  <h3 className="text-yellow-800 font-medium">Pendentes</h3>
                  <p className="text-2xl font-bold">{pendingAppointments.length}</p>
                </div>
                <div className="bg-green-50 p-4 rounded-lg">
                  <h3 className="text-green-800 font-medium">Concluídos (Mês)</h3>
                  <p className="text-2xl font-bold">{completedThisMonth.length}</p>
                </div>
              </div>
              <p className="text-sm text-muted-foreground">
                Dados atualizados em tempo real conforme os agendamentos são gerenciados no sistema.
              </p>
            </div>
          </div>
        );
      default:
        return <Calendar />;
    }
  };

  return (
    <div className="flex h-full">
      {/* Sidebar */}
      <div className="w-48 border-r bg-muted/30 p-4">
        <nav className="space-y-1">
          <button
            className={`w-full text-left px-3 py-2 text-sm rounded-md ${
              activeView === "agenda" ? "bg-primary text-white" : "hover:bg-muted"
            }`}
            onClick={() => setActiveView("agenda")}
          >
            Agenda
          </button>
          <button
            className={`w-full text-left px-3 py-2 text-sm rounded-md ${
              activeView === "visao-geral" ? "bg-primary text-white" : "hover:bg-muted"
            }`}
            onClick={() => setActiveView("visao-geral")}
          >
            Visão geral
          </button>
          <button
            className={`w-full text-left px-3 py-2 text-sm rounded-md ${
              activeView === "relatorio" ? "bg-primary text-white" : "hover:bg-muted"
            }`}
            onClick={() => setActiveView("relatorio")}
          >
            Relatório de agendamentos
          </button>
        </nav>
      </div>
      
      {/* Main content */}
      <div className="flex-1 overflow-auto p-4">
        {renderView()}
      </div>
    </div>
  );
}