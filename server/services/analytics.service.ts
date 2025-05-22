import { IStorage } from "../storage";

export interface AnalyticsData {
  clientGrowth: Array<{
    month: string;
    newClients: number;
    totalClients: number;
  }>;
  revenueAnalysis: Array<{
    month: string;
    revenue: number;
    appointments: number;
    averageTicket: number;
  }>;
  servicePerformance: Array<{
    id: number;
    name: string;
    category: string;
    bookings: number;
    revenue: number;
    averagePrice: number;
    popularityScore: number;
  }>;
  staffPerformance: Array<{
    id: number;
    name: string;
    specialization: string;
    appointmentsCompleted: number;
    revenue: number;
    averageRating: number;
    utilizationRate: number;
  }>;
  clientRetention: {
    returnRate: number;
    averageVisits: number;
    loyalClients: number;
    churnRate: number;
  };
  busyHours: Array<{
    hour: string;
    appointments: number;
    dayOfWeek: string;
  }>;
  cancellationAnalysis: {
    totalCancellations: number;
    cancellationRate: number;
    topReasons: Array<{
      reason: string;
      count: number;
    }>;
  };
}

export class AnalyticsService {
  constructor(private storage: IStorage) {}

  async getAnalyticsData(startDate?: string, endDate?: string): Promise<AnalyticsData> {
    try {
      const [clients, appointments, services, staff, transactions] = await Promise.all([
        this.storage.getClients(),
        this.storage.getAppointments(),
        this.storage.getServices(),
        this.storage.getStaffMembers(),
        this.storage.getFinancialTransactions()
      ]);

      // Filtrar por período se especificado
      let filteredAppointments = appointments;
      let filteredTransactions = transactions;

      if (startDate && endDate) {
        const start = new Date(startDate);
        const end = new Date(endDate);
        
        filteredAppointments = appointments.filter(apt => {
          const aptDate = new Date(apt.startTime);
          return aptDate >= start && aptDate <= end;
        });

        filteredTransactions = transactions.filter(trans => {
          const transDate = new Date(trans.createdAt || 0);
          return transDate >= start && transDate <= end;
        });
      }

      return {
        clientGrowth: this.calculateClientGrowth(clients),
        revenueAnalysis: this.calculateRevenueAnalysis(filteredTransactions, filteredAppointments),
        servicePerformance: this.calculateServicePerformance(services, filteredAppointments, filteredTransactions),
        staffPerformance: this.calculateStaffPerformance(staff, filteredAppointments, filteredTransactions),
        clientRetention: this.calculateClientRetention(clients, filteredAppointments),
        busyHours: this.calculateBusyHours(filteredAppointments),
        cancellationAnalysis: this.calculateCancellationAnalysis(filteredAppointments)
      };

    } catch (error) {
      console.error('Erro ao calcular dados de análise:', error);
      throw new Error('Falha ao calcular dados de análise');
    }
  }

  private calculateClientGrowth(clients: any[]): Array<{ month: string; newClients: number; totalClients: number }> {
    const monthlyData: { [key: string]: number } = {};
    
    clients.forEach(client => {
      const date = new Date(client.createdAt || Date.now());
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      monthlyData[monthKey] = (monthlyData[monthKey] || 0) + 1;
    });

    const result = [];
    const now = new Date();
    let totalClients = 0;
    
    for (let i = 11; i >= 0; i--) {
      const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      const monthName = date.toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' });
      const newClients = monthlyData[monthKey] || 0;
      totalClients += newClients;
      
      result.push({
        month: monthName,
        newClients,
        totalClients
      });
    }

    return result;
  }

  private calculateRevenueAnalysis(transactions: any[], appointments: any[]): Array<{ month: string; revenue: number; appointments: number; averageTicket: number }> {
    const monthlyData: { [key: string]: { revenue: number; appointments: number } } = {};
    
    transactions
      .filter(t => t.type === 'income')
      .forEach(transaction => {
        const date = new Date(transaction.createdAt || Date.now());
        const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
        
        if (!monthlyData[monthKey]) {
          monthlyData[monthKey] = { revenue: 0, appointments: 0 };
        }
        monthlyData[monthKey].revenue += transaction.amount;
      });

    appointments.forEach(apt => {
      const date = new Date(apt.startTime);
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      
      if (!monthlyData[monthKey]) {
        monthlyData[monthKey] = { revenue: 0, appointments: 0 };
      }
      monthlyData[monthKey].appointments += 1;
    });

    const result = [];
    const now = new Date();
    
    for (let i = 11; i >= 0; i--) {
      const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      const monthName = date.toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' });
      const data = monthlyData[monthKey] || { revenue: 0, appointments: 0 };
      
      result.push({
        month: monthName,
        revenue: data.revenue,
        appointments: data.appointments,
        averageTicket: data.appointments > 0 ? data.revenue / data.appointments : 0
      });
    }

    return result;
  }

