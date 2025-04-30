import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Plus, CalendarDays, Users, ListFilter } from "lucide-react";
import AppointmentCalendar from "@/components/appointment-calendar";

export default function Agenda() {
  const [selectedStaff, setSelectedStaff] = useState<string>("all");
  const [selectedService, setSelectedService] = useState<string>("all");
  const [selectedStatus, setSelectedStatus] = useState<string>("all");
  
  // Fetch data
  const { data: staff = [] } = useQuery({
    queryKey: ['/api/staff'],
  });

  const { data: services = [] } = useQuery({
    queryKey: ['/api/services'],
  });
  
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

  const handleApplyFilters = () => {
    // Lógica para aplicar filtros - seria implementada para filtrar os agendamentos
    console.log("Filtros aplicados:", { selectedStaff, selectedService, selectedStatus });
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl md:text-3xl font-bold tracking-tight">Agenda</h1>
        <Button>
          <Plus className="mr-2 h-4 w-4" />
          Novo Agendamento
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {/* Sidebar - Mini calendário e Filtros */}
        <div className="md:col-span-1 space-y-6">
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-md flex items-center">
                <CalendarDays className="h-4 w-4 mr-2" />
                Visão geral
              </CardTitle>
            </CardHeader>
            <CardContent>
              {/* Mini Calendário - Simplificado */}
              <div className="text-center border rounded-md p-4 mb-4">
                <div className="flex justify-between items-center mb-4">
                  <button className="text-sm">&lt;</button>
                  <span className="font-medium">Maio 2025</span>
                  <button className="text-sm">&gt;</button>
                </div>
                <div className="grid grid-cols-7 gap-1 mb-2">
                  <div className="text-xs text-muted-foreground">D</div>
                  <div className="text-xs text-muted-foreground">S</div>
                  <div className="text-xs text-muted-foreground">T</div>
                  <div className="text-xs text-muted-foreground">Q</div>
                  <div className="text-xs text-muted-foreground">Q</div>
                  <div className="text-xs text-muted-foreground">S</div>
                  <div className="text-xs text-muted-foreground">S</div>
                </div>
                <div className="grid grid-cols-7 gap-1">
                  {Array.from({ length: 31 }).map((_, i) => (
                    <div 
                      key={i} 
                      className={`text-xs p-1 rounded-full w-6 h-6 flex items-center justify-center mx-auto
                        ${i === 29 ? 'bg-primary text-white' : 'hover:bg-muted cursor-pointer'}`}
                    >
                      {i + 1}
                    </div>
                  ))}
                </div>
              </div>
              
              <h3 className="font-medium mb-2">Relatório de agendamentos</h3>
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <span className="text-sm">Agendados hoje:</span>
                  <span className="font-medium">{todaysAppointments.length}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm">Pendentes:</span>
                  <span className="font-medium">{pendingAppointments.length}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm">Concluídos (mês):</span>
                  <span className="font-medium">{completedThisMonth.length}</span>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-md flex items-center">
                <ListFilter className="h-4 w-4 mr-2" />
                Filtros
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Profissional</label>
                <Select value={selectedStaff} onValueChange={setSelectedStaff}>
                  <SelectTrigger>
                    <SelectValue placeholder="Todos os profissionais" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">Todos os profissionais</SelectItem>
                    {staff.map((s: any) => (
                      <SelectItem key={s.id} value={s.id.toString()}>
                        {s.user?.fullName || `Profissional #${s.id}`}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div className="space-y-2">
                <label className="text-sm font-medium">Serviços</label>
                <Select value={selectedService} onValueChange={setSelectedService}>
                  <SelectTrigger>
                    <SelectValue placeholder="Todos os serviços" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">Todos os serviços</SelectItem>
                    {services.map((service: any) => (
                      <SelectItem key={service.id} value={service.id.toString()}>
                        {service.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              
              <div className="space-y-2">
                <label className="text-sm font-medium">Status</label>
                <Select value={selectedStatus} onValueChange={setSelectedStatus}>
                  <SelectTrigger>
                    <SelectValue placeholder="Todos os status" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">Todos os status</SelectItem>
                    <SelectItem value="scheduled">Agendado</SelectItem>
                    <SelectItem value="completed">Concluído</SelectItem>
                    <SelectItem value="cancelled">Cancelado</SelectItem>
                    <SelectItem value="no-show">Não compareceu</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div className="pt-2">
                <Button variant="outline" className="w-full" onClick={handleApplyFilters}>
                  Aplicar Filtros
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
        
        {/* Calendário Principal */}
        <div className="md:col-span-3">
          <AppointmentCalendar />
        </div>
      </div>
    </div>
  );
}