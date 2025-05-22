import { IStorage } from "../storage";

export interface Package {
  id: number;
  name: string;
  description: string;
  services: number[]; // IDs dos serviços inclusos
  price: number;
  originalPrice: number;
  discount: number;
  duration: string; // ex: "3 meses"
  isActive: boolean;
  features: string[];
  category: string;
  createdAt: Date;
}

export interface PackageStats {
  totalPackages: number;
  activePackages: number;
  totalRevenue: number;
  popularPackages: Array<{
    id: number;
    name: string;
    sales: number;
    revenue: number;
  }>;
  categoryStats: Array<{
    category: string;
    count: number;
    revenue: number;
  }>;
}

export class PackagesService {
  constructor(private storage: IStorage) {}

  async getAllPackages(): Promise<Package[]> {
    try {
      // Por enquanto retornando dados mock, mas estruturado para dados reais
      return [
        {
          id: 1,
          name: "Pacote Básico Dental",
          description: "Cuidados essenciais para sua saúde bucal",
          services: [1, 2], // IDs dos serviços
          price: 299.99,
          originalPrice: 450.00,
          discount: 33,
          duration: "6 meses",
          isActive: true,
          features: ["Limpeza dental", "Consulta de avaliação", "Raio-X"],
          category: "Odontologia",
          createdAt: new Date()
        },
        {
          id: 2,
          name: "Pacote Harmonização Facial",
          description: "Tratamento completo de harmonização",
          services: [3, 4, 5],
          price: 899.99,
          originalPrice: 1200.00,
          discount: 25,
          duration: "4 meses",
          isActive: true,
          features: ["Botox", "Preenchimento", "Consulta especializada"],
          category: "Harmonização",
          createdAt: new Date()
        }
      ];
    } catch (error) {
      console.error('Erro ao buscar pacotes:', error);
      throw new Error('Falha ao buscar pacotes');
    }
  }

  async getPackageById(id: number): Promise<Package | null> {
    try {
      const packages = await this.getAllPackages();
      return packages.find(pkg => pkg.id === id) || null;
    } catch (error) {
      console.error('Erro ao buscar pacote:', error);
      throw new Error('Falha ao buscar pacote');
    }
  }

  async createPackage(packageData: Omit<Package, 'id' | 'createdAt'>): Promise<Package> {
    try {
      // Simular criação - em implementação real salvaria no banco
      const newPackage: Package = {
        ...packageData,
        id: Date.now(), // Mock ID
        createdAt: new Date()
      };
      
      return newPackage;
    } catch (error) {
      console.error('Erro ao criar pacote:', error);
      throw new Error('Falha ao criar pacote');
    }
  }

  async updatePackage(id: number, updates: Partial<Package>): Promise<Package | null> {
    try {
      const existingPackage = await this.getPackageById(id);
      if (!existingPackage) return null;

      // Simular atualização
      return { ...existingPackage, ...updates };
    } catch (error) {
      console.error('Erro ao atualizar pacote:', error);
      throw new Error('Falha ao atualizar pacote');
    }
  }

  async deletePackage(id: number): Promise<boolean> {
    try {
      const existingPackage = await this.getPackageById(id);
      return !!existingPackage;
    } catch (error) {
      console.error('Erro ao deletar pacote:', error);
      throw new Error('Falha ao deletar pacote');
    }
  }

  async getPackageStats(): Promise<PackageStats> {
    try {
      const packages = await this.getAllPackages();
      const appointments = await this.storage.getAppointments();
      const transactions = await this.storage.getFinancialTransactions();

      const totalPackages = packages.length;
      const activePackages = packages.filter(pkg => pkg.isActive).length;
      
      // Calcular receita de pacotes (simulado)
      const totalRevenue = packages.reduce((sum, pkg) => sum + pkg.price, 0);

      // Pacotes populares (baseado em simulação)
      const popularPackages = packages.map(pkg => ({
        id: pkg.id,
        name: pkg.name,
        sales: Math.floor(Math.random() * 50) + 10, // Mock
        revenue: pkg.price * (Math.floor(Math.random() * 50) + 10)
      })).sort((a, b) => b.sales - a.sales);

      // Estatísticas por categoria
      const categoryStats = packages.reduce((acc, pkg) => {
        const existing = acc.find(item => item.category === pkg.category);
        if (existing) {
          existing.count++;
          existing.revenue += pkg.price;
        } else {
          acc.push({
            category: pkg.category,
            count: 1,
            revenue: pkg.price
          });
        }
        return acc;
      }, [] as Array<{ category: string; count: number; revenue: number }>);

      return {
        totalPackages,
        activePackages,
        totalRevenue,
        popularPackages,
        categoryStats
      };

    } catch (error) {
      console.error('Erro ao calcular estatísticas de pacotes:', error);
      throw new Error('Falha ao calcular estatísticas de pacotes');
    }
  }
}