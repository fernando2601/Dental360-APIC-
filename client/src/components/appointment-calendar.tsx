import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { useToast } from "@/hooks/use-toast";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequest } from "@/lib/queryClient";
import { Loader2, ChevronLeft, ChevronRight, Plus } from "lucide-react";
import { format, addDays, startOfWeek, addWeeks, subWeeks } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

// Interface para agendamento
interface Appointment {
  id: number;
  clientId: number;
  staffId: number;
  serviceId: number;
  startTime: string;
  endTime: string;
  status: string;
  notes?: string;
  createdAt: string;
}

// Interface para client, service, staff
interface Client {
  id: number;
  fullName: string;
}

interface Service {
  id: number;
  name: string;
  duration: number;
}

interface Staff {
  id: number;
  user?: {
    fullName: string;
  };
}

// Extend the appointment schema with client validation
const appointmentFormSchema = z.object({
  clientId: z.string().or(z.number()).transform(val => Number(val)),
  staffId: z.string().or(z.number()).transform(val => Number(val)),
  serviceId: z.string().or(z.number()).transform(val => Number(val)),
  startTime: z.string(),
  endTime: z.string(),
  status: z.string(),
  notes: z.string().optional(),
});

type AppointmentFormValues = z.infer<typeof appointmentFormSchema>;

