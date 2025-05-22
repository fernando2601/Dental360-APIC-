import { useState, useEffect } from "react";
import { Card, CardContent } from "@/components/ui/card";
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
import { Loader2 } from "lucide-react";
import { format } from 'date-fns';
import { Calendar } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

// Definição da localização portuguesa para o calendário
const ptBrLocale = {
  code: 'pt-br',
  week: {
    dow: 0, // Domingo como primeiro dia da semana
    doy: 4, // A semana que contém Jan 4 é a primeira semana do ano
  },
  buttonText: {
    prev: 'Anterior',
    next: 'Próximo',
    today: 'Hoje',
    month: 'Mês',
    week: 'Semana',
    day: 'Dia',
    list: 'Lista',
  },
  weekText: 'Sm',
  allDayText: 'Todo o dia',
  moreLinkText: 'mais',
  noEventsText: 'Sem eventos para mostrar'
};

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
  const [calendarApi, setCalendarApi] = useState<Calendar | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
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
      form.reset({
        clientId: 1,
        staffId: 1,
        serviceId: 1,
        startTime: '',
        endTime: '',
        status: 'scheduled',
        notes: '',
      });
    },
    onError: () => {
      toast({
        title: "Erro",
        description: "Falha ao criar agendamento. Por favor, tente novamente.",
        variant: "destructive",
      });
    },
  });

  // Initialize calendar after data is loaded
  useEffect(() => {
    if (
      !isLoadingAppointments &&
      !isLoadingClients &&
      !isLoadingStaff &&
      !isLoadingServices &&
      !calendarApi
    ) {
      const calendarEl = document.getElementById('calendar');
      if (!calendarEl) return;

      const calendar = new Calendar(calendarEl, {
        plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
        initialView: 'timeGridWeek',
        locale: ptBrLocale,
        headerToolbar: false, // Desabilita a barra de ferramentas padrão
        slotMinTime: '06:00:00',
        slotMaxTime: '22:00:00',
        slotDuration: '00:30:00', // Intervalos de 30 minutos
        slotLabelInterval: '01:00:00', // Mostra label a cada hora
        allDaySlot: false,
        height: 700,
        editable: true,
        selectable: true,
        selectMirror: true,
        dayMaxEvents: true,
        nowIndicator: true,
        scrollTime: '08:00:00', // Começa visualização às 8h
        slotLabelFormat: {
          hour: '2-digit',
          minute: '2-digit',
          hour12: false,
          meridiem: false
        },
        dayHeaderFormat: { 
          weekday: 'long', 
          day: 'numeric',
          month: 'short'
        },
        // Personalizações de estilo
        dayCellClassNames: 'rounded-md bg-slate-50/50',
        slotLabelClassNames: 'text-sm font-medium text-slate-500',
        events: appointments.map((appointment: any) => {
          const client = clients.find((c: any) => c.id === appointment.clientId);
          const service = services.find((s: any) => s.id === appointment.serviceId);
          const staffMember = staff.find((s: any) => s.id === appointment.staffId);
          
          return {
            id: appointment.id.toString(),
            title: client ? client.fullName : 'Cliente',
            start: appointment.startTime,
            end: appointment.endTime,
            backgroundColor: getStatusColor(appointment.status),
            borderColor: getStatusColor(appointment.status),
            textColor: '#ffffff',
            classNames: 'rounded-md shadow-sm',
            extendedProps: {
              clientId: appointment.clientId,
              staffId: appointment.staffId,
              serviceId: appointment.serviceId,
              status: appointment.status,
              notes: appointment.notes,
              clientName: client ? client.fullName : 'Cliente',
              serviceName: service ? service.name : 'Serviço',
              staffName: staffMember && staffMember.user ? staffMember.user.fullName : `Profissional #${appointment.staffId}`,
            },
          };
        }),
        eventContent: function(arg) {
          const timeText = arg.timeText;
          const title = arg.event.title;
          const serviceName = arg.event.extendedProps.serviceName;
          
          return { 
            html: `
              <div class="p-1">
                <div class="text-xs font-semibold">${timeText}</div>
                <div class="text-sm font-medium">${title}</div>
                ${serviceName ? `<div class="text-xs opacity-90">${serviceName}</div>` : ''}
              </div>
            `
          };
        },
        select: (info) => {
          // Handle date selection - open appointment form
          setSelectedDate(info.start);
          const endTime = new Date(info.start);
          endTime.setMinutes(endTime.getMinutes() + 60); // Default 1-hour appointment
          
          form.setValue('startTime', formatDateTimeForInput(info.start));
          form.setValue('endTime', formatDateTimeForInput(endTime));
          
          setIsDialogOpen(true);
        },
        eventClick: (info) => {
          // Handle event click - open appointment details
          console.log('Event clicked:', info.event);
          // Implement appointment details view
        },
      });

      calendar.render();
      setCalendarApi(calendar);

      return () => {
        calendar.destroy();
      };
    }
  }, [appointments, isLoadingAppointments, isLoadingClients, isLoadingStaff, isLoadingServices, calendarApi, form]);

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
  function navigatePrev() {
    if (calendarApi) {
      calendarApi.prev();
    }
  }

  function navigateNext() {
    if (calendarApi) {
      calendarApi.next();
    }
  }

  function navigateToday() {
    if (calendarApi) {
      calendarApi.today();
    }
  }

  function changeView(viewName: string) {
    if (calendarApi) {
      calendarApi.changeView(viewName);
    }
  }

  const isLoading = isLoadingAppointments || isLoadingClients || isLoadingStaff || isLoadingServices;

  return (
    <Card className="shadow-lg">
      <CardContent className="p-0">
        {isLoading ? (
          <div className="flex items-center justify-center h-[600px]">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        ) : (
          <div>
            <div className="flex justify-between items-center p-4 border-b">
              <div className="flex items-center gap-2">
                <Button 
                  variant="ghost" 
                  size="sm" 
                  className="text-sm font-medium"
                  onClick={navigateToday}
                >
                  Hoje
                </Button>
                <Button 
                  variant="ghost" 
                  size="sm" 
                  className="px-1 text-lg"
                  onClick={navigatePrev}
                >
                  &lt;
                </Button>
                <span className="text-sm font-medium">
                  27 de abr. - 3 de mai. de 2025
                </span>
                <Button 
                  variant="ghost" 
                  size="sm" 
                  className="px-1 text-lg"
                  onClick={navigateNext}
                >
                  &gt;
                </Button>
              </div>
              <div className="flex items-center gap-1">
                <Select defaultValue="week">
                  <SelectTrigger className="w-[100px]">
                    <SelectValue placeholder="Semana" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="month" onClick={() => changeView('dayGridMonth')}>Mês</SelectItem>
                    <SelectItem value="week" onClick={() => changeView('timeGridWeek')}>Semana</SelectItem>
                    <SelectItem value="day" onClick={() => changeView('timeGridDay')}>Dia</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div id="calendar" className="h-[700px] p-2 calendar-custom" />
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
