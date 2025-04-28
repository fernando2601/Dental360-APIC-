import { useQuery } from "@tanstack/react-query";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { format, addMonths, subMonths } from "date-fns";
import { ptBR } from "date-fns/locale";
import { BarChart, Bar, LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell } from "recharts";
import { formatCurrency } from "@/lib/utils";
import { User, BarChart3, Calendar, TrendingUp, Users, CircleDollarSign } from "lucide-react";

// Cores para os gráficos
const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#8884D8", "#FF6B6B"];

// Componente de análise de conversão do ChatBot
export function ChatbotConversionAnalytics() {
  // Dados simulados para demonstração
  const chatbotData = [
    { month: "Jan", totalConversations: 120, bookings: 28, conversionRate: 23.3 },
    { month: "Fev", totalConversations: 150, bookings: 42, conversionRate: 28.0 },
    { month: "Mar", totalConversations: 180, bookings: 58, conversionRate: 32.2 },
    { month: "Abr", totalConversations: 210, bookings: 73, conversionRate: 34.8 },
    { month: "Mai", totalConversations: 250, bookings: 95, conversionRate: 38.0 },
    { month: "Jun", totalConversations: 230, bookings: 93, conversionRate: 40.4 },
  ];

  const suggestionData = [
    { name: "Extração de Siso", value: 42, count: 26 },
    { name: "Clareamento Dental", value: 28, count: 18 },
    { name: "Aplicação de Botox", value: 22, count: 14 },
    { name: "Consulta Geral", value: 8, count: 5 },
  ];

  const procedureData = [
    { name: "Procedimentos Dentais", value: 65 },
    { name: "Harmonização Facial", value: 35 },
  ];

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BarChart3 className="w-5 h-5 text-primary" />
            Análise de Conversão do ChatBot
          </CardTitle>
          <CardDescription>
            Dados de conversão de interações do ChatBot em agendamentos
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <User className="w-4 h-4 text-blue-500" />
                Total de Conversas
              </h3>
              <p className="text-2xl font-bold">1,140</p>
              <p className="text-xs text-muted-foreground">Últimos 6 meses</p>
            </div>
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <Calendar className="w-4 h-4 text-green-500" />
                Agendamentos
              </h3>
              <p className="text-2xl font-bold">389</p>
              <p className="text-xs text-muted-foreground">Via ChatBot (6 meses)</p>
            </div>
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <TrendingUp className="w-4 h-4 text-purple-500" />
                Taxa de Conversão
              </h3>
              <p className="text-2xl font-bold">34.1%</p>
              <p className="text-xs text-muted-foreground">Média dos últimos 6 meses</p>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Evolução da Taxa de Conversão</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart
                    data={chatbotData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" stroke="#444" opacity={0.1} />
                    <XAxis dataKey="month" />
                    <YAxis unit="%" />
                    <Tooltip 
                      formatter={(value: any) => [`${value}%`, 'Taxa de Conversão']}
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                    <Legend />
                    <Line 
                      type="monotone" 
                      dataKey="conversionRate" 
                      name="Taxa de Conversão"
                      stroke="#8884d8" 
                      strokeWidth={2}
                      activeDot={{ r: 8 }} 
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Interações vs. Agendamentos</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart
                    data={chatbotData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" stroke="#444" opacity={0.1} />
                    <XAxis dataKey="month" />
                    <YAxis />
                    <Tooltip 
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                    <Legend />
                    <Bar dataKey="totalConversations" name="Total de Conversas" fill="#0088FE" />
                    <Bar dataKey="bookings" name="Agendamentos" fill="#00C49F" />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Sugestões que Geram Mais Agendamentos</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={suggestionData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {suggestionData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip 
                      formatter={(value, name, props) => [
                        `${value}%`,
                        `${props.payload.name} (${props.payload.count} agendamentos)`
                      ]}
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Tipos de Procedimentos Agendados</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={procedureData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {procedureData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip 
                      formatter={(value) => [`${value}%`, 'Percentual']}
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

// Componente de análise de sazonalidade e tendências
export function SeasonalityTrendsAnalytics() {
  // Dados de sazonalidade ao longo do ano
  const monthlyData = [
    { name: "Jan", dental: 35000, harmonization: 25000, total: 60000 },
    { name: "Fev", dental: 32000, harmonization: 28000, total: 60000 },
    { name: "Mar", dental: 40000, harmonization: 30000, total: 70000 },
    { name: "Abr", dental: 38000, harmonization: 32000, total: 70000 },
    { name: "Mai", dental: 42000, harmonization: 38000, total: 80000 },
    { name: "Jun", dental: 48000, harmonization: 42000, total: 90000 },
    { name: "Jul", dental: 52000, harmonization: 48000, total: 100000 },
    { name: "Ago", dental: 58000, harmonization: 52000, total: 110000 },
    { name: "Set", dental: 62000, harmonization: 58000, total: 120000 },
    { name: "Out", dental: 55000, harmonization: 55000, total: 110000 },
    { name: "Nov", dental: 48000, harmonization: 52000, total: 100000 },
    { name: "Dez", dental: 45000, harmonization: 55000, total: 100000 },
  ];
  
  // Dados para comparação anual
  const yearlyComparisonData = [
    { name: "2023", dental: 450000, harmonization: 380000, total: 830000 },
    { name: "2024", dental: 520000, harmonization: 465000, total: 985000 },
    { name: "2025", dental: 605000, harmonization: 565000, total: 1170000 }, // Projeção
  ];

  // Dados de previsão de demanda por procedimento
  const forecastData = [
    { procedimento: "Botox Facial", tendencia: "alta", crescimento: 28, demanda: "muito alta" },
    { procedimento: "Clareamento Dental", tendencia: "estável", crescimento: 12, demanda: "alta" },
    { procedimento: "Implantes Dentais", tendencia: "alta", crescimento: 22, demanda: "muito alta" },
    { procedimento: "Preenchimento Labial", tendencia: "alta", crescimento: 18, demanda: "alta" },
    { procedimento: "Aparelho Ortodôntico", tendencia: "estável", crescimento: 8, demanda: "média" },
  ];

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <TrendingUp className="w-5 h-5 text-primary" />
            Análise de Sazonalidade e Tendências
          </CardTitle>
          <CardDescription>
            Padrões sazonais, crescimento anual e previsões de demanda
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <Calendar className="w-4 h-4 text-blue-500" />
                Período de Alta Demanda
              </h3>
              <p className="text-2xl font-bold">Set-Out</p>
              <p className="text-xs text-muted-foreground">Meses com maior faturamento</p>
            </div>
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <TrendingUp className="w-4 h-4 text-green-500" />
                Crescimento Anual
              </h3>
              <p className="text-2xl font-bold">18.7%</p>
              <p className="text-xs text-muted-foreground">Comparado ao ano anterior</p>
            </div>
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <CircleDollarSign className="w-4 h-4 text-purple-500" />
                Previsão para 2025
              </h3>
              <p className="text-2xl font-bold">{formatCurrency(1170000)}</p>
              <p className="text-xs text-muted-foreground">Faturamento anual projetado</p>
            </div>
          </div>

          <div className="bg-muted/30 p-4 rounded-lg mb-6">
            <h3 className="text-sm font-medium mb-4">Receita Mensal por Categoria (Último Ano)</h3>
            <div className="h-72">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart
                  data={monthlyData}
                  margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                >
                  <CartesianGrid strokeDasharray="3 3" stroke="#444" opacity={0.1} />
                  <XAxis dataKey="name" />
                  <YAxis tickFormatter={(value) => `R$${value / 1000}k`} />
                  <Tooltip 
                    formatter={(value) => [`R$ ${value.toLocaleString('pt-BR')}`, '']}
                    contentStyle={{ 
                      backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                      border: 'none',
                      borderRadius: '4px',
                      color: '#fff'
                    }}
                  />
                  <Legend />
                  <Bar dataKey="dental" name="Procedimentos Dentais" stackId="a" fill="#0088FE" />
                  <Bar dataKey="harmonization" name="Harmonização Facial" stackId="a" fill="#00C49F" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Comparação de Faturamento Anual</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart
                    data={yearlyComparisonData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" stroke="#444" opacity={0.1} />
                    <XAxis dataKey="name" />
                    <YAxis tickFormatter={(value) => `R$${value / 1000}k`} />
                    <Tooltip 
                      formatter={(value) => [`R$ ${value.toLocaleString('pt-BR')}`, '']}
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                    <Legend />
                    <Bar dataKey="dental" name="Procedimentos Dentais" fill="#0088FE" />
                    <Bar dataKey="harmonization" name="Harmonização Facial" fill="#00C49F" />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-3">Previsão de Demanda por Procedimento</h3>
              <div className="overflow-hidden">
                <table className="min-w-full text-sm">
                  <thead>
                    <tr className="border-b border-muted-foreground/20">
                      <th className="px-4 py-2 text-left font-medium">Procedimento</th>
                      <th className="px-4 py-2 text-left font-medium">Tendência</th>
                      <th className="px-4 py-2 text-left font-medium">Crescimento</th>
                      <th className="px-4 py-2 text-left font-medium">Demanda</th>
                    </tr>
                  </thead>
                  <tbody>
                    {forecastData.map((item, index) => (
                      <tr key={index} className={index !== forecastData.length - 1 ? "border-b border-muted-foreground/10" : ""}>
                        <td className="px-4 py-3">{item.procedimento}</td>
                        <td className="px-4 py-3">
                          <span className={`px-2 py-1 rounded-full text-xs ${
                            item.tendencia === "alta" 
                              ? "bg-green-500/20 text-green-500" 
                              : item.tendencia === "estável"
                              ? "bg-blue-500/20 text-blue-500"
                              : "bg-red-500/20 text-red-500"
                          }`}>
                            {item.tendencia}
                          </span>
                        </td>
                        <td className="px-4 py-3">{item.crescimento}%</td>
                        <td className="px-4 py-3">
                          <span className={`px-2 py-1 rounded-full text-xs ${
                            item.demanda === "muito alta" 
                              ? "bg-purple-500/20 text-purple-500" 
                              : item.demanda === "alta"
                              ? "bg-blue-500/20 text-blue-500"
                              : "bg-yellow-500/20 text-yellow-500"
                          }`}>
                            {item.demanda}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

// Componente de análise de lealdade de clientes
export function ClientLoyaltyAnalytics() {
  // Dados para segmentação de clientes
  const clientSegmentData = [
    { name: "Primeira Visita", value: 30 },
    { name: "Ocasionais", value: 25 },
    { name: "Regulares", value: 30 },
    { name: "Frequentes", value: 15 },
  ];

  // Dados de renovação de tratamentos
  const renewalData = [
    { month: "Jan", rate: 45 },
    { month: "Fev", rate: 48 },
    { month: "Mar", rate: 52 },
    { month: "Abr", rate: 55 },
    { month: "Mai", rate: 62 },
    { month: "Jun", rate: 68 },
  ];

  // Dados de valor do cliente ao longo do tempo
  const clientValueData = [
    { month: 1, valor: 450 },
    { month: 3, valor: 850 },
    { month: 6, valor: 1200 },
    { month: 12, valor: 2400 },
    { month: 24, valor: 4800 },
  ];

  // Dados para oportunidades de up-selling
  const upsellData = [
    { 
      cliente: "Clareamento", 
      oportunidade: "Facetas de Porcelana", 
      chance: "alta", 
      valorEstimado: 3800 
    },
    { 
      cliente: "Botox", 
      oportunidade: "Preenchimento Labial", 
      chance: "média", 
      valorEstimado: 1200 
    },
    { 
      cliente: "Limpeza Dental", 
      oportunidade: "Clareamento", 
      chance: "alta", 
      valorEstimado: 950 
    },
    { 
      cliente: "Extração Dental", 
      oportunidade: "Implante", 
      chance: "média", 
      valorEstimado: 4500 
    },
    { 
      cliente: "Consulta Rotina", 
      oportunidade: "Lentes de Contato Dental", 
      chance: "baixa", 
      valorEstimado: 6200 
    },
  ];

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="w-5 h-5 text-primary" />
            Análise de Lealdade de Clientes
          </CardTitle>
          <CardDescription>
            Segmentação, retenção e oportunidades de aumento de valor
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <Users className="w-4 h-4 text-blue-500" />
                Taxa de Retenção
              </h3>
              <p className="text-2xl font-bold">67%</p>
              <p className="text-xs text-muted-foreground">Média dos últimos 6 meses</p>
            </div>
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <CircleDollarSign className="w-4 h-4 text-green-500" />
                Valor Médio por Cliente
              </h3>
              <p className="text-2xl font-bold">{formatCurrency(1850)}</p>
              <p className="text-xs text-muted-foreground">Anual</p>
            </div>
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-2 flex items-center gap-2">
                <TrendingUp className="w-4 h-4 text-purple-500" />
                Potencial de Up-selling
              </h3>
              <p className="text-2xl font-bold">{formatCurrency(450000)}</p>
              <p className="text-xs text-muted-foreground">Estimado para próximos 6 meses</p>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Segmentação de Clientes</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={clientSegmentData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {clientSegmentData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip 
                      formatter={(value) => [`${value}%`, 'Percentual']}
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Taxa de Renovação de Tratamentos</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart
                    data={renewalData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" stroke="#444" opacity={0.1} />
                    <XAxis dataKey="month" />
                    <YAxis unit="%" />
                    <Tooltip 
                      formatter={(value: any) => [`${value}%`, 'Taxa de Renovação']}
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                    <Legend />
                    <Line 
                      type="monotone" 
                      dataKey="rate" 
                      name="Taxa de Renovação"
                      stroke="#00C49F" 
                      strokeWidth={2}
                      activeDot={{ r: 8 }} 
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-4">Valor do Cliente ao Longo do Tempo</h3>
              <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart
                    data={clientValueData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" stroke="#444" opacity={0.1} />
                    <XAxis 
                      dataKey="month" 
                      label={{ 
                        value: 'Meses', 
                        position: 'insideBottomRight', 
                        offset: -10 
                      }} 
                    />
                    <YAxis 
                      tickFormatter={(value) => `R$${value}`}
                      label={{ 
                        value: 'Valor gasto', 
                        angle: -90, 
                        position: 'insideLeft'
                      }} 
                    />
                    <Tooltip 
                      formatter={(value) => [`R$ ${value.toLocaleString('pt-BR')}`, 'Valor Médio']}
                      labelFormatter={(value) => `Mês ${value}`}
                      contentStyle={{ 
                        backgroundColor: 'rgba(22, 22, 22, 0.8)', 
                        border: 'none',
                        borderRadius: '4px',
                        color: '#fff'
                      }}
                    />
                    <Line 
                      type="monotone" 
                      dataKey="valor" 
                      name="Valor Médio"
                      stroke="#8884d8" 
                      strokeWidth={2}
                      activeDot={{ r: 8 }} 
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="bg-muted/30 p-4 rounded-lg">
              <h3 className="text-sm font-medium mb-3">Oportunidades de Up-selling</h3>
              <div className="overflow-hidden">
                <table className="min-w-full text-sm">
                  <thead>
                    <tr className="border-b border-muted-foreground/20">
                      <th className="px-4 py-2 text-left font-medium">Perfil</th>
                      <th className="px-4 py-2 text-left font-medium">Sugestão</th>
                      <th className="px-4 py-2 text-left font-medium">Chance</th>
                      <th className="px-4 py-2 text-left font-medium">Valor</th>
                    </tr>
                  </thead>
                  <tbody>
                    {upsellData.map((item, index) => (
                      <tr key={index} className={index !== upsellData.length - 1 ? "border-b border-muted-foreground/10" : ""}>
                        <td className="px-4 py-3">{item.cliente}</td>
                        <td className="px-4 py-3">{item.oportunidade}</td>
                        <td className="px-4 py-3">
                          <span className={`px-2 py-1 rounded-full text-xs ${
                            item.chance === "alta" 
                              ? "bg-green-500/20 text-green-500" 
                              : item.chance === "média"
                              ? "bg-yellow-500/20 text-yellow-500"
                              : "bg-red-500/20 text-red-500"
                          }`}>
                            {item.chance}
                          </span>
                        </td>
                        <td className="px-4 py-3">{formatCurrency(item.valorEstimado)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

// Componente principal que agrupa todas as análises
export default function AnalyticsDashboards() {
  return (
    <Tabs defaultValue="chatbot" className="w-full">
      <TabsList className="mb-6">
        <TabsTrigger value="chatbot">Conversão do ChatBot</TabsTrigger>
        <TabsTrigger value="seasonality">Sazonalidade e Tendências</TabsTrigger>
        <TabsTrigger value="loyalty">Lealdade de Clientes</TabsTrigger>
      </TabsList>
      
      <TabsContent value="chatbot">
        <ChatbotConversionAnalytics />
      </TabsContent>
      
      <TabsContent value="seasonality">
        <SeasonalityTrendsAnalytics />
      </TabsContent>
      
      <TabsContent value="loyalty">
        <ClientLoyaltyAnalytics />
      </TabsContent>
    </Tabs>
  );
}