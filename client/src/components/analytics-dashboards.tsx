import React, { useState } from 'react';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { BarChart, Bar, LineChart, Line, PieChart, Pie, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, Cell } from 'recharts';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { formatCurrency } from '@/lib/utils';
import { Badge } from '@/components/ui/badge';
import { UserRound, MessageSquare, Calendar, ArrowUpRight, TrendingUp, Users, Clock } from 'lucide-react';

const AnalyticsDashboards = () => {
  const [activeTab, setActiveTab] = useState('chatbot');

  // Dados para a análise de conversão do ChatBot
  const chatbotData = [
    { month: 'Jan', mensagens: 220, conversoes: 45, taxa: 20.5 },
    { month: 'Fev', mensagens: 240, conversoes: 52, taxa: 21.7 },
    { month: 'Mar', mensagens: 310, conversoes: 68, taxa: 21.9 },
    { month: 'Abr', mensagens: 280, conversoes: 70, taxa: 25.0 },
    { month: 'Mai', mensagens: 350, conversoes: 98, taxa: 28.0 },
    { month: 'Jun', mensagens: 390, conversoes: 120, taxa: 30.8 },
  ];

  // Dados para análise de sazonalidade
  const seasonalityData = [
    { month: 'Jan', limpeza: 28, clareamento: 12, botox: 8, harmonizacao: 5 },
    { month: 'Fev', limpeza: 32, clareamento: 18, botox: 11, harmonizacao: 7 },
    { month: 'Mar', limpeza: 35, clareamento: 23, botox: 15, harmonizacao: 9 },
    { month: 'Abr', limpeza: 30, clareamento: 28, botox: 14, harmonizacao: 11 },
    { month: 'Mai', limpeza: 24, clareamento: 32, botox: 10, harmonizacao: 15 },
    { month: 'Jun', limpeza: 20, clareamento: 25, botox: 7, harmonizacao: 18 },
  ];

  // Dados para análise de lealdade
  const loyaltyData = [
    { name: 'Novos', value: 35, color: '#94a3b8' },
    { name: 'Ocasionais', value: 45, color: '#3b82f6' },
    { name: 'Regulares', value: 15, color: '#10b981' },
    { name: 'Premium', value: 5, color: '#8b5cf6' },
  ];

  // Dados para tabela de frases mais efetivas
  const topPhrases = [
    { 
      phrase: "Promoção especial de clareamento este mês!", 
      uses: 85, 
      conversions: 32, 
      rate: "37.6%" 
    },
    { 
      phrase: "Agende hoje e ganhe avaliação gratuita de harmonização!", 
      uses: 64, 
      conversions: 23, 
      rate: "35.9%" 
    },
    { 
      phrase: "Primeira consulta com 50% de desconto!", 
      uses: 72, 
      conversions: 25, 
      rate: "34.7%" 
    },
    { 
      phrase: "Dor de dente? Temos horários de emergência!", 
      uses: 58, 
      conversions: 19, 
      rate: "32.8%" 
    },
    { 
      phrase: "Pacote completo de estética: Clareamento + Harmonização", 
      uses: 43, 
      conversions: 14, 
      rate: "32.6%" 
    },
  ];

  // Dados para tabela de clientes fiéis
  const loyalCustomers = [
    { name: "Maria Silva", visits: 12, revenue: 8450, lastVisit: "Há 2 semanas", status: "Premium" },
    { name: "João Oliveira", visits: 8, revenue: 5820, lastVisit: "Há 1 mês", status: "Regular" },
    { name: "Ana Santos", visits: 10, revenue: 7230, lastVisit: "Há 3 dias", status: "Premium" },
    { name: "Carlos Ferreira", visits: 7, revenue: 4320, lastVisit: "Há 2 meses", status: "Regular" },
    { name: "Beatriz Lima", visits: 14, revenue: 9540, lastVisit: "Há 1 semana", status: "Premium" },
  ];
  
  // Horários de pico para agendamentos
  const peakHours = [
    { hour: '8-10h', count: 15 },
    { hour: '10-12h', count: 28 },
    { hour: '12-14h', count: 12 },
    { hour: '14-16h', count: 18 },
    { hour: '16-18h', count: 32 },
    { hour: '18-20h', count: 25 },
    { hour: '20-22h', count: 8 },
  ];

  return (
    <div className="space-y-6">
      <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
        <TabsList className="grid w-full grid-cols-3 mb-8">
          <TabsTrigger value="chatbot">
            <MessageSquare className="h-4 w-4 mr-2" />
            Análise do ChatBot
          </TabsTrigger>
          <TabsTrigger value="seasonality">
            <TrendingUp className="h-4 w-4 mr-2" />
            Sazonalidade e Tendências
          </TabsTrigger>
          <TabsTrigger value="loyalty">
            <Users className="h-4 w-4 mr-2" />
            Análise de Lealdade
          </TabsTrigger>
        </TabsList>

        {/* Análise do ChatBot */}
        <TabsContent value="chatbot">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Total de Conversas</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center">
                  <MessageSquare className="h-5 w-5 mr-2 text-primary" />
                  <div className="text-2xl font-bold">1,790</div>
                </div>
                <p className="text-xs text-muted-foreground mt-1">+15% em relação ao período anterior</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Conversões</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center">
                  <Calendar className="h-5 w-5 mr-2 text-primary" />
                  <div className="text-2xl font-bold">453</div>
                </div>
                <p className="text-xs text-muted-foreground mt-1">+23% em relação ao período anterior</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Taxa de Conversão</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center">
                  <ArrowUpRight className="h-5 w-5 mr-2 text-emerald-500" />
                  <div className="text-2xl font-bold">25.3%</div>
                </div>
                <p className="text-xs text-muted-foreground mt-1">+7% em relação ao período anterior</p>
              </CardContent>
            </Card>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-5 gap-6 mb-6">
            <Card className="lg:col-span-3">
              <CardHeader>
                <CardTitle>Evolução de Conversões do ChatBot</CardTitle>
                <CardDescription>Análise de mensagens e conversões mensais</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="h-80">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart
                      data={chatbotData}
                      margin={{ top: 20, right: 30, left: 0, bottom: 5 }}
                    >
                      <CartesianGrid strokeDasharray="3 3" opacity={0.2} />
                      <XAxis dataKey="month" />
                      <YAxis yAxisId="left" orientation="left" stroke="#10b981" />
                      <YAxis yAxisId="right" orientation="right" stroke="#3b82f6" />
                      <Tooltip />
                      <Legend />
                      <Bar yAxisId="left" dataKey="mensagens" name="Total de Mensagens" fill="#3b82f6" radius={[4, 4, 0, 0]} />
                      <Bar yAxisId="right" dataKey="conversoes" name="Conversões" fill="#10b981" radius={[4, 4, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </CardContent>
            </Card>

            <Card className="lg:col-span-2">
              <CardHeader>
                <CardTitle>Top Frases de Conversão</CardTitle>
                <CardDescription>Frases que mais convertem clientes</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Frase</TableHead>
                        <TableHead className="text-right">Conv.</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {topPhrases.map((phrase, i) => (
                        <TableRow key={i}>
                          <TableCell className="font-medium truncate max-w-[180px]">
                            {phrase.phrase}
                          </TableCell>
                          <TableCell className="text-right">{phrase.rate}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
              <CardFooter className="border-t px-6 py-3">
                <p className="text-xs text-muted-foreground">
                  Baseado nas últimas 500 conversas do ChatBot
                </p>
              </CardFooter>
            </Card>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Análise de Taxa de Conversão</CardTitle>
              <CardDescription>Tendência de efetividade ao longo do tempo</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="h-72">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart
                    data={chatbotData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" opacity={0.2} />
                    <XAxis dataKey="month" />
                    <YAxis domain={[0, 40]} />
                    <Tooltip />
                    <Legend />
                    <Line 
                      type="monotone" 
                      dataKey="taxa" 
                      name="Taxa de Conversão (%)" 
                      stroke="#8b5cf6" 
                      strokeWidth={3}
                      dot={{ r: 6 }}
                      activeDot={{ r: 8 }}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Análise de Sazonalidade */}
        <TabsContent value="seasonality">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Melhor Mês</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">Maio</div>
                <p className="text-xs text-muted-foreground mt-1">Para serviços de Clareamento</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Serviço em Alta</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">Harmonização</div>
                <p className="text-xs text-muted-foreground mt-1">+35% de crescimento</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Pico de Horário</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">16h às 18h</div>
                <p className="text-xs text-muted-foreground mt-1">32 agendamentos em média</p>
              </CardContent>
            </Card>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
            <Card>
              <CardHeader>
                <CardTitle>Tendências Sazonais por Serviço</CardTitle>
                <CardDescription>Variação de procura ao longo do ano</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="h-80">
                  <ResponsiveContainer width="100%" height="100%">
                    <LineChart
                      data={seasonalityData}
                      margin={{ top: 20, right: 30, left: 0, bottom: 5 }}
                    >
                      <CartesianGrid strokeDasharray="3 3" opacity={0.2} />
                      <XAxis dataKey="month" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Line type="monotone" dataKey="limpeza" name="Limpeza Dental" stroke="#3b82f6" strokeWidth={2} />
                      <Line type="monotone" dataKey="clareamento" name="Clareamento" stroke="#10b981" strokeWidth={2} />
                      <Line type="monotone" dataKey="botox" name="Botox" stroke="#8b5cf6" strokeWidth={2} />
                      <Line type="monotone" dataKey="harmonizacao" name="Harmonização" stroke="#f97316" strokeWidth={2} />
                    </LineChart>
                  </ResponsiveContainer>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Horários de Pico</CardTitle>
                <CardDescription>Quando os pacientes mais agendam</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="h-80">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart
                      data={peakHours}
                      margin={{ top: 20, right: 10, left: 10, bottom: 5 }}
                    >
                      <CartesianGrid strokeDasharray="3 3" opacity={0.2} />
                      <XAxis dataKey="hour" />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="count" name="Agendamentos" fill="#8b5cf6" radius={[4, 4, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </CardContent>
              <CardFooter className="border-t px-6 py-3">
                <p className="text-xs text-muted-foreground flex items-center">
                  <Clock className="h-3.5 w-3.5 mr-1" />
                  Dados baseados nos últimos 90 dias
                </p>
              </CardFooter>
            </Card>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Previsões e Recomendações</CardTitle>
              <CardDescription>Estratégias baseadas em dados sazonais</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="p-4 bg-muted/30 rounded-lg">
                  <h4 className="font-medium mb-2">Próximos 3 meses</h4>
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div className="space-y-2">
                      <Badge variant="outline" className="bg-blue-50 dark:bg-blue-950 text-blue-700 dark:text-blue-300 border-blue-200 dark:border-blue-800">
                        Julho
                      </Badge>
                      <p className="text-sm">Aumento esperado de 22% em clareamentos.</p>
                      <p className="text-xs text-muted-foreground">Promover pacotes especiais de estética.</p>
                    </div>
                    <div className="space-y-2">
                      <Badge variant="outline" className="bg-green-50 dark:bg-green-950 text-green-700 dark:text-green-300 border-green-200 dark:border-green-800">
                        Agosto
                      </Badge>
                      <p className="text-sm">Crescimento de 18% em harmonização facial.</p>
                      <p className="text-xs text-muted-foreground">Webinar educativo sobre procedimentos estéticos.</p>
                    </div>
                    <div className="space-y-2">
                      <Badge variant="outline" className="bg-purple-50 dark:bg-purple-950 text-purple-700 dark:text-purple-300 border-purple-200 dark:border-purple-800">
                        Setembro
                      </Badge>
                      <p className="text-sm">Volta às aulas - aumento de 30% em limpeza.</p>
                      <p className="text-xs text-muted-foreground">Campanha focada em pais e estudantes.</p>
                    </div>
                  </div>
                </div>
                
                <div className="p-4 border rounded-lg">
                  <h4 className="font-medium mb-2">Otimização de Agenda</h4>
                  <p className="text-sm">Baseado nos horários de pico:</p>
                  <ul className="mt-2 space-y-1">
                    <li className="text-sm flex items-start gap-1.5">
                      <span className="text-primary">•</span>
                      <span>Adicionar mais profissionais entre 16h e 18h</span>
                    </li>
                    <li className="text-sm flex items-start gap-1.5">
                      <span className="text-primary">•</span>
                      <span>Oferecer descontos em horários menos procurados (8-10h e 20-22h)</span>
                    </li>
                    <li className="text-sm flex items-start gap-1.5">
                      <span className="text-primary">•</span>
                      <span>Reservar horários de 10-12h para procedimentos mais longos</span>
                    </li>
                  </ul>
                </div>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Análise de Lealdade */}
        <TabsContent value="loyalty">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Clientes Fiéis</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center">
                  <UserRound className="h-5 w-5 mr-2 text-primary" />
                  <div className="text-2xl font-bold">20%</div>
                </div>
                <p className="text-xs text-muted-foreground mt-1">Clientes Premium e Regulares</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Valor Médio</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">{formatCurrency(7080)}</div>
                <p className="text-xs text-muted-foreground mt-1">Gasto médio por cliente fiel</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-md font-medium">Taxa de Retenção</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center">
                  <ArrowUpRight className="h-5 w-5 mr-2 text-emerald-500" />
                  <div className="text-2xl font-bold">85%</div>
                </div>
                <p className="text-xs text-muted-foreground mt-1">Em clientes Premium</p>
              </CardContent>
            </Card>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-5 gap-6 mb-6">
            <Card className="lg:col-span-2">
              <CardHeader>
                <CardTitle>Segmentação de Clientes</CardTitle>
                <CardDescription>Distribuição por categoria de fidelidade</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="h-72 flex items-center justify-center">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={loyaltyData}
                        cx="50%"
                        cy="50%"
                        innerRadius={60}
                        outerRadius={110}
                        paddingAngle={2}
                        dataKey="value"
                        label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                        labelLine={false}
                      >
                        {loyaltyData.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={entry.color} />
                        ))}
                      </Pie>
                      <Tooltip formatter={(value) => [`${value}%`, 'Porcentagem']} />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </CardContent>
            </Card>

            <Card className="lg:col-span-3">
              <CardHeader>
                <CardTitle>Top Clientes Fiéis</CardTitle>
                <CardDescription>Clientes com maior valor e frequência</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Cliente</TableHead>
                        <TableHead className="text-right">Visitas</TableHead>
                        <TableHead className="text-right">Receita</TableHead>
                        <TableHead>Última Visita</TableHead>
                        <TableHead>Status</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {loyalCustomers.map((customer, i) => (
                        <TableRow key={i}>
                          <TableCell className="font-medium">{customer.name}</TableCell>
                          <TableCell className="text-right">{customer.visits}</TableCell>
                          <TableCell className="text-right">{formatCurrency(customer.revenue)}</TableCell>
                          <TableCell>{customer.lastVisit}</TableCell>
                          <TableCell>
                            <Badge variant={customer.status === "Premium" ? "default" : "outline"}>
                              {customer.status}
                            </Badge>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Estratégias de Fidelização</CardTitle>
              <CardDescription>Recomendações para aumentar a lealdade dos clientes</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="space-y-3">
                  <div className="flex items-center gap-2">
                    <div className="bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-300 p-2 rounded-full">
                      <UserRound className="h-5 w-5" />
                    </div>
                    <h3 className="font-medium">Clientes Ocasionais</h3>
                  </div>
                  <ul className="space-y-2 text-sm">
                    <li className="flex items-start gap-1.5">
                      <span className="text-blue-600 dark:text-blue-400 font-bold">•</span>
                      <span>Enviar lembretes personalizados para check-ups</span>
                    </li>
                    <li className="flex items-start gap-1.5">
                      <span className="text-blue-600 dark:text-blue-400 font-bold">•</span>
                      <span>Oferecer desconto de 10% na próxima visita</span>
                    </li>
                    <li className="flex items-start gap-1.5">
                      <span className="text-blue-600 dark:text-blue-400 font-bold">•</span>
                      <span>Compartilhar conteúdo educativo via email</span>
                    </li>
                  </ul>
                </div>
                
                <div className="space-y-3">
                  <div className="flex items-center gap-2">
                    <div className="bg-green-100 dark:bg-green-900 text-green-700 dark:text-green-300 p-2 rounded-full">
                      <UserRound className="h-5 w-5" />
                    </div>
                    <h3 className="font-medium">Clientes Regulares</h3>
                  </div>
                  <ul className="space-y-2 text-sm">
                    <li className="flex items-start gap-1.5">
                      <span className="text-green-600 dark:text-green-400 font-bold">•</span>
                      <span>Implementar programa de pontos por visita</span>
                    </li>
                    <li className="flex items-start gap-1.5">
                      <span className="text-green-600 dark:text-green-400 font-bold">•</span>
                      <span>Oferecer consulas preferenciais em horários de pico</span>
                    </li>
                    <li className="flex items-start gap-1.5">
                      <span className="text-green-600 dark:text-green-400 font-bold">•</span>
                      <span>Enviar brindes em datas comemorativas</span>
                    </li>
                  </ul>
                </div>
                
                <div className="space-y-3">
                  <div className="flex items-center gap-2">
                    <div className="bg-purple-100 dark:bg-purple-900 text-purple-700 dark:text-purple-300 p-2 rounded-full">
                      <UserRound className="h-5 w-5" />
                    </div>
                    <h3 className="font-medium">Clientes Premium</h3>
                  </div>
                  <ul className="space-y-2 text-sm">
                    <li className="flex items-start gap-1.5">
                      <span className="text-purple-600 dark:text-purple-400 font-bold">•</span>
                      <span>Acesso a lançamentos de novos tratamentos</span>
                    </li>
                    <li className="flex items-start gap-1.5">
                      <span className="text-purple-600 dark:text-purple-400 font-bold">•</span>
                      <span>Descontos exclusivos em pacotes de harmonização</span>
                    </li>
                    <li className="flex items-start gap-1.5">
                      <span className="text-purple-600 dark:text-purple-400 font-bold">•</span>
                      <span>Atendimento com acesso direto via WhatsApp</span>
                    </li>
                  </ul>
                </div>
              </div>
            </CardContent>
            <CardFooter className="border-t px-6 py-3">
              <p className="text-xs text-muted-foreground">
                As estratégias acima podem aumentar a taxa de retenção em até 35%
              </p>
            </CardFooter>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
};

export default AnalyticsDashboards;