function AppointmentCalendar() {
  const [currentWeek, setCurrentWeek] = useState<Date>(startOfWeek(new Date(), { weekStartsOn: 0 }));
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [selectedHour, setSelectedHour] = useState<number>(8);
  const { toast } = useToast();
  const queryClient = useQueryClient();

  // Fetch data
  const { data: appointments = [], isLoading: isLoadingAppointments } = useQuery({
    queryKey: ['/api/appointments'],
  });

  const { data: clients = [], isLoading: isLoadingClients } = useQuery({
    queryKey: ['/api/clients'],
  });

  const { data: staff = [], isLoading: isLoadingStaff } = useQuery({
    queryKey: ['/api/staff'],
  });

  const { data: services = [], isLoading: isLoadingServices } = useQuery({
    queryKey: ['/api/services'],
  });

  // Gerar dias da semana
  const weekDays = Array.from({ length: 7 }, (_, i) => addDays(currentWeek, i));
  
  // Gerar horários (0-23)
  const hours = Array.from({ length: 24 }, (_, i) => i);

  // Create appointment form
  const form = useForm<AppointmentFormValues>({
    resolver: zodResolver(appointmentFormSchema),
    defaultValues: {
      clientId: 1,
      staffId: 1,
      serviceId: 1,
      startTime: '',
      endTime: '',
      status: 'scheduled',
      notes: '',
    },
  });

  // Create appointment mutation
  const createAppointment = useMutation({
    mutationFn: async (values: AppointmentFormValues) => {
      const response = await apiRequest('POST', '/api/appointments', values);
      return response.json();
    },
    onSuccess: () => {
      toast({
        title: "Sucesso",
        description: "Agendamento criado com sucesso.",
      });
      queryClient.invalidateQueries({ queryKey: ['/api/appointments'] });
      setIsDialogOpen(false);
      form.reset();
    },
    onError: () => {
      toast({
        title: "Erro",
        description: "Falha ao criar agendamento. Por favor, tente novamente.",
        variant: "destructive",
      });
    },
  });

  // Função para lidar com clique na célula de horário
  const handleCellClick = (day: Date, hour: number) => {
    const appointmentDate = new Date(day);
    appointmentDate.setHours(hour, 0, 0, 0);
    const endDate = new Date(appointmentDate);
    endDate.setHours(hour + 1, 0, 0, 0);
    
    setSelectedDate(appointmentDate);
    setSelectedHour(hour);
    form.setValue('startTime', formatDateTimeForInput(appointmentDate));
    form.setValue('endTime', formatDateTimeForInput(endDate));
    setIsDialogOpen(true);
  };

  // Função para verificar se existe agendamento em um horário específico
  const getAppointmentForSlot = (day: Date, hour: number) => {
    return (appointments as Appointment[]).find(appointment => {
      const startTime = new Date(appointment.startTime);
      const appointmentDay = startTime.toDateString();
      const dayString = day.toDateString();
      const appointmentHour = startTime.getHours();
      
      return appointmentDay === dayString && appointmentHour === hour;
    });
  };

  // Format date for datetime-local input
  function formatDateTimeForInput(date: Date) {
    return format(date, "yyyy-MM-dd'T'HH:mm");
  }

  // Get status color
  function getStatusColor(status: string) {
    switch (status) {
      case 'scheduled':
        return '#60a5fa'; // Azul claro vibrante
      case 'in-progress':
        return '#f43f5e'; // Vermelho forte - indicador em andamento
      case 'completed':
        return '#34d399'; // Verde vibrante  
      case 'cancelled':
        return '#f87171'; // Vermelho suave
      case 'no-show':
        return '#fbbf24'; // Amarelo âmbar
      default:
        return '#9CA3AF';
    }
  }

  // Calculate end time based on service duration
  function calculateEndTime(serviceId: string, startTimeStr: string) {
    if (!serviceId || !startTimeStr) return;

    const service = services.find((s: any) => s.id === Number(serviceId));
    if (!service) return;

    const startTime = new Date(startTimeStr);
    const endTime = new Date(startTime);
    endTime.setMinutes(startTime.getMinutes() + service.duration);

    form.setValue('endTime', formatDateTimeForInput(endTime));
  }

  // Handle form submission
  function onSubmit(values: AppointmentFormValues) {
    createAppointment.mutate(values);
  }

  // Navegação do calendário
  const navigatePrev = () => {
    setCurrentWeek(subWeeks(currentWeek, 1));
  };

  const navigateNext = () => {
    setCurrentWeek(addWeeks(currentWeek, 1));
  };

  const navigateToday = () => {
    setCurrentWeek(startOfWeek(new Date(), { weekStartsOn: 0 }));
  };

  const isLoading = isLoadingAppointments || isLoadingClients || isLoadingStaff || isLoadingServices;

  return (
    <Card>
      <CardHeader>
        <div className="flex justify-between items-center">
          <CardTitle className="text-2xl font-bold">Agenda</CardTitle>
          <Button 
            onClick={() => setIsDialogOpen(true)}
          >
            <Plus className="h-4 w-4 mr-2" />
            Novo Agendamento
          </Button>
        </div>
        
        {/* Navegação da semana */}
        <div className="flex items-center justify-between mt-4">
          <div className="flex items-center gap-2">
            <Button 
              variant="outline" 
              size="sm" 
              onClick={navigateToday}
            >
              Hoje
            </Button>
            <Button 
              variant="ghost" 
              size="sm" 
              onClick={navigatePrev}
            >
              <ChevronLeft className="h-4 w-4" />
            </Button>
            <span className="text-sm font-medium text-muted-foreground">
              {format(weekDays[0], 'dd MMM', { locale: ptBR })} - {format(weekDays[6], 'dd MMM yyyy', { locale: ptBR })}
            </span>
            <Button 
              variant="ghost" 
              size="sm" 
              onClick={navigateNext}
            >
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      </CardHeader>

      <CardContent className="p-0">
        {isLoading ? (
          <div className="flex items-center justify-center h-[600px]">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        ) : (
          <div className="custom-calendar">
            {/* Header com dias da semana */}
            <div className="calendar-header">
              <div className="hour-column-header"></div>
              {weekDays.map((day, index) => (
                <div key={index} className="day-header">
                  <div className="day-name">{format(day, 'EEEE', { locale: ptBR })}</div>
                  <div className="day-number">{format(day, 'd')}</div>
                </div>
              ))}
            </div>

            {/* Grid principal */}
            <div className="calendar-grid">
              {/* Coluna de horas */}
              <div className="hours-column">
                {hours.map((hour) => (
                  <div key={hour} className="hour-label">
                    {hour.toString().padStart(2, '0')}:00
                  </div>
                ))}
              </div>

              {/* Grid de dias */}
              <div className="days-grid">
                {weekDays.map((day, dayIndex) => (
                  <div key={dayIndex} className="day-column">
                    {hours.map((hour) => {
                      const appointment = getAppointmentForSlot(day, hour);
                      return (
                        <div
                          key={hour}
                          className={`hour-cell ${appointment ? 'has-appointment' : ''}`}
                          onClick={() => !appointment && handleCellClick(day, hour)}
                        >
                          {appointment && (
                            <div className="appointment-block">
                              <div className="appointment-client">
                                {(clients as Client[]).find(c => c.id === appointment.clientId)?.fullName || 'Cliente'}
                              </div>
                              <div className="appointment-service">
                                {(services as Service[]).find(s => s.id === appointment.serviceId)?.name || 'Serviço'}
                              </div>
                            </div>
                          )}
                        </div>
                      );
                    })}
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogContent className="sm:max-w-[500px]">
            <DialogHeader>
              <DialogTitle>Agendar Nova Consulta</DialogTitle>
              <DialogDescription>
                {selectedDate && `Criando agendamento para ${format(selectedDate, 'MMMM d, yyyy')}`}
              </DialogDescription>
            </DialogHeader>
            
            <Form {...form}>
              <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                <FormField
                  control={form.control}
                  name="clientId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Cliente</FormLabel>
                      <Select
                        value={field.value.toString()}
                        onValueChange={(value) => {
                          field.onChange(value);
                        }}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione o cliente" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {clients.map((client: any) => (
                            <SelectItem key={client.id} value={client.id.toString()}>
                              {client.fullName}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <FormField
                  control={form.control}
                  name="serviceId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Serviço</FormLabel>
                      <Select
                        value={field.value.toString()}
                        onValueChange={(value) => {
                          field.onChange(value);
                          calculateEndTime(value, form.getValues('startTime'));
                        }}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione o serviço" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {services.map((service: any) => (
                            <SelectItem key={service.id} value={service.id.toString()}>
                              {service.name} ({service.duration} min)
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <FormField
                  control={form.control}
                  name="staffId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Profissional</FormLabel>
                      <Select
                        value={field.value.toString()}
                        onValueChange={field.onChange}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione o profissional" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {staff.map((s: any) => (
                            <SelectItem key={s.id} value={s.id.toString()}>
                              {s.user?.fullName || `Profissional #${s.id}`}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="startTime"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Horário de Início</FormLabel>
                        <FormControl>
                          <Input
                            {...field}
                            type="datetime-local"
                            onChange={(e) => {
                              field.onChange(e);
                              calculateEndTime(form.getValues('serviceId'), e.target.value);
                            }}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  
                  <FormField
                    control={form.control}
                    name="endTime"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Horário de Término</FormLabel>
                        <FormControl>
                          <Input {...field} type="datetime-local" />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
                
                <FormField
                  control={form.control}
                  name="status"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Status</FormLabel>
                      <Select
                        value={field.value}
                        onValueChange={field.onChange}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione o status" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="scheduled">Agendado</SelectItem>
                          <SelectItem value="in-progress">Em Andamento</SelectItem>
                          <SelectItem value="completed">Concluído</SelectItem>
                          <SelectItem value="cancelled">Cancelado</SelectItem>
                          <SelectItem value="no-show">Não Compareceu</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <FormField
                  control={form.control}
                  name="notes"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Observações</FormLabel>
                      <FormControl>
                        <Textarea
                          {...field}
                          placeholder="Instruções ou observações especiais"
                          rows={3}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                
                <DialogFooter>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => setIsDialogOpen(false)}
                  >
                    Cancelar
                  </Button>
                  <Button type="submit" disabled={createAppointment.isPending}>
                    {createAppointment.isPending && (
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    )}
                    Agendar Consulta
                  </Button>
                </DialogFooter>
              </form>
            </Form>
          </DialogContent>
        </Dialog>
      </CardContent>
    </Card>
  );
}

export default AppointmentCalendar;
