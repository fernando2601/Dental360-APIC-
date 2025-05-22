import { useState } from "react";
import { Calendar, BarChart3, FileText, Clock, ChevronDown, ChevronUp, ChevronLeft, ChevronRight } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { cn } from "@/lib/utils";
import { format, addMonths, subMonths, startOfMonth, endOfMonth, eachDayOfInterval, isSameMonth, isToday } from "date-fns";
import { ptBR } from "date-fns/locale";
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

function MiniCalendar() {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [isAdvancedOpen, setIsAdvancedOpen] = useState(false);
  
  const monthStart = startOfMonth(currentDate);
  const monthEnd = endOfMonth(currentDate);
  const days = eachDayOfInterval({ start: monthStart, end: monthEnd });
  
  const weekDays = ['D', 'S', 'T', 'Q', 'Q', 'S', 'S'];
  
  const goToPrevMonth = () => setCurrentDate(subMonths(currentDate, 1));
  const goToNextMonth = () => setCurrentDate(addMonths(currentDate, 1));
  
  return (
    <div className="space-y-4">
      {/* Mini Calendário */}
      <Card>
        <CardContent className="p-4">
          {/* Header do calendário */}
          <div className="flex items-center justify-between mb-4">
            <Button variant="ghost" size="sm" onClick={goToPrevMonth}>
              <ChevronLeft className="h-4 w-4" />
            </Button>
            <span className="font-medium">
              {format(currentDate, 'MMMM yyyy', { locale: ptBR })}
            </span>
            <Button variant="ghost" size="sm" onClick={goToNextMonth}>
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
          
          {/* Dias da semana */}
          <div className="grid grid-cols-7 gap-1 mb-2">
            {weekDays.map((day, index) => (
              <div key={index} className="text-center text-xs font-medium text-muted-foreground p-1">
                {day}
              </div>
            ))}
          </div>
          
          {/* Dias do mês */}
          <div className="grid grid-cols-7 gap-1">
            {days.map((day, index) => (
              <Button
                key={index}
                variant={isToday(day) ? "default" : "ghost"}
                size="sm"
                className={cn(
                  "h-8 w-8 p-0 text-xs",
                  !isSameMonth(day, currentDate) && "text-muted-foreground opacity-50",
                  isToday(day) && "bg-primary text-primary-foreground"
                )}
              >
                {format(day, 'd')}
              </Button>
            ))}
          </div>
        </CardContent>
      </Card>
      
      {/* Filtros */}
      <Card>
        <CardContent className="p-4 space-y-4">
          <div className="flex items-center justify-between">
            <h4 className="font-medium">Filtros</h4>
            <Button variant="ghost" size="sm" className="text-primary">
              Limpar filtros
            </Button>
          </div>
          
          {/* Status */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Status</label>
            <Select defaultValue="todos">
              <SelectTrigger>
                <SelectValue placeholder="Todos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos</SelectItem>
                <SelectItem value="agendado">Agendado</SelectItem>
                <SelectItem value="confirmado">Confirmado</SelectItem>
                <SelectItem value="realizado">Realizado</SelectItem>
                <SelectItem value="cancelado">Cancelado</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Profissional */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Profissional</label>
            <Select defaultValue="todos">
              <SelectTrigger>
                <SelectValue placeholder="Todos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos</SelectItem>
                <SelectItem value="dr-silva">Dr. Silva</SelectItem>
                <SelectItem value="dra-santos">Dra. Santos</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Paciente */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Paciente</label>
            <Select defaultValue="todos">
              <SelectTrigger>
                <SelectValue placeholder="Todos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Procedimento */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Procedimento</label>
            <Select defaultValue="todos">
              <SelectTrigger>
                <SelectValue placeholder="Todos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos</SelectItem>
                <SelectItem value="limpeza">Limpeza Dental</SelectItem>
                <SelectItem value="canal">Tratamento de Canal</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Sala de atendimento */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Sala de atendimento</label>
            <Select defaultValue="todas">
              <SelectTrigger>
                <SelectValue placeholder="Todas" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todas">Todas</SelectItem>
                <SelectItem value="sala1">Sala 1</SelectItem>
                <SelectItem value="sala2">Sala 2</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Opções Avançadas */}
          <Collapsible open={isAdvancedOpen} onOpenChange={setIsAdvancedOpen}>
            <CollapsibleTrigger asChild>
              <Button variant="ghost" className="w-full justify-between p-0 h-auto text-primary">
                Opções avançadas
                {isAdvancedOpen ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
              </Button>
            </CollapsibleTrigger>
            <CollapsibleContent className="space-y-3 mt-3">
              <div className="flex items-center justify-between">
                <span className="text-sm">Mostrar finais de semana</span>
                <Switch defaultChecked />
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm">Mostrar lembretes</span>
                <Switch defaultChecked />
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm">Mostrar feriados</span>
                <Switch defaultChecked />
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm">Mostrar eventos do sistema</span>
                <Switch defaultChecked />
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm">Mostrar eventos no mês</span>
                <Switch />
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm">Sobrepor eventos</span>
                <Switch />
              </div>
            </CollapsibleContent>
          </Collapsible>
        </CardContent>
      </Card>
    </div>
  );
}

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
        
        {/* Mini calendário e filtros - aparece apenas quando "Agenda" está selecionada */}
        {currentView === "agenda" && <MiniCalendar />}
      </div>

      {/* Conteúdo principal */}
      <div className="flex-1">
        {renderContent()}
      </div>
    </div>
  );
}