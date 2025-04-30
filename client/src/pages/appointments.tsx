import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { apiRequest } from "@/lib/queryClient";
import { useToast } from "@/hooks/use-toast";
import { Plus, ChevronLeft, ChevronRight, Loader2 } from "lucide-react";
import { format, addDays } from "date-fns";
import { ptBR } from "date-fns/locale";

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

export default function Appointments() {
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [currentDate, setCurrentDate] = useState(new Date());
  const [viewMode, setViewMode] = useState("week");
  
  const { toast } = useToast();
  const queryClient = useQueryClient();

  // Fetch data
  const { data: clients = [] } = useQuery({
    queryKey: ['/api/clients'],
  });

  const { data: staff = [] } = useQuery({
    queryKey: ['/api/staff'],
  });

  const { data: services = [] } = useQuery({
    queryKey: ['/api/services'],
  });
  
  const { data: appointments = [] } = useQuery({
    queryKey: ['/api/appointments'],
  });

  // Create appointment form
  const form = useForm<AppointmentFormValues>({
    resolver: zodResolver(appointmentFormSchema),
    defaultValues: {
      clientId: '',
      staffId: '',
      serviceId: '',
      startTime: format(new Date(), "yyyy-MM-dd'T'HH:mm"),
      endTime: format(new Date(new Date().getTime() + 60 * 60 * 1000), "yyyy-MM-dd'T'HH:mm"),
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
        title: "Success",
        description: "Appointment created successfully.",
      });
      queryClient.invalidateQueries({ queryKey: ['/api/appointments'] });
      setIsCreateDialogOpen(false);
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

  // Calculate end time based on service duration
  function calculateEndTime(serviceId: string, startTimeStr: string) {
    if (!serviceId || !startTimeStr) return;

    const service = services.find((s: any) => s.id === Number(serviceId));
    if (!service) return;

    const startTime = new Date(startTimeStr);
    const endTime = new Date(startTime);
    endTime.setMinutes(startTime.getMinutes() + service.duration);

    form.setValue('endTime', format(endTime, "yyyy-MM-dd'T'HH:mm"));
  }

  // Handle form submission
  function onSubmit(values: AppointmentFormValues) {
    createAppointment.mutate(values);
  }

  // Generate the week days
  const weekDays = Array(7).fill(0).map((_, index) => {
    const date = addDays(currentDate, index - currentDate.getDay());
    return {
      date,
      day: format(date, 'd'),
      weekday: format(date, 'EEE', { locale: ptBR }),
      isToday: format(date, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd'),
      appointments: appointments.filter((a: any) => {
        const appointmentDate = new Date(a.startTime);
        return format(appointmentDate, 'yyyy-MM-dd') === format(date, 'yyyy-MM-dd');
      })
    };
  });

  // Navigation functions
  const goToPreviousWeek = () => {
    setCurrentDate(prev => addDays(prev, -7));
  };

  const goToNextWeek = () => {
    setCurrentDate(prev => addDays(prev, 7));
  };

  const goToToday = () => {
    setCurrentDate(new Date());
  };

  // Time slots for the day
  const timeSlots = [
    { hour: 8, displayHour: "8:00" },
    { hour: 9, displayHour: "9:00" },
    { hour: 10, displayHour: "10:00" },
    { hour: 11, displayHour: "11:00" },
    { hour: 12, displayHour: "12:00" },
    { hour: 12, displayHour: "12:30" },
    { hour: 13, displayHour: "13:00" },
    { hour: 13, displayHour: "13:30" },
    { hour: 14, displayHour: "14:00" },
    { hour: 14, displayHour: "14:30" },
    { hour: 15, displayHour: "15:00" },
    { hour: 15, displayHour: "15:30" },
    { hour: 16, displayHour: "16:00" },
    { hour: 16, displayHour: "16:30" },
    { hour: 17, displayHour: "17:00" },
    { hour: 17, displayHour: "17:30" },
    { hour: 18, displayHour: "18:00" },
  ];

  // Get appointments for a specific hour and day
  const getAppointmentsForTimeSlot = (date: Date, hour: number) => {
    return appointments.filter((a: any) => {
      const appointmentDate = new Date(a.startTime);
      return format(appointmentDate, 'yyyy-MM-dd') === format(date, 'yyyy-MM-dd') && 
             appointmentDate.getHours() === hour;
    });
  };

  // Render different modes
  const renderCalendarView = () => {
    if (viewMode === "day") {
      return <div>Visualização diária</div>;
    } 
    
    if (viewMode === "month") {
      return <div>Visualização mensal</div>;
    }
    
    // Week view (default)
    return (
      <div className="overflow-auto">
        <div className="min-w-[800px]">
          {/* Days of the week header */}
          <div className="grid grid-cols-7 border-b">
            {weekDays.map((day, index) => (
              <div 
                key={index} 
                className="text-center py-2"
              >
                <div className="text-sm font-medium">{day.day}</div>
                <div className="text-xs text-muted-foreground">{day.weekday}</div>
              </div>
            ))}
          </div>

          {/* Time slots */}
          <div className="relative pl-16">
            {timeSlots.map((slot, slotIndex) => (
              <div key={slotIndex} className="grid grid-cols-7 border-b h-14">
                <div className="absolute -left-16 text-xs text-muted-foreground flex items-center h-14 w-14 justify-end pr-2">
                  {slot.displayHour}
                </div>
                {weekDays.map((day, dayIndex) => {
                  const slotAppointments = getAppointmentsForTimeSlot(day.date, slot.hour);
                  return (
                    <div 
                      key={`${slotIndex}-${dayIndex}`} 
                      className="border-r relative"
                    >
                      {slotAppointments.map((appointment: any, i: number) => (
                        <div 
                          key={i}
                          className="absolute top-0 left-1 right-1 bg-blue-100 text-blue-800 rounded p-1 text-xs overflow-hidden whitespace-nowrap"
                          style={{ 
                            zIndex: 10,
                            height: `${Math.min(slotAppointments.length * 20, 90)}%` 
                          }}
                        >
                          {appointment.title || `Paciente #${appointment.clientId}`}
                        </div>
                      ))}
                    </div>
                  );
                })}
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl md:text-3xl font-bold tracking-tight">Agenda</h1>
        <div className="flex items-center gap-4">
          <Button variant="outline" size="sm" onClick={goToToday}>
            Hoje
          </Button>
          <Button variant="ghost" size="icon" onClick={goToPreviousWeek}>
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <span className="text-sm whitespace-nowrap">
            {format(weekDays[0].date, "d 'de' MMM.", { locale: ptBR })} - {format(weekDays[6].date, "d 'de' MMM. 'de' yyyy", { locale: ptBR })}
          </span>
          <Button variant="ghost" size="icon" onClick={goToNextWeek}>
            <ChevronRight className="h-4 w-4" />
          </Button>
          <Select 
            defaultValue="week"
            onValueChange={(value) => setViewMode(value)}
          >
            <SelectTrigger className="w-[100px]">
              <SelectValue placeholder="Semana" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="day">Dia</SelectItem>
              <SelectItem value="week">Semana</SelectItem>
              <SelectItem value="month">Mês</SelectItem>
            </SelectContent>
          </Select>
          <Button onClick={() => setIsCreateDialogOpen(true)}>
            <Plus className="mr-2 h-4 w-4" />
            Novo Agendamento
          </Button>
        </div>
      </div>

      <Card>
        <CardContent className="p-4 pt-6">
          {renderCalendarView()}
        </CardContent>
      </Card>

      {/* Create Appointment Dialog */}
      <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Agendar Nova Consulta</DialogTitle>
            <DialogDescription>
              Crie um novo agendamento para um cliente.
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
                        // Update end time based on service duration
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
                      onValueChange={(value) => {
                        field.onChange(value);
                      }}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione o profissional" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {staff.map((staffMember: any) => (
                          <SelectItem key={staffMember.id} value={staffMember.id.toString()}>
                            {staffMember.name || `Profissional #${staffMember.id}`}
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
                      <FormLabel>Data e Hora Inicial</FormLabel>
                      <FormControl>
                        <Input type="datetime-local" {...field} onChange={(e) => {
                          field.onChange(e);
                          // Recalculate end time when start time changes
                          calculateEndTime(form.getValues('serviceId'), e.target.value);
                        }} />
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
                      <FormLabel>Data e Hora Final</FormLabel>
                      <FormControl>
                        <Input type="datetime-local" {...field} />
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
                        <SelectItem value="completed">Concluído</SelectItem>
                        <SelectItem value="cancelled">Cancelado</SelectItem>
                        <SelectItem value="no-show">Não compareceu</SelectItem>
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
                        placeholder="Alguma observação sobre o agendamento"
                        className="min-h-[80px]"
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
                  onClick={() => setIsCreateDialogOpen(false)}
                >
                  Cancelar
                </Button>
                <Button type="submit" disabled={createAppointment.isPending}>
                  {createAppointment.isPending && (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  )}
                  Salvar
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </DialogContent>
      </Dialog>
      
      {/* Floating action button for mobile */}
      <div className="fixed bottom-6 right-6 md:hidden">
        <Button 
          size="icon" 
          className="h-14 w-14 rounded-full shadow-lg"
          onClick={() => setIsCreateDialogOpen(true)}
        >
          <Plus className="h-6 w-6" />
        </Button>
      </div>
    </div>
  );
}