import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { format, addDays } from "date-fns";
import { ptBR } from "date-fns/locale";
import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight, Plus } from "lucide-react";

export default function Calendar() {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [viewMode, setViewMode] = useState("week");
  
  // Fetch appointments data
  const { data: appointments = [] } = useQuery({
    queryKey: ['/api/appointments'],
  });

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
  const timeSlots = Array(24).fill(0).map((_, i) => {
    const hour = i;
    return {
      hour,
      displayHour: `${hour}:00`,
    };
  });

  // Get appointments for a specific hour and day
  const getAppointmentsForTimeSlot = (date: Date, hour: number) => {
    return appointments.filter((a: any) => {
      const appointmentDate = new Date(a.startTime);
      return format(appointmentDate, 'yyyy-MM-dd') === format(date, 'yyyy-MM-dd') && 
             appointmentDate.getHours() === hour;
    });
  };

  return (
    <div className="flex flex-col h-full">
      {/* Top navigation */}
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Agenda</h1>
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="sm" onClick={goToToday}>
            Hoje
          </Button>
          <Button variant="ghost" size="sm" onClick={goToPreviousWeek}>
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <span className="text-sm">
            {format(weekDays[0].date, "d 'de' MMM.", { locale: ptBR })} - {format(weekDays[6].date, "d 'de' MMM. 'de' yyyy", { locale: ptBR })}
          </span>
          <Button variant="ghost" size="sm" onClick={goToNextWeek}>
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
        </div>
      </div>

      {/* Calendar grid */}
      <div className="flex-1 overflow-auto">
        <div className="min-w-[800px]">
          {/* Days of the week header */}
          <div className="grid grid-cols-7 border-b">
            {weekDays.map((day, index) => (
              <div 
                key={index} 
                className={`text-center py-2 ${day.isToday ? "bg-primary text-primary-foreground rounded-t-lg" : ""}`}
              >
                <div className="text-sm font-medium">{day.day}</div>
                <div className="text-xs text-muted-foreground">{day.weekday}</div>
              </div>
            ))}
          </div>

          {/* Time slots */}
          <div className="relative">
            {timeSlots.map((slot, slotIndex) => (
              <div key={slotIndex} className="grid grid-cols-7 border-b h-12">
                <div className="absolute -left-12 text-xs text-muted-foreground flex items-center h-12">
                  {slot.displayHour}
                </div>
                {weekDays.map((day, dayIndex) => {
                  const appointments = getAppointmentsForTimeSlot(day.date, slot.hour);
                  return (
                    <div 
                      key={`${slotIndex}-${dayIndex}`} 
                      className={`border-r relative ${day.isToday ? "bg-primary/5" : ""}`}
                    >
                      {appointments.map((appointment: any, i: number) => (
                        <div 
                          key={i}
                          className="absolute top-0 left-1 right-1 bg-blue-100 text-blue-800 rounded p-1 text-xs overflow-hidden whitespace-nowrap"
                          style={{ 
                            zIndex: 10,
                            height: `${Math.min(appointments.length * 20, 100)}%` 
                          }}
                        >
                          {appointment.title || "Agendamento"}
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

      {/* Floating action button */}
      <div className="fixed bottom-6 right-6">
        <Button 
          size="icon" 
          className="h-14 w-14 rounded-full shadow-lg"
        >
          <Plus className="h-6 w-6" />
        </Button>
      </div>
    </div>
  );
}

// Import necessary components
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";