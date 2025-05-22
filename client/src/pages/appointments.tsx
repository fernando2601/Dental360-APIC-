import { useState } from "react";
import { Calendar, BarChart3, FileText, Clock, ChevronDown, ChevronUp, ChevronLeft, ChevronRight, CheckCircle2, XCircle, AlertCircle, RefreshCw, UserX, PauseCircle, PlayCircle, Check, Menu, X, User } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
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
  const [selectedPeriodOption, setSelectedPeriodOption] = useState("este-mes");
  const [showProfissionalSearch, setShowProfissionalSearch] = useState(false);
  const [showStatusSearch, setShowStatusSearch] = useState(false);
  const [profissionalSearch, setProfissionalSearch] = useState("");
  const [statusSearch, setStatusSearch] = useState("");
  const [showFrequentPatientsModal, setShowFrequentPatientsModal] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(25);
  
  // Função para limpar filtros da Visão Geral
  const clearOverviewFilters = () => {
    setStatusFilter("todos");
    setProfissionalFilter("todos");
    setSelectedPeriodOption("este-mes");
    setProfissionalSearch("");
    setStatusSearch("");
    setShowProfissionalSearch(false);
    setShowStatusSearch(false);
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
    // Dados baseados na seleção do período
    switch(selectedPeriodOption) {
      case "hoje":
        return { totalAgendamentos: 3, ociosidade: 85, listaEspera: 1 };
      case "esta-semana":
        return { totalAgendamentos: 15, ociosidade: 65, listaEspera: 2 };
      case "este-mes":
        return { totalAgendamentos: 89, ociosidade: 25, listaEspera: 5 };
      case "ultimos-7":
        return { totalAgendamentos: 12, ociosidade: 70, listaEspera: 1 };
      case "ultimos-30":
        return { totalAgendamentos: 156, ociosidade: 15, listaEspera: 8 };
      default:
        return { totalAgendamentos: 89, ociosidade: 25, listaEspera: 5 };
    }
  };
  
  const getChartData = () => {
    switch(selectedPeriodOption) {
      case "hoje":
        return Array.from({length: 24}, (_, i) => Math.floor(Math.random() * 3)); // 24 horas
      case "esta-semana":
        return [8, 12, 15, 9, 18, 22, 11]; // 7 dias
      case "este-mes":
        return Array.from({length: 31}, (_, i) => Math.floor(Math.random() * 8 + 1)); // 31 dias
      case "ultimos-7":
        return [5, 8, 12, 7, 15, 18, 9]; // 7 dias
      case "ultimos-30":
        return Array.from({length: 30}, (_, i) => Math.floor(Math.random() * 12 + 2)); // 30 dias
      default:
        return Array.from({length: 31}, (_, i) => Math.floor(Math.random() * 8 + 1));
    }
  };
  
  const getStatusData = () => {
    const total = getFilteredData().totalAgendamentos;
    if (statusFilter !== "todos") {
      // Se um status específico foi selecionado, mostre apenas esse
      return [
        { name: statusFilter, value: total, color: "bg-purple-500" }
      ];
    }
    
    // Distribuição baseada no período selecionado
    switch(selectedPeriodOption) {
      case "hoje":
        return [
          { name: "Agendado", value: 1, color: "bg-purple-500" },
          { name: "Confirmado", value: 0, color: "bg-blue-500" },
          { name: "Não compareceu", value: 0, color: "bg-gray-500" },
          { name: "Concluído", value: 0, color: "bg-green-500" }
        ];
      case "este-mes":
        return [
          { name: "Agendado", value: 25, color: "bg-purple-500" },
          { name: "Confirmado", value: 20, color: "bg-blue-500" },
          { name: "Não compareceu", value: 15, color: "bg-gray-500" },
          { name: "Concluído", value: 29, color: "bg-green-500" }
        ];
      default:
        return [
          { name: "Agendado", value: 8, color: "bg-purple-500" },
          { name: "Confirmado", value: 5, color: "bg-blue-500" },
          { name: "Não compareceu", value: 1, color: "bg-gray-500" },
          { name: "Concluído", value: 1, color: "bg-green-500" }
        ];
    }
  };
  
  // Dados dos pacientes mais frequentes
  const getFrequentPatientsData = () => {
    return [
      { nome: "Clara Ribeiro", consultas: 12, porcentagem: 15.2 },
      { nome: "Fernando Ferreira", consultas: 8, porcentagem: 10.1 },
      { nome: "Maria Santos", consultas: 7, porcentagem: 8.9 },
      { nome: "João Silva", consultas: 6, porcentagem: 7.6 },
      { nome: "Ana Costa", consultas: 5, porcentagem: 6.3 },
      { nome: "Pedro Oliveira", consultas: 5, porcentagem: 6.3 },
      { nome: "Lucia Mendes", consultas: 4, porcentagem: 5.1 },
      { nome: "Carlos Alberto", consultas: 4, porcentagem: 5.1 },
      { nome: "Patricia Lima", consultas: 3, porcentagem: 3.8 },
      { nome: "Roberto Souza", consultas: 3, porcentagem: 3.8 },
      { nome: "Juliana Alves", consultas: 3, porcentagem: 3.8 },
      { nome: "Marcos Paulo", consultas: 2, porcentagem: 2.5 },
      { nome: "Sandra Dias", consultas: 2, porcentagem: 2.5 },
      { nome: "Ricardo Gomes", consultas: 2, porcentagem: 2.5 },
      { nome: "Angela Rosa", consultas: 2, porcentagem: 2.5 },
      { nome: "Daniel Campos", consultas: 1, porcentagem: 1.3 },
      { nome: "Fernanda Cruz", consultas: 1, porcentagem: 1.3 },
      { nome: "Antonio Neves", consultas: 1, porcentagem: 1.3 },
      { nome: "Beatriz Martins", consultas: 1, porcentagem: 1.3 },
      { nome: "Eduardo Rocha", consultas: 1, porcentagem: 1.3 }
    ];
  };
  
  // Lógica de paginação
  const allPatients = getFrequentPatientsData();
  const totalPages = Math.ceil(allPatients.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentPatients = allPatients.slice(startIndex, endIndex);
  
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
              <div className="flex gap-6">
                {/* Seção esquerda - Filtros em colunas */}
                <div className="flex flex-col gap-4 w-80">
                  {/* Período */}
                  <div className="space-y-3">
                    <Button className="w-full bg-purple-600 hover:bg-purple-700 text-white flex items-center gap-2">
                      <Calendar className="w-4 h-4" />
                      Período
                    </Button>
                    
                    <div className="space-y-2 text-sm">
                      <div className="flex items-center gap-2">
                        <input 
                          type="radio" 
                          name="periodo" 
                          value="hoje" 
                          checked={selectedPeriodOption === "hoje"}
                          onChange={(e) => setSelectedPeriodOption(e.target.value)}
                          className="text-purple-600" 
                        />
                        <span>Hoje</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <input 
                          type="radio" 
                          name="periodo" 
                          value="esta-semana" 
                          checked={selectedPeriodOption === "esta-semana"}
                          onChange={(e) => setSelectedPeriodOption(e.target.value)}
                          className="text-purple-600" 
                        />
                        <span>Esta semana</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <input 
                          type="radio" 
                          name="periodo" 
                          value="este-mes" 
                          checked={selectedPeriodOption === "este-mes"}
                          onChange={(e) => setSelectedPeriodOption(e.target.value)}
                          className="text-purple-600" 
                        />
                        <span>Este mês</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <input 
                          type="radio" 
                          name="periodo" 
                          value="ultimos-7" 
                          checked={selectedPeriodOption === "ultimos-7"}
                          onChange={(e) => setSelectedPeriodOption(e.target.value)}
                          className="text-purple-600" 
                        />
                        <span>Últimos 7 dias</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <input 
                          type="radio" 
                          name="periodo" 
                          value="ultimos-30" 
                          checked={selectedPeriodOption === "ultimos-30"}
                          onChange={(e) => setSelectedPeriodOption(e.target.value)}
                          className="text-purple-600" 
                        />
                        <span>Últimos 30 dias</span>
                      </div>
                    </div>
                  </div>
                  
                  {/* Profissionais */}
                  <div className="space-y-3">
                    <Button 
                      variant={showProfissionalSearch ? "default" : "outline"} 
                      className={`w-full flex items-center gap-2 ${showProfissionalSearch ? 'bg-purple-600 text-white' : ''}`}
                      onClick={() => {
                        setShowProfissionalSearch(!showProfissionalSearch);
                        setShowStatusSearch(false); // Esconder Status quando clicar em Profissionais
                      }}
                    >
                      <UserX className="w-4 h-4" />
                      Profissionais
                    </Button>
                  </div>
                  
                  {/* Status */}
                  <div className="space-y-3">
                    <Button 
                      variant={showStatusSearch ? "default" : "outline"}
                      className={`w-full ${showStatusSearch ? 'bg-purple-600 text-white' : ''}`}
                      onClick={() => {
                        setShowStatusSearch(!showStatusSearch);
                        setShowProfissionalSearch(false); // Esconder Profissionais quando clicar em Status
                      }}
                    >
                      Status
                    </Button>
                  </div>
                </div>
                
                {/* Seção direita - Conteúdo dinâmico */}
                <div className="flex-1">
                  {/* Busca de Profissionais */}
                  {showProfissionalSearch && (
                    <div className="bg-white rounded-lg p-4 border">
                      <div className="space-y-4">
                        <div className="flex items-center justify-between">
                          <span className="font-medium">Profissionais</span>
                          <Button 
                            variant="ghost" 
                            size="sm" 
                            className="text-purple-600"
                            onClick={() => {
                              setProfissionalFilter("todos");
                              setProfissionalSearch("");
                            }}
                          >
                            Limpar
                          </Button>
                        </div>
                        
                        <div className="relative">
                          <input
                            type="text"
                            placeholder="Digite"
                            value={profissionalSearch}
                            onChange={(e) => setProfissionalSearch(e.target.value)}
                            className="w-full px-3 py-2 border rounded-lg text-sm"
                          />
                        </div>
                        
                        <div className="space-y-2 max-h-40 overflow-y-auto">
                          {["FERNANDO FERREIRA NERI", "DRA. SANTOS", "DR. SILVA", "DR. CARLOS MENDES", "DRA. ANA PAULA"].filter(prof => 
                            prof.toLowerCase().includes(profissionalSearch.toLowerCase())
                          ).map((prof, index) => (
                            <div 
                              key={index}
                              className={`p-2 hover:bg-gray-100 cursor-pointer rounded text-sm ${
                                profissionalFilter === prof.toLowerCase().replace(/\s+/g, '-') ? 'bg-purple-100 text-purple-600' : ''
                              }`}
                              onClick={() => {
                                setProfissionalFilter(prof.toLowerCase().replace(/\s+/g, '-'));
                                setProfissionalSearch(prof);
                              }}
                            >
                              {prof}
                            </div>
                          ))}
                        </div>
                      </div>
                    </div>
                  )}
                  
                  {/* Busca de Status */}
                  {showStatusSearch && (
                    <div className="bg-white rounded-lg p-4 border">
                      <div className="space-y-4">
                        <div className="flex items-center justify-between">
                          <span className="font-medium">Status</span>
                          <Button 
                            variant="ghost" 
                            size="sm" 
                            className="text-purple-600"
                            onClick={() => {
                              setStatusFilter("todos");
                              setStatusSearch("");
                            }}
                          >
                            Limpar
                          </Button>
                        </div>
                        
                        <div className="relative">
                          <input
                            type="text"
                            placeholder="Digite"
                            value={statusSearch}
                            onChange={(e) => setStatusSearch(e.target.value)}
                            className="w-full px-3 py-2 border rounded-lg text-sm"
                          />
                        </div>
                        
                        <div className="space-y-2 max-h-40 overflow-y-auto">
                          {["Agendado", "Confirmado", "Remarcado", "Cancelado", "Não compareceu"].filter(status => 
                            status.toLowerCase().includes(statusSearch.toLowerCase())
                          ).map((status, index) => (
                            <div 
                              key={index}
                              className={`p-2 hover:bg-gray-100 cursor-pointer rounded text-sm ${
                                statusFilter === status.toLowerCase().replace(/\s+/g, '-') ? 'bg-purple-100 text-purple-600' : ''
                              }`}
                              onClick={() => {
                                setStatusFilter(status.toLowerCase().replace(/\s+/g, '-'));
                                setStatusSearch(status);
                              }}
                            >
                              {status}
                            </div>
                          ))}
                        </div>
                      </div>
                    </div>
                  )}
                  
                  {/* Calendário menor (só quando Período está ativo) */}
                  {!showProfissionalSearch && !showStatusSearch && (
                    <div className="bg-white rounded-lg p-3 border max-w-xs">
                      <div className="flex items-center justify-between mb-3">
                        <span className="text-sm font-medium">Mai 2025</span>
                        <div className="flex items-center gap-1">
                          <Button variant="ghost" size="sm">
                            <ChevronLeft className="w-3 h-3" />
                          </Button>
                          <Button variant="ghost" size="sm">
                            <ChevronRight className="w-3 h-3" />
                          </Button>
                        </div>
                      </div>
                      
                      <div className="grid grid-cols-7 gap-1 mb-1">
                        {['D', 'S', 'T', 'Q', 'Q', 'S', 'S'].map((day, index) => (
                          <div key={index} className="text-center text-xs text-gray-500 p-1">
                            {day}
                          </div>
                        ))}
                      </div>
                      
                      <div className="grid grid-cols-7 gap-1">
                        {[27, 28, 29, 30].map((day) => (
                          <div key={`prev-${day}`} className="text-center p-1 text-gray-300 text-xs">
                            {day}
                          </div>
                        ))}
                        
                        {Array.from({length: 31}, (_, i) => i + 1).map((day) => (
                          <div 
                            key={day} 
                            className={`text-center p-1 text-xs cursor-pointer hover:bg-gray-100 rounded ${
                              day === 18 ? 'bg-purple-600 text-white' : 
                              day === 24 ? 'bg-purple-600 text-white' :
                              day >= 18 && day <= 24 ? 'bg-purple-100 text-purple-600' : ''
                            }`}
                          >
                            {day}
                          </div>
                        ))}
                        
                        {[1, 2, 3, 4, 5, 6, 7].map((day) => (
                          <div key={`next-${day}`} className="text-center p-1 text-gray-300 text-xs">
                            {day}
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
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
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card className="bg-blue-50 border-blue-200">
          <CardContent className="p-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center">
                <CheckCircle2 className="h-4 w-4 text-white" />
              </div>
              <span className="font-medium text-blue-700">Pacientes mais frequentes</span>
            </div>
            <Dialog>
              <DialogTrigger asChild>
                <Button variant="ghost" size="sm" className="text-blue-600 p-0 h-auto">
                  ver mais
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-4xl max-h-[80vh]">
                <DialogHeader>
                  <DialogTitle>Pacientes mais frequentes</DialogTitle>
                </DialogHeader>
                
                <div className="space-y-4">
                  {/* Conteúdo principal */}
                  {allPatients.length === 0 ? (
                    /* Estado vazio */
                    <div className="flex flex-col items-center justify-center py-12">
                      <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center mb-4">
                        <AlertCircle className="w-6 h-6 text-purple-500" />
                      </div>
                      <h3 className="text-lg font-medium text-gray-900 mb-2">Hmm, está vazio por aqui!</h3>
                      <p className="text-gray-500 text-center">Nenhum registro encontrado.</p>
                    </div>
                  ) : (
                    /* Lista de pacientes */
                    <div className="space-y-3 max-h-96 overflow-y-auto">
                      {currentPatients.map((patient, index) => (
                        <div key={index} className="flex items-center justify-between py-2 px-4 border-b">
                          <div className="flex items-center gap-3">
                            <div className="w-6 h-6 bg-yellow-400 rounded-full flex items-center justify-center">
                              <span className="text-xs font-bold text-white">{startIndex + index + 1}</span>
                            </div>
                            <span className="text-sm">{patient.nome} (Paciente)</span>
                          </div>
                          <div className="flex items-center gap-8">
                            <span className="text-sm font-medium">{patient.consultas}</span>
                            <span className="text-sm text-gray-500 w-12 text-right">{patient.porcentagem}%</span>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                  
                  {/* Controles de paginação - sempre visíveis */}
                  <div className="flex items-center justify-between pt-4 border-t">
                    <div className="flex items-center gap-2">
                      <Select value={itemsPerPage.toString()} onValueChange={(value) => {
                        setItemsPerPage(parseInt(value));
                        setCurrentPage(1);
                      }}>
                        <SelectTrigger className="w-40">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="10">10 por página</SelectItem>
                          <SelectItem value="25">25 por página</SelectItem>
                          <SelectItem value="50">50 por página</SelectItem>
                          <SelectItem value="100">100 por página</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    
                    <div className="flex items-center gap-2">
                      <Button 
                        variant="ghost" 
                        size="sm"
                        onClick={() => setCurrentPage(1)}
                        disabled={currentPage === 1 || allPatients.length === 0}
                      >
                        <ChevronLeft className="w-4 h-4" />
                        <ChevronLeft className="w-4 h-4 -ml-1" />
                      </Button>
                      <Button 
                        variant="ghost" 
                        size="sm"
                        onClick={() => setCurrentPage(currentPage - 1)}
                        disabled={currentPage === 1 || allPatients.length === 0}
                      >
                        <ChevronLeft className="w-4 h-4" />
                      </Button>
                      
                      {allPatients.length > 0 && (
                        <div className="flex items-center gap-1">
                          {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                            let pageNum;
                            if (totalPages <= 5) {
                              pageNum = i + 1;
                            } else if (currentPage <= 3) {
                              pageNum = i + 1;
                            } else if (currentPage >= totalPages - 2) {
                              pageNum = totalPages - 4 + i;
                            } else {
                              pageNum = currentPage - 2 + i;
                            }
                            
                            return (
                              <Button
                                key={pageNum}
                                variant={currentPage === pageNum ? "default" : "ghost"}
                                size="sm"
                                className={currentPage === pageNum ? "bg-purple-600 text-white" : ""}
                                onClick={() => setCurrentPage(pageNum)}
                              >
                                {pageNum}
                              </Button>
                            );
                          })}
                        </div>
                      )}
                      
                      <Button 
                        variant="ghost" 
                        size="sm"
                        onClick={() => setCurrentPage(currentPage + 1)}
                        disabled={currentPage === totalPages || allPatients.length === 0}
                      >
                        <ChevronRight className="w-4 h-4" />
                      </Button>
                      <Button 
                        variant="ghost" 
                        size="sm"
                        onClick={() => setCurrentPage(totalPages)}
                        disabled={currentPage === totalPages || allPatients.length === 0}
                      >
                        <ChevronRight className="w-4 h-4" />
                        <ChevronRight className="w-4 h-4 -ml-1" />
                      </Button>
                    </div>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
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
            <Dialog>
              <DialogTrigger asChild>
                <Button variant="ghost" size="sm" className="text-pink-600 p-0 h-auto">
                  ver mais
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-4xl max-h-[80vh]">
                <DialogHeader>
                  <DialogTitle>Ociosidade por sala</DialogTitle>
                </DialogHeader>
                
                <div className="space-y-4">
                  {/* Estado vazio */}
                  <div className="flex flex-col items-center justify-center py-12">
                    <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center mb-4">
                      <AlertCircle className="w-6 h-6 text-purple-500" />
                    </div>
                    <h3 className="text-lg font-medium text-gray-900 mb-2">Hmm, está vazio por aqui!</h3>
                    <p className="text-gray-500 text-center">Nenhum registro encontrado.</p>
                  </div>
                  
                  {/* Controles de paginação */}
                  <div className="flex items-center justify-between pt-4 border-t">
                    <div className="flex items-center gap-2">
                      <Select value="25">
                        <SelectTrigger className="w-40">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="10">10 por página</SelectItem>
                          <SelectItem value="25">25 por página</SelectItem>
                          <SelectItem value="50">50 por página</SelectItem>
                          <SelectItem value="100">100 por página</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    
                    <div className="flex items-center gap-2">
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronLeft className="w-4 h-4" />
                        <ChevronLeft className="w-4 h-4 -ml-1" />
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronLeft className="w-4 h-4" />
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronRight className="w-4 h-4" />
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronRight className="w-4 h-4" />
                        <ChevronRight className="w-4 h-4 -ml-1" />
                      </Button>
                    </div>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
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
            <Dialog>
              <DialogTrigger asChild>
                <Button variant="ghost" size="sm" className="text-yellow-600 p-0 h-auto">
                  ver mais
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-4xl max-h-[80vh]">
                <DialogHeader>
                  <DialogTitle>Ociosidade por profissional</DialogTitle>
                </DialogHeader>
                
                <div className="space-y-4">
                  {/* Lista de profissionais */}
                  <div className="space-y-3 max-h-96 overflow-y-auto">
                    <div className="flex items-center justify-between py-2 px-4 border-b">
                      <div className="flex items-center gap-3">
                        <div className="w-6 h-6 bg-yellow-400 rounded-full flex items-center justify-center">
                          <span className="text-xs font-bold text-white">1</span>
                        </div>
                        <span className="text-sm">FERNANDO FERREIRA</span>
                      </div>
                      <div className="flex items-center gap-8">
                        <span className="text-sm font-medium">1</span>
                        <span className="text-sm text-gray-500 w-12 text-right">98%</span>
                      </div>
                    </div>
                  </div>
                  
                  {/* Controles de paginação */}
                  <div className="flex items-center justify-between pt-4 border-t">
                    <div className="flex items-center gap-2">
                      <Select value="25">
                        <SelectTrigger className="w-40">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="10">10 por página</SelectItem>
                          <SelectItem value="25">25 por página</SelectItem>
                          <SelectItem value="50">50 por página</SelectItem>
                          <SelectItem value="100">100 por página</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    
                    <div className="flex items-center gap-2">
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronLeft className="w-4 h-4" />
                        <ChevronLeft className="w-4 h-4 -ml-1" />
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronLeft className="w-4 h-4" />
                      </Button>
                      <Button variant="default" size="sm" className="bg-purple-600 text-white">
                        1
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronRight className="w-4 h-4" />
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronRight className="w-4 h-4" />
                        <ChevronRight className="w-4 h-4 -ml-1" />
                      </Button>
                    </div>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
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
            <Dialog>
              <DialogTrigger asChild>
                <Button variant="ghost" size="sm" className="text-purple-600 p-0 h-auto">
                  ver mais
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-4xl max-h-[80vh]">
                <DialogHeader>
                  <DialogTitle>Procedimentos mais frequentes</DialogTitle>
                </DialogHeader>
                
                <div className="space-y-4">
                  {/* Lista de procedimentos */}
                  <div className="space-y-3 max-h-96 overflow-y-auto">
                    <div className="flex items-center justify-between py-2 px-4 border-b">
                      <div className="flex items-center gap-3">
                        <div className="w-6 h-6 bg-yellow-400 rounded-full flex items-center justify-center">
                          <span className="text-xs font-bold text-white">1</span>
                        </div>
                        <span className="text-sm">Clareamento a Laser</span>
                      </div>
                      <div className="flex items-center gap-8">
                        <span className="text-sm font-medium">1</span>
                        <span className="text-sm text-gray-500 w-12 text-right">100%</span>
                      </div>
                    </div>
                  </div>
                  
                  {/* Controles de paginação */}
                  <div className="flex items-center justify-between pt-4 border-t">
                    <div className="flex items-center gap-2">
                      <Select value="25">
                        <SelectTrigger className="w-40">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="10">10 por página</SelectItem>
                          <SelectItem value="25">25 por página</SelectItem>
                          <SelectItem value="50">50 por página</SelectItem>
                          <SelectItem value="100">100 por página</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    
                    <div className="flex items-center gap-2">
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronLeft className="w-4 h-4" />
                        <ChevronLeft className="w-4 h-4 -ml-1" />
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronLeft className="w-4 h-4" />
                      </Button>
                      <Button variant="default" size="sm" className="bg-purple-600 text-white">
                        1
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronRight className="w-4 h-4" />
                      </Button>
                      <Button variant="ghost" size="sm" disabled>
                        <ChevronRight className="w-4 h-4" />
                        <ChevronRight className="w-4 h-4 -ml-1" />
                      </Button>
                    </div>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
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
            
            {/* Cabeçalho dos dias da semana */}
            <div className="flex items-center gap-3 mb-2">
              <div className="w-8"></div> {/* Espaço para os horários */}
              <div className="flex-1"></div> {/* Espaço para as barras */}
              <div className="flex gap-1">
                {["D", "S", "T", "Q", "Q", "S", "S"].map((day, dayIndex) => (
                  <div key={dayIndex} className="w-4 text-xs text-center text-muted-foreground font-medium">
                    {day}
                  </div>
                ))}
              </div>
            </div>
            
            <div className="space-y-1">
              {Array.from({ length: 24 }, (_, i) => {
                const isActive = i >= 8 && i <= 17; // Horário comercial mais movimentado
                const intensity = isActive ? Math.random() * 0.8 + 0.2 : Math.random() * 0.2; // Intensidade da barra
                
                return (
                  <div key={i} className="flex items-center gap-3">
                    {/* Horário */}
                    <div className="w-8 text-xs text-muted-foreground text-right">
                      {i.toString().padStart(2, '0')}h
                    </div>
                    
                    {/* Barra horizontal */}
                    <div className="flex-1 bg-gray-100 h-3 rounded">
                      <div 
                        className={`h-full rounded ${isActive ? 'bg-purple-500' : 'bg-gray-300'}`}
                        style={{ width: `${intensity * 100}%` }}
                      ></div>
                    </div>
                    
                    {/* Dias da semana abreviados */}
                    <div className="flex gap-1">
                      {["D", "S", "T", "Q", "Q", "S", "S"].map((day, dayIndex) => (
                        <div 
                          key={dayIndex} 
                          className={`w-4 h-3 rounded-sm text-xs flex items-center justify-center ${
                            isActive && dayIndex >= 1 && dayIndex <= 5 
                              ? 'bg-purple-500 text-white' 
                              : 'bg-gray-200 text-gray-500'
                          }`}
                        >
                          {/* Pequenos pontos para simular atividade */}
                        </div>
                      ))}
                    </div>
                  </div>
                );
              })}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function RelatorioAgendamentos() {
  const [selectedPeriod, setSelectedPeriod] = useState("custom");
  const [showFilters, setShowFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(25);
  const [selectedStatus, setSelectedStatus] = useState("todos");

  // Dados mock para relatório
  const reportData = [
    {
      id: 1,
      procedimento: "Clareamento a Laser",
      paciente: { nome: "Clara Ribeiro", avatar: "CR" },
      profissional: { nome: "FERNANDO", avatar: "F" },
      duracao: "60 min",
      agendadoPara: "22/05/2025 16:36",
      status: "concluido"
    }
  ];

  const statusCounts = {
    agendado: 0,
    confirmado: 0,
    naoCompareceu: 0,
    concluido: 1,
    cancelado: 0,
    todos: 1
  };

  const totalPages = Math.ceil(reportData.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentData = reportData.slice(startIndex, startIndex + itemsPerPage);

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <h2 className="text-2xl font-bold">Relatório de agendamentos</h2>
          <span className="px-2 py-1 bg-gray-100 rounded text-sm text-gray-600">Treglatro</span>
        </div>
        <div className="flex gap-2">
          <Select defaultValue="acoes">
            <SelectTrigger className="w-40">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="acoes">Ações em lote</SelectItem>
              <SelectItem value="excluir">Excluir selecionados</SelectItem>
              <SelectItem value="exportar">Exportar selecionados</SelectItem>
            </SelectContent>
          </Select>
          <Select defaultValue="exportar">
            <SelectTrigger className="w-32">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="exportar">Exportar</SelectItem>
              <SelectItem value="pdf">PDF</SelectItem>
              <SelectItem value="excel">Excel</SelectItem>
              <SelectItem value="csv">CSV</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Filtros */}
      <div className="space-y-4">
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2 px-3 py-2 bg-gray-100 rounded">
            <span className="text-sm">Período:</span>
            <span className="text-sm font-medium">18/05/2025 - 24/05/2025</span>
          </div>
          <Button 
            variant="ghost" 
            size="sm" 
            className="text-purple-600"
            onClick={() => setShowFilters(!showFilters)}
          >
            + Adicionar filtro
          </Button>
        </div>

        {/* Painel de filtros avançados */}
        {showFilters && (
          <Card className="p-4">
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
              <Button variant="default" className="bg-purple-600 text-white justify-start">
                📅 Período
              </Button>
              <Button variant="outline" className="justify-start">
                🔄 Status
              </Button>
              <Button variant="outline" className="justify-start">
                👤 Profissionais
              </Button>
              <Button variant="outline" className="justify-start">
                👥 Pacientes
              </Button>
              <Button variant="outline" className="justify-start">
                ❤️ Convênio
              </Button>
              <Button variant="outline" className="justify-start">
                ⚙️ Mais
              </Button>
            </div>
          </Card>
        )}

        {/* Contadores de status */}
        <div className="flex gap-6">
          {[
            { key: "agendado", label: "Agendado", color: "purple", count: statusCounts.agendado },
            { key: "confirmado", label: "Confirmado", color: "blue", count: statusCounts.confirmado },
            { key: "naoCompareceu", label: "Não compareceu", color: "gray", count: statusCounts.naoCompareceu },
            { key: "concluido", label: "Concluído", color: "green", count: statusCounts.concluido },
            { key: "cancelado", label: "Cancelado", color: "red", count: statusCounts.cancelado },
            { key: "todos", label: "Todos", color: "blue", count: statusCounts.todos }
          ].map((status) => (
            <button
              key={status.key}
              onClick={() => setSelectedStatus(status.key)}
              className={`flex flex-col items-center gap-1 pb-2 border-b-2 transition-colors ${
                selectedStatus === status.key
                  ? `border-${status.color}-500 text-${status.color}-600`
                  : "border-transparent text-gray-500 hover:text-gray-700"
              }`}
            >
              <div className="flex items-center gap-2">
                <div className={`w-3 h-3 rounded-full bg-${status.color}-500`}></div>
                <span className="text-sm font-medium">{status.label}</span>
              </div>
              <span className="text-lg font-bold">{status.count}</span>
            </button>
          ))}
        </div>
      </div>

      {/* Tabela */}
      <Card>
        <CardContent className="p-0">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="border-b">
                <tr>
                  <th className="text-left p-4">
                    <input type="checkbox" className="rounded" />
                  </th>
                  <th className="text-left p-4">Procedimentos</th>
                  <th className="text-left p-4">Paciente ↕</th>
                  <th className="text-left p-4">Profissional ↕</th>
                  <th className="text-left p-4">Duração ↕</th>
                  <th className="text-left p-4">Agendado para ↕</th>
                  <th className="text-left p-4">Status ↕</th>
                  <th className="text-left p-4">⚙️</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map((item) => (
                  <tr key={item.id} className="border-b hover:bg-gray-50">
                    <td className="p-4">
                      <input type="checkbox" className="rounded" />
                    </td>
                    <td className="p-4">{item.procedimento}</td>
                    <td className="p-4">
                      <div className="flex items-center gap-2">
                        <div className="w-8 h-8 bg-purple-100 rounded-full flex items-center justify-center">
                          <span className="text-xs font-bold text-purple-600">{item.paciente.avatar}</span>
                        </div>
                        <span>{item.paciente.nome}...</span>
                      </div>
                    </td>
                    <td className="p-4">
                      <div className="flex items-center gap-2">
                        <div className="w-8 h-8 bg-gray-100 rounded-full flex items-center justify-center">
                          <span className="text-xs font-bold">{item.profissional.avatar}</span>
                        </div>
                        <span>{item.profissional.nome}...</span>
                      </div>
                    </td>
                    <td className="p-4">{item.duracao}</td>
                    <td className="p-4">{item.agendadoPara}</td>
                    <td className="p-4">
                      <span className="px-2 py-1 bg-green-100 text-green-800 rounded text-xs">
                        ✓ Concluído
                      </span>
                    </td>
                    <td className="p-4">
                      <Button variant="ghost" size="sm">⋮</Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Paginação */}
          <div className="flex items-center justify-between p-4 border-t">
            <div className="flex items-center gap-2">
              <Select value={itemsPerPage.toString()} onValueChange={(value) => {
                setItemsPerPage(parseInt(value));
                setCurrentPage(1);
              }}>
                <SelectTrigger className="w-40">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="10">10 por página</SelectItem>
                  <SelectItem value="25">25 por página</SelectItem>
                  <SelectItem value="50">50 por página</SelectItem>
                  <SelectItem value="100">100 por página</SelectItem>
                </SelectContent>
              </Select>
            </div>
            
            <div className="flex items-center gap-2">
              <Button 
                variant="ghost" 
                size="sm"
                onClick={() => setCurrentPage(1)}
                disabled={currentPage === 1}
              >
                <ChevronLeft className="w-4 h-4" />
                <ChevronLeft className="w-4 h-4 -ml-1" />
              </Button>
              <Button 
                variant="ghost" 
                size="sm"
                onClick={() => setCurrentPage(currentPage - 1)}
                disabled={currentPage === 1}
              >
                <ChevronLeft className="w-4 h-4" />
              </Button>
              
              <Button variant="default" size="sm" className="bg-purple-600 text-white">
                1
              </Button>
              
              <Button 
                variant="ghost" 
                size="sm"
                onClick={() => setCurrentPage(currentPage + 1)}
                disabled={currentPage === totalPages}
              >
                <ChevronRight className="w-4 h-4" />
              </Button>
              <Button 
                variant="ghost" 
                size="sm"
                onClick={() => setCurrentPage(totalPages)}
                disabled={currentPage === totalPages}
              >
                <ChevronRight className="w-4 h-4" />
                <ChevronRight className="w-4 h-4 -ml-1" />
              </Button>
            </div>
          </div>
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