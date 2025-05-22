import { useState } from "react";
import { Calendar, BarChart3, FileText, Clock, ChevronDown, ChevronUp, ChevronLeft, ChevronRight, CheckCircle2, XCircle, AlertCircle, RefreshCw, UserX, PauseCircle, PlayCircle, Check, Menu, X } from "lucide-react";
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
  const [filters, setFilters] = useState({
    status: "todos",
    profissional: "todos", 
    paciente: "todos",
    procedimento: "todos",
    sala: "todas"
  });
  
  const clearFilters = () => {
    setFilters({
      status: "todos",
      profissional: "todos",
      paciente: "todos", 
      procedimento: "todos",
      sala: "todas"
    });
  };
  
  // Contar filtros ativos
  const getActiveFiltersCount = () => {
    let count = 0;
    if (filters.status !== "todos") count++;
    if (filters.profissional !== "todos") count++;
    if (filters.paciente !== "todos") count++;
    if (filters.procedimento !== "todos") count++;
    if (filters.sala !== "todas") count++;
    return count;
  };
  
  const activeFiltersCount = getActiveFiltersCount();
  
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
            <div className="flex items-center gap-2">
              <h4 className="font-medium">Filtros</h4>
              {activeFiltersCount > 0 && (
                <span className="px-2 py-1 bg-blue-100 text-blue-600 text-xs rounded-full font-medium">
                  {activeFiltersCount} filtro{activeFiltersCount > 1 ? 's' : ''} aplicado{activeFiltersCount > 1 ? 's' : ''}
                </span>
              )}
            </div>
            <Button 
              variant="ghost" 
              size="sm" 
              className="text-primary" 
              onClick={clearFilters}
              disabled={activeFiltersCount === 0}
            >
              Limpar filtros
            </Button>
          </div>
          
          {/* Status */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Status</label>
            <Select value={filters.status} onValueChange={(value) => setFilters({...filters, status: value})}>
              <SelectTrigger className="border-purple-200 focus:ring-purple-500">
                <SelectValue placeholder="Todos" />
              </SelectTrigger>
              <SelectContent className="border-purple-200">
                <SelectItem value="todos" className="flex items-center gap-2">
                  Todos
                </SelectItem>
                <SelectItem value="agendado" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <RefreshCw className="h-4 w-4 text-purple-500" />
                    Agendado
                  </div>
                </SelectItem>
                <SelectItem value="confirmado" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <CheckCircle2 className="h-4 w-4 text-blue-500" />
                    Confirmado
                  </div>
                </SelectItem>
                <SelectItem value="remarcado" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <AlertCircle className="h-4 w-4 text-orange-500" />
                    Remarcado
                  </div>
                </SelectItem>
                <SelectItem value="cancelado" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <XCircle className="h-4 w-4 text-red-500" />
                    Cancelado
                  </div>
                </SelectItem>
                <SelectItem value="nao-compareceu" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <UserX className="h-4 w-4 text-gray-500" />
                    Não compareceu
                  </div>
                </SelectItem>
                <SelectItem value="aguardando" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <PauseCircle className="h-4 w-4 text-yellow-500" />
                    Aguardando
                  </div>
                </SelectItem>
                <SelectItem value="em-atendimento" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <PlayCircle className="h-4 w-4 text-green-500" />
                    Em atendimento
                  </div>
                </SelectItem>
                <SelectItem value="concluido" className="flex items-center gap-2">
                  <div className="flex items-center gap-2">
                    <Check className="h-4 w-4 text-emerald-600" />
                    Concluído
                  </div>
                </SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Profissional */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Profissional</label>
            <Select value={filters.profissional} onValueChange={(value) => setFilters({...filters, profissional: value})}>
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
            <Select value={filters.paciente} onValueChange={(value) => setFilters({...filters, paciente: value})}>
              <SelectTrigger>
                <SelectValue placeholder="Todos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos</SelectItem>
                <SelectItem value="clara">Clara Ribeiro</SelectItem>
                <SelectItem value="fernando">Fernando Ferreira</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Procedimento */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Procedimento</label>
            <Select value={filters.procedimento} onValueChange={(value) => setFilters({...filters, procedimento: value})}>
              <SelectTrigger>
                <SelectValue placeholder="Todos" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos</SelectItem>
                <SelectItem value="limpeza">Limpeza Dental</SelectItem>
                <SelectItem value="canal">Tratamento de Canal</SelectItem>
                <SelectItem value="clareamento">Clareamento a Laser</SelectItem>
              </SelectContent>
            </Select>
          </div>
          
          {/* Sala de atendimento */}
          <div className="space-y-2">
            <label className="text-sm font-medium text-muted-foreground">Sala de atendimento</label>
            <Select value={filters.sala} onValueChange={(value) => setFilters({...filters, sala: value})}>
              <SelectTrigger>
                <SelectValue placeholder="Todas" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todas">Todas</SelectItem>
                <SelectItem value="sala1">Sala 1</SelectItem>
                <SelectItem value="sala2">Sala 2</SelectItem>
                <SelectItem value="sala3">Sala 3</SelectItem>
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
  const [periodFilter, setPeriodFilter] = useState("diaria");
  const [statusFilter, setStatusFilter] = useState("todos");
  const [profissionalFilter, setProfissionalFilter] = useState("todos");
  const [isFiltersExpanded, setIsFiltersExpanded] = useState(false);
  
  // Função para limpar filtros da Visão Geral
  const clearOverviewFilters = () => {
    setStatusFilter("todos");
    setProfissionalFilter("todos");
  };
  
  // Contar filtros ativos na Visão Geral
  const getActiveOverviewFiltersCount = () => {
    let count = 0;
    if (statusFilter !== "todos") count++;
    if (profissionalFilter !== "todos") count++;
    return count;
  };
  
  const activeOverviewFiltersCount = getActiveOverviewFiltersCount();
  
  // Mock data baseado nos filtros
  const getFilteredData = () => {
    const baseData = {
      totalAgendamentos: periodFilter === "diaria" ? 1 : periodFilter === "semanal" ? 7 : periodFilter === "mensal" ? 25 : 150,
      ociosidade: periodFilter === "diaria" ? 98 : periodFilter === "semanal" ? 85 : periodFilter === "mensal" ? 75 : 45,
      listaEspera: 0
    };
    
    return baseData;
  };
  
  const getChartData = () => {
    switch(periodFilter) {
      case "semanal":
        return [12, 15, 8, 25, 18, 22, 10];
      case "mensal":
        return [45, 32, 28, 55, 38, 42, 35, 48, 52, 41, 39, 47];
      case "anual":
        return [180, 165, 142, 198, 175, 156, 189, 201, 178, 192, 167, 185];
      default: // diaria
        return [2, 4, 3, 8, 5, 6, 1];
    }
  };
  
  const data = getFilteredData();
  const chartData = getChartData();
  
  return (
    <div className="space-y-6 p-6">
      {/* Header com filtros */}
      <Card>
        <CardContent className="p-4">
          <div className="flex items-center justify-between mb-4">
            <div className="flex items-center gap-2">
              <span className="font-medium">Filtros</span>
              {activeOverviewFiltersCount > 0 && (
                <span className="text-xs bg-blue-100 text-blue-600 px-2 py-1 rounded">
                  {activeOverviewFiltersCount} filtro aplicado
                </span>
              )}
            </div>
            <div className="flex items-center gap-2">
              <Button 
                variant="ghost" 
                size="sm" 
                className="text-purple-600"
                onClick={clearOverviewFilters}
                disabled={activeOverviewFiltersCount === 0}
              >
                Limpar filtros
              </Button>
              <Button 
                variant="ghost" 
                size="sm"
                onClick={() => setIsFiltersExpanded(!isFiltersExpanded)}
              >
                {isFiltersExpanded ? "Recolher" : "Expandir"}
              </Button>
            </div>
          </div>
          
          {/* Filtro de período sempre visível */}
          <div className="flex items-center gap-2 mb-4">
            <span className="text-sm text-gray-600">Período:</span>
            <span className="bg-gray-100 px-3 py-1 rounded text-sm">18/05/2025 - 24/05/2025</span>
            <Button 
              variant="ghost" 
              size="sm" 
              className="text-purple-600"
              onClick={() => setIsFiltersExpanded(true)}
            >
              + Adicionar filtro
            </Button>
          </div>
          
          {/* Filtros expandidos */}
          {isFiltersExpanded && (
            <div className="space-y-4 pt-4 border-t">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                {/* Período com calendário */}
                <div className="space-y-2">
                  <label className="text-sm font-medium text-purple-600 flex items-center gap-2">
                    <Calendar className="w-4 h-4" />
                    Período
                  </label>
                  <div className="text-sm text-gray-600">18/05/2025 - 24/05/2025</div>
                </div>
                
                {/* Profissionais */}
                <div className="space-y-2">
                  <label className="text-sm font-medium text-gray-600 flex items-center gap-2">
                    <User className="w-4 h-4" />
                    Profissionais
                  </label>
                  <Select value={profissionalFilter} onValueChange={setProfissionalFilter}>
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
                
                {/* Status */}
                <div className="space-y-2">
                  <label className="text-sm font-medium text-gray-600">Status</label>
                  <Select value={statusFilter} onValueChange={setStatusFilter}>
                    <SelectTrigger>
                      <SelectValue placeholder="Todos" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="todos">Todos</SelectItem>
                      <SelectItem value="agendado">Agendado</SelectItem>
                      <SelectItem value="confirmado">Confirmado</SelectItem>
                      <SelectItem value="cancelado">Cancelado</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Cards de estatísticas principais */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card className="border-l-4 border-l-green-500">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">Total de agendamentos</p>
                <p className="text-2xl font-bold">{data.totalAgendamentos}</p>
              </div>
              <div className="text-green-500 text-sm font-medium flex items-center gap-1">
                <span>↗</span> {periodFilter === "diaria" ? "0%" : periodFilter === "semanal" ? "15%" : periodFilter === "mensal" ? "25%" : "45%"}
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="border-l-4 border-l-blue-500">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">Ociosidade</p>
                <p className="text-2xl font-bold">{data.ociosidade} %</p>
              </div>
              <div className="text-red-500 text-sm font-medium flex items-center gap-1">
                <span>↘</span> -{periodFilter === "diaria" ? "2%" : periodFilter === "semanal" ? "8%" : periodFilter === "mensal" ? "15%" : "25%"}
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="border-l-4 border-l-purple-500">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">Pacientes na lista de espera</p>
                <p className="text-2xl font-bold">{data.listaEspera}</p>
              </div>
              <div className="text-green-500 text-sm font-medium flex items-center gap-1">
                <span>↗</span> 0%
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos principais */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Agendamentos por período */}
        <Card>
          <CardContent className="p-6">
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <h3 className="font-semibold">Agendamentos por período</h3>
                <div className="flex gap-2">
                  {["Diária", "Semanal", "Mensal", "Anual"].map((period) => (
                    <Button
                      key={period}
                      variant={periodFilter === period.toLowerCase() ? "default" : "ghost"}
                      size="sm"
                      className={periodFilter === period.toLowerCase() ? "bg-purple-600 text-white" : ""}
                      onClick={() => setPeriodFilter(period.toLowerCase())}
                    >
                      {period}
                    </Button>
                  ))}
                </div>
              </div>
              
              {/* Gráfico dinâmico */}
              <div className="h-40 flex items-end justify-center space-x-2">
                {chartData.map((value, index) => {
                  const maxValue = Math.max(...chartData);
                  const height = (value / maxValue) * 120;
                  const isHighest = value === maxValue;
                  
                  return (
                    <div key={index} className="flex flex-col items-center">
                      <div 
                        className={`w-8 transition-all duration-500 ${isHighest ? 'bg-green-400' : 'bg-gray-200'}`}
                        style={{ height: `${height}px` }}
                      ></div>
                      {periodFilter === "diaria" && (
                        <span className="text-xs text-muted-foreground mt-1">
                          {["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"][index]}
                        </span>
                      )}
                      {periodFilter === "semanal" && (
                        <span className="text-xs text-muted-foreground mt-1">
                          S{index + 1}
                        </span>
                      )}
                      {periodFilter === "mensal" && index % 2 === 0 && (
                        <span className="text-xs text-muted-foreground mt-1">
                          {index + 1}
                        </span>
                      )}
                      {periodFilter === "anual" && (
                        <span className="text-xs text-muted-foreground mt-1">
                          {["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"][index]}
                        </span>
                      )}
                    </div>
                  );
                })}
              </div>
              
              <div className="text-center text-sm text-muted-foreground">
                <span className="text-green-600">—</span> Agendamentos
                <span className="ml-4 text-purple-600">--</span> Média
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Agendamentos por status */}
        <Card>
          <CardContent className="p-6">
            <h3 className="font-semibold mb-4">Agendamentos por status</h3>
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <RefreshCw className="h-4 w-4 text-purple-500" />
                  <span className="text-sm">Agendado</span>
                  <span className="text-xs text-muted-foreground">1</span>
                </div>
                <span className="text-sm font-medium">100%</span>
              </div>
              
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <CheckCircle2 className="h-4 w-4 text-blue-500" />
                  <span className="text-sm">Confirmado</span>
                  <span className="text-xs text-muted-foreground">0</span>
                </div>
                <span className="text-sm font-medium">0%</span>
              </div>
              
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <UserX className="h-4 w-4 text-gray-500" />
                  <span className="text-sm">Não compareceu</span>
                  <span className="text-xs text-muted-foreground">0</span>
                </div>
                <span className="text-sm font-medium">0%</span>
              </div>
              
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <Check className="h-4 w-4 text-green-500" />
                  <span className="text-sm">Concluído</span>
                  <span className="text-xs text-muted-foreground">0</span>
                </div>
                <span className="text-sm font-medium">0%</span>
              </div>
              
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <XCircle className="h-4 w-4 text-red-500" />
                  <span className="text-sm">Cancelado</span>
                  <span className="text-xs text-muted-foreground">0</span>
                </div>
                <span className="text-sm font-medium">0%</span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Cards informativos */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card className="bg-blue-50 border-blue-200">
          <CardContent className="p-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center">
                <CheckCircle2 className="h-4 w-4 text-white" />
              </div>
              <span className="font-medium text-blue-700">Pacientes mais frequentes</span>
            </div>
            <Button variant="ghost" size="sm" className="text-blue-600 p-0 h-auto">
              ver mais
            </Button>
            <div className="mt-3 space-y-2">
              <div className="flex items-center gap-2">
                <div className="w-6 h-6 bg-yellow-400 rounded-full flex items-center justify-center">
                  <span className="text-xs font-bold">1</span>
                </div>
                <span className="text-sm">Clara Ribeiro (Paciente)</span>
                <span className="text-xs text-muted-foreground ml-auto">100%</span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="bg-pink-50 border-pink-200">
          <CardContent className="p-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="w-8 h-8 bg-pink-500 rounded-full flex items-center justify-center">
                <AlertCircle className="h-4 w-4 text-white" />
              </div>
              <span className="font-medium text-pink-700">Ociosidade por sala</span>
            </div>
            <Button variant="ghost" size="sm" className="text-pink-600 p-0 h-auto">
              ver mais
            </Button>
            <div className="mt-3 text-center">
              <AlertCircle className="h-8 w-8 mx-auto text-gray-400 mb-2" />
              <p className="text-sm text-gray-600">Não há nada aqui!</p>
              <p className="text-xs text-gray-500">Nenhuma venda encontrada para os filtros selecionados</p>
            </div>
          </CardContent>
        </Card>

        <Card className="bg-yellow-50 border-yellow-200">
          <CardContent className="p-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="w-8 h-8 bg-yellow-500 rounded-full flex items-center justify-center">
                <Clock className="h-4 w-4 text-white" />
              </div>
              <span className="font-medium text-yellow-700">Ociosidade por profissional</span>
            </div>
            <Button variant="ghost" size="sm" className="text-yellow-600 p-0 h-auto">
              ver mais
            </Button>
            <div className="mt-3 space-y-2">
              <div className="flex items-center gap-2">
                <div className="w-6 h-6 bg-yellow-400 rounded-full flex items-center justify-center">
                  <span className="text-xs font-bold">1</span>
                </div>
                <span className="text-sm">FERNANDO FERREIRA</span>
                <span className="text-xs text-muted-foreground ml-auto">98%</span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="bg-purple-50 border-purple-200">
          <CardContent className="p-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="w-8 h-8 bg-purple-500 rounded-full flex items-center justify-center">
                <BarChart3 className="h-4 w-4 text-white" />
              </div>
              <span className="font-medium text-purple-700">Procedimentos mais frequentes</span>
            </div>
            <Button variant="ghost" size="sm" className="text-purple-600 p-0 h-auto">
              ver mais
            </Button>
            <div className="mt-3 space-y-2">
              <div className="flex items-center gap-2">
                <div className="w-6 h-6 bg-yellow-400 rounded-full flex items-center justify-center">
                  <span className="text-xs font-bold">1</span>
                </div>
                <span className="text-sm">Clareamento a Laser</span>
                <span className="text-xs text-muted-foreground ml-auto">100%</span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos de movimento */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Dias mais movimentados */}
        <Card>
          <CardContent className="p-6">
            <h3 className="font-semibold mb-4">Dias mais movimentados</h3>
            <div className="h-40 flex items-end justify-center space-x-1">
              {["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"].map((day, index) => (
                <div key={day} className="flex flex-col items-center">
                  <div 
                    className={`w-8 ${index === 3 ? 'bg-purple-500 h-32' : 'bg-gray-200 h-8'}`}
                  ></div>
                  <span className="text-xs text-muted-foreground mt-1">{day}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Horários mais movimentados */}
        <Card>
          <CardContent className="p-6">
            <h3 className="font-semibold mb-4">Horários mais movimentados</h3>
            <div className="h-40 grid grid-cols-12 gap-1">
              {Array.from({ length: 24 }, (_, i) => (
                <div key={i} className="flex flex-col">
                  <div 
                    className={`w-full ${i >= 16 && i <= 17 ? 'bg-purple-500' : 'bg-gray-200'} ${i >= 16 && i <= 17 ? 'h-20' : 'h-2'}`}
                  ></div>
                  {i % 4 === 0 && (
                    <span className="text-xs text-muted-foreground mt-1 transform -rotate-45 origin-left">
                      {i}h
                    </span>
                  )}
                </div>
              ))}
            </div>
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
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);

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
    <div className="flex">
      {/* Botão para abrir/fechar sidebar */}
      {!isSidebarOpen && (
        <Button
          variant="outline"
          size="icon"
          className="fixed top-4 left-4 z-50 shadow-lg"
          onClick={() => setIsSidebarOpen(true)}
        >
          <Menu className="h-4 w-4" />
        </Button>
      )}

      {/* Sidebar de navegação */}
      {isSidebarOpen && (
        <div className="w-64 shrink-0 relative">
          {/* Botão para fechar sidebar */}
          <Button
            variant="ghost"
            size="icon"
            className="absolute top-2 right-2 z-10"
            onClick={() => setIsSidebarOpen(false)}
          >
            <X className="h-4 w-4" />
          </Button>
          
          <Card>
            <CardContent className="p-4 pt-12">
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
      )}

      {/* Conteúdo principal */}
      <div className={cn("flex-1 transition-all duration-300", isSidebarOpen ? "ml-6" : "ml-0")}>
        {renderContent()}
      </div>
    </div>
  );
}