  private calculateServicePerformance(services: any[], appointments: any[], transactions: any[]): Array<any> {
    return services.map(service => {
      const serviceAppointments = appointments.filter(apt => apt.serviceId === service.id);
      const serviceRevenue = transactions
        .filter(t => t.type === 'income' && t.description?.includes(service.name))
        .reduce((sum, t) => sum + t.amount, 0);

      const bookings = serviceAppointments.length;
      const revenue = serviceRevenue || (bookings * (service.price || 0));
      const averagePrice = bookings > 0 ? revenue / bookings : service.price || 0;
      
      // Score de popularidade baseado em frequência e receita
      const popularityScore = bookings * 0.7 + (revenue / 1000) * 0.3;

      return {
        id: service.id,
        name: service.name,
        category: service.category,
        bookings,
        revenue,
        averagePrice,
        popularityScore: Math.round(popularityScore)
      };
    }).sort((a, b) => b.popularityScore - a.popularityScore);
  }

  private calculateStaffPerformance(staff: any[], appointments: any[], transactions: any[]): Array<any> {
    return staff.map(member => {
      const staffAppointments = appointments.filter(apt => 
        apt.staffId === member.id && apt.status === 'completed'
      );
      
      const staffRevenue = transactions
        .filter(t => t.type === 'income' && t.description?.includes(member.specialization))
        .reduce((sum, t) => sum + t.amount, 0);

      const appointmentsCompleted = staffAppointments.length;
      const revenue = staffRevenue;
      
      // Calcular taxa de utilização (assumindo 8h/dia, 5 dias/semana)
      const totalPossibleHours = 8 * 5 * 4; // 4 semanas
      const workedHours = appointmentsCompleted * 1; // Assumindo 1h por consulta
      const utilizationRate = Math.min((workedHours / totalPossibleHours) * 100, 100);

      return {
        id: member.id,
        name: member.specialization, // Usando specialization como nome
        specialization: member.specialization,
        appointmentsCompleted,
        revenue,
        averageRating: 4.5 + Math.random() * 0.5, // Mock por enquanto
        utilizationRate: Math.round(utilizationRate)
      };
    });
  }

  private calculateClientRetention(clients: any[], appointments: any[]): any {
    const clientVisits: { [key: number]: number } = {};
    
    appointments.forEach(apt => {
      clientVisits[apt.clientId] = (clientVisits[apt.clientId] || 0) + 1;
    });

    const totalClients = clients.length;
    const returningClients = Object.values(clientVisits).filter(visits => visits > 1).length;
    const totalVisits = Object.values(clientVisits).reduce((sum, visits) => sum + visits, 0);
    
    return {
      returnRate: totalClients > 0 ? Math.round((returningClients / totalClients) * 100) : 0,
      averageVisits: totalClients > 0 ? Math.round(totalVisits / totalClients * 10) / 10 : 0,
      loyalClients: Object.values(clientVisits).filter(visits => visits >= 5).length,
      churnRate: totalClients > 0 ? Math.round(((totalClients - returningClients) / totalClients) * 100) : 0
    };
  }

  private calculateBusyHours(appointments: any[]): Array<any> {
    const hourlyData: { [key: string]: number } = {};
    
    appointments.forEach(apt => {
      const date = new Date(apt.startTime);
      const hour = date.getHours();
      const dayOfWeek = date.toLocaleDateString('pt-BR', { weekday: 'short' });
      const key = `${hour}:00-${dayOfWeek}`;
      
      hourlyData[key] = (hourlyData[key] || 0) + 1;
    });

    return Object.entries(hourlyData)
      .map(([key, count]) => {
        const [hour, dayOfWeek] = key.split('-');
        return {
          hour,
          appointments: count,
          dayOfWeek
        };
      })
      .sort((a, b) => b.appointments - a.appointments);
  }

  private calculateCancellationAnalysis(appointments: any[]): any {
    const cancelledAppointments = appointments.filter(apt => 
      apt.status === 'cancelled' || apt.status === 'no_show'
    );
    
    const totalAppointments = appointments.length;
    const totalCancellations = cancelledAppointments.length;
    const cancellationRate = totalAppointments > 0 ? 
      Math.round((totalCancellations / totalAppointments) * 100) : 0;

    // Mock de razões por enquanto
    const topReasons = [
      { reason: 'Conflito de agenda', count: Math.floor(totalCancellations * 0.4) },
      { reason: 'Problemas de saúde', count: Math.floor(totalCancellations * 0.3) },
      { reason: 'Emergência', count: Math.floor(totalCancellations * 0.2) },
      { reason: 'Outros', count: Math.floor(totalCancellations * 0.1) }
    ];

    return {
      totalCancellations,
      cancellationRate,
      topReasons
    };
  }
}