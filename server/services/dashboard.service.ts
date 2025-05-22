import { IStorage } from "../storage";

export interface DashboardMetrics {
  totalClients: number;
  totalAppointments: number;
  totalRevenue: number;
  monthlyGrowth: number;
  appointmentsToday: number;
  pendingPayments: number;
  activeStaff: number;
  popularServices: Array<{
    id: number;
    name: string;
    bookings: number;
    revenue: number;
  }>;
  recentAppointments: Array<{
    id: number;
    clientName: string;
    serviceName: string;
    dateTime: Date;
    status: string;
  }>;
  monthlyRevenue: Array<{
    month: string;
    revenue: number;
  }>;
  appointmentsByStatus: Array<{
    status: string;
    count: number;
    percentage: number;
  }>;
}

export class DashboardService {
  constructor(private storage: IStorage) {}

  async getDashboardMetrics(): Promise<DashboardMetrics> {
    try {
      // Buscar dados reais do sistema
      const [clients, appointments, services, staff, transactions] = await Promise.all([
        this.storage.getClients(),
        this.storage.getAppointments(),
        this.storage.getServices(),
        this.storage.getStaffMembers(),
        this.storage.getFinancialTransactions()
      ]);

      // Calcular métricas reais
      const totalClients = clients.length;
      const totalAppointments = appointments.length;
      const totalRevenue = transactions
        .filter(t => t.type === 'income')
        .reduce((sum, t) => sum + t.amount, 0);

      // Agendamentos de hoje (dados reais)
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      const tomorrow = new Date(today);
      tomorrow.setDate(tomorrow.getDate() + 1);
      
      const appointmentsToday = appointments.filter(apt => {
        const aptDate = new Date(apt.startTime);
        return aptDate >= today && aptDate < tomorrow;
      }).length;

      // Pagamentos pendentes (dados reais)
      const pendingPayments = transactions
        .filter(t => t.status === 'pending')
        .reduce((sum, t) => sum + t.amount, 0);

      // Staff ativo (dados reais)
      const activeStaff = staff.filter(s => s.available).length;

      // Serviços populares baseados em dados reais
      const thirtyDaysAgo = new Date();
      thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
      
      const recentAppointments = appointments.filter(apt => 
        new Date(apt.startTime) >= thirtyDaysAgo
      );

      const serviceStats = services.map(service => {
        const serviceAppointments = recentAppointments.filter(apt => apt.serviceId === service.id);
        const revenue = serviceAppointments.length * (service.price || 0);
        
        return {
          id: service.id,
          name: service.name,
          bookings: serviceAppointments.length,
          revenue
        };
      }).sort((a, b) => b.bookings - a.bookings).slice(0, 5);

      // Lista de agendamentos recentes (dados reais)
      const recentAppointmentsList = appointments
        .sort((a, b) => new Date(b.startTime).getTime() - new Date(a.startTime).getTime())
        .slice(0, 10)
        .map(apt => {
          const client = clients.find(c => c.id === apt.clientId);
          const service = services.find(s => s.id === apt.serviceId);
          
          return {
            id: apt.id,
            clientName: client?.fullName || 'Cliente não encontrado',
            serviceName: service?.name || 'Serviço não encontrado',
            dateTime: apt.startTime,
            status: apt.status
          };
        });

      // Receita mensal baseada em dados reais
      const monthlyRevenue = this.calculateMonthlyRevenue(transactions);

      // Distribuição por status baseada em dados reais
      const appointmentsByStatus = this.calculateAppointmentsByStatus(appointments);

      // Crescimento mensal calculado com dados reais
      const monthlyGrowth = this.calculateMonthlyGrowth(transactions);

      return {
        totalClients,
        totalAppointments,
        totalRevenue,
        monthlyGrowth,
        appointmentsToday,
        pendingPayments,
        activeStaff,
        popularServices: serviceStats,
        recentAppointments: recentAppointmentsList,
        monthlyRevenue,
        appointmentsByStatus
      };

    } catch (error) {
      console.error('Erro ao calcular métricas do dashboard:', error);
      throw new Error('Falha ao calcular métricas do dashboard');
    }
  }

  private calculateMonthlyRevenue(transactions: any[]): Array<{ month: string; revenue: number }> {
    const monthlyData: { [key: string]: number } = {};
    
    transactions
      .filter(t => t.type === 'income')
      .forEach(transaction => {
        const date = new Date(transaction.createdAt || Date.now());
        const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
        
        monthlyData[monthKey] = (monthlyData[monthKey] || 0) + transaction.amount;
      });

    // Últimos 12 meses
    const result = [];
    const now = new Date();
    
    for (let i = 11; i >= 0; i--) {
      const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      const monthName = date.toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' });
      
      result.push({
        month: monthName,
        revenue: monthlyData[monthKey] || 0
      });
    }

    return result;
  }

  private calculateAppointmentsByStatus(appointments: any[]): Array<{ status: string; count: number; percentage: number }> {
    const statusCount: { [key: string]: number } = {};
    
    appointments.forEach(apt => {
      statusCount[apt.status] = (statusCount[apt.status] || 0) + 1;
    });

    const total = appointments.length;
    
    return Object.entries(statusCount).map(([status, count]) => ({
      status,
      count,
      percentage: total > 0 ? Math.round((count / total) * 100) : 0
    }));
  }

  private calculateMonthlyGrowth(transactions: any[]): number {
    const now = new Date();
    const currentMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1);
    
    const currentMonthRevenue = transactions
      .filter(t => t.type === 'income' && new Date(t.createdAt || 0) >= currentMonth)
      .reduce((sum, t) => sum + t.amount, 0);
    
    const lastMonthRevenue = transactions
      .filter(t => t.type === 'income' && 
               new Date(t.createdAt || 0) >= lastMonth && 
               new Date(t.createdAt || 0) < currentMonth)
      .reduce((sum, t) => sum + t.amount, 0);
    
    if (lastMonthRevenue === 0) return 0;
    return ((currentMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100;
  }
}