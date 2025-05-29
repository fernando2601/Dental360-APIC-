// Service para conectar com o backend .NET Core
const DOTNET_API_BASE = 'http://localhost:5001/api';

class DotNetApiService {
  private baseUrl = DOTNET_API_BASE;
  private token: string | null = null;

  constructor() {
    this.token = localStorage.getItem('auth_token');
  }

  private async request(endpoint: string, options: RequestInit = {}) {
    const url = `${this.baseUrl}${endpoint}`;
    
    const headers = {
      'Content-Type': 'application/json',
      ...(this.token && { Authorization: `Bearer ${this.token}` }),
      ...options.headers,
    };

    const config: RequestInit = {
      ...options,
      headers,
    };

    const response = await fetch(url, config);

    if (!response.ok) {
      if (response.status === 401) {
        this.token = null;
        localStorage.removeItem('auth_token');
        window.location.href = '/login';
      }
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  // Auth endpoints
  async login(username: string, password: string) {
    const response = await fetch(`${this.baseUrl}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    });

    if (!response.ok) {
      throw new Error('Credenciais inválidas');
    }

    const data = await response.json();
    this.token = data.token;
    localStorage.setItem('auth_token', data.token);
    return data;
  }

  async logout() {
    this.token = null;
    localStorage.removeItem('auth_token');
    return this.request('/auth/logout', { method: 'POST' });
  }

  async getCurrentUser() {
    return this.request('/auth/me');
  }

  // Patient endpoints
  async getPatients(filters = {}) {
    const params = new URLSearchParams(filters as any).toString();
    return this.request(`/patient/filter?${params}`);
  }

  async getPatient(id: number) {
    return this.request(`/patient/${id}`);
  }

  async createPatient(data: any) {
    return this.request('/patient', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updatePatient(id: number, data: any) {
    return this.request(`/patient/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async deletePatient(id: number) {
    return this.request(`/patient/${id}`, { method: 'DELETE' });
  }

  async getPatientAnalytics() {
    return this.request('/patient/analytics');
  }

  // Agenda endpoints
  async getAppointments(filters = {}) {
    const params = new URLSearchParams(filters as any).toString();
    return this.request(`/agenda/filter?${params}`);
  }

  async getCalendarView(startDate: string, endDate: string) {
    return this.request(`/agenda/calendar?startDate=${startDate}&endDate=${endDate}`);
  }

  async getTodayAppointments() {
    return this.request('/agenda/today');
  }

  async createAppointment(data: any) {
    return this.request('/agenda', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateAppointment(id: number, data: any) {
    return this.request(`/agenda/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async confirmAppointment(id: number) {
    return this.request(`/agenda/${id}/confirm`, { method: 'POST' });
  }

  async cancelAppointment(id: number, reason: string) {
    return this.request(`/agenda/${id}/cancel`, {
      method: 'POST',
      body: JSON.stringify({ reason }),
    });
  }

  // Inventory endpoints
  async getInventory(filters = {}) {
    const params = new URLSearchParams(filters as any).toString();
    return this.request(`/inventory/filter?${params}`);
  }

  async getInventoryItem(id: number) {
    return this.request(`/inventory/${id}`);
  }

  async createInventoryItem(data: any) {
    return this.request('/inventory', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateInventoryItem(id: number, data: any) {
    return this.request(`/inventory/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async deleteInventoryItem(id: number) {
    return this.request(`/inventory/${id}`, { method: 'DELETE' });
  }

  async getInventoryAnalytics() {
    return this.request('/inventory/analytics');
  }

  // Financial endpoints
  async getTransactions(filters = {}) {
    const params = new URLSearchParams(filters as any).toString();
    return this.request(`/financial/transactions/filter?${params}`);
  }

  async createTransaction(data: any) {
    return this.request('/financial/transactions', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async updateTransaction(id: number, data: any) {
    return this.request(`/financial/transactions/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async getFinancialAnalytics() {
    return this.request('/financial/analytics');
  }

  async getDashboardMetrics() {
    return this.request('/financial/dashboard-metrics');
  }

  // Dashboard endpoints
  async getDashboardData() {
    const [patients, appointments, inventory, financial] = await Promise.all([
      this.request('/patient/dashboard-metrics'),
      this.request('/agenda/dashboard-metrics'),
      this.request('/inventory/dashboard-metrics'),
      this.request('/financial/dashboard-metrics'),
    ]);

    return {
      patients,
      appointments,
      inventory,
      financial,
    };
  }
}

export const dotNetApi = new DotNetApiService();
export default dotNetApi;