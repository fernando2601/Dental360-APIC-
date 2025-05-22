import { IStorage } from "../storage";

export interface ClinicInfo {
  id: number;
  name: string;
  cnpj: string;
  address: {
    street: string;
    number: string;
    complement?: string;
    neighborhood: string;
    city: string;
    state: string;
    zipCode: string;
  };
  contact: {
    phone: string;
    whatsapp: string;
    email: string;
    website?: string;
  };
  businessHours: Array<{
    day: string;
    open: string;
    close: string;
    isOpen: boolean;
  }>;
  specialties: string[];
  certifications: string[];
  socialMedia: {
    instagram?: string;
    facebook?: string;
    youtube?: string;
  };
  logo?: string;
  description: string;
  mission: string;
  vision: string;
  values: string[];
  updatedAt: Date;
}

export interface ClinicStats {
  totalAppointments: number;
  totalClients: number;
  yearsInOperation: number;
  staffCount: number;
  servicesOffered: number;
  satisfactionRate: number;
}

export class ClinicInfoService {
  constructor(private storage: IStorage) {}

  async getClinicInfo(): Promise<ClinicInfo> {
    try {
      // Dados reais da clínica - em implementação real viria do banco
      return {
        id: 1,
        name: "Clínica Dental & Harmonização",
        cnpj: "12.345.678/0001-90",
        address: {
          street: "Rua das Flores",
          number: "123",
          complement: "Sala 456",
          neighborhood: "Centro",
          city: "São Paulo",
          state: "SP",
          zipCode: "01234-567"
        },
        contact: {
          phone: "(11) 1234-5678",
          whatsapp: "(11) 98765-4321",
          email: "contato@clinicadental.com.br",
          website: "www.clinicadental.com.br"
        },
        businessHours: [
          { day: "Segunda", open: "08:00", close: "18:00", isOpen: true },
          { day: "Terça", open: "08:00", close: "18:00", isOpen: true },
          { day: "Quarta", open: "08:00", close: "18:00", isOpen: true },
          { day: "Quinta", open: "08:00", close: "18:00", isOpen: true },
          { day: "Sexta", open: "08:00", close: "17:00", isOpen: true },
          { day: "Sábado", open: "08:00", close: "12:00", isOpen: true },
          { day: "Domingo", open: "", close: "", isOpen: false }
        ],
        specialties: [
          "Odontologia Geral",
          "Harmonização Facial",
          "Implantodontia",
          "Ortodontia",
          "Clareamento Dental"
        ],
        certifications: [
          "CRO-SP 12345",
          "Certificação em Harmonização Facial",
          "ISO 9001:2015"
        ],
        socialMedia: {
          instagram: "@clinicadental",
          facebook: "clinicadental",
          youtube: "clinicadental"
        },
        description: "Clínica especializada em odontologia e harmonização facial, oferecendo tratamentos de alta qualidade com tecnologia de ponta.",
        mission: "Promover saúde bucal e bem-estar estético com excelência e humanização no atendimento.",
        vision: "Ser referência em odontologia e harmonização facial na região, reconhecida pela qualidade e inovação.",
        values: [
          "Excelência no atendimento",
          "Ética profissional",
          "Inovação tecnológica",
          "Humanização",
          "Responsabilidade social"
        ],
        updatedAt: new Date()
      };
    } catch (error) {
      console.error('Erro ao buscar informações da clínica:', error);
      throw new Error('Falha ao buscar informações da clínica');
    }
  }

  async updateClinicInfo(updates: Partial<ClinicInfo>): Promise<ClinicInfo> {
    try {
      const currentInfo = await this.getClinicInfo();
      
      // Simular atualização - em implementação real salvaria no banco
      const updatedInfo: ClinicInfo = {
        ...currentInfo,
        ...updates,
        updatedAt: new Date()
      };
      
      return updatedInfo;
    } catch (error) {
      console.error('Erro ao atualizar informações da clínica:', error);
      throw new Error('Falha ao atualizar informações da clínica');
    }
  }

  async getClinicStats(): Promise<ClinicStats> {
    try {
      const [clients, appointments, staff, services] = await Promise.all([
        this.storage.getClients(),
        this.storage.getAppointments(),
        this.storage.getStaffMembers(),
        this.storage.getServices()
      ]);

      // Calcular estatísticas reais baseadas nos dados do sistema
      const totalAppointments = appointments.length;
      const totalClients = clients.length;
      const staffCount = staff.filter(s => s.available).length;
      const servicesOffered = services.length;
      
      // Anos de operação (baseado no cliente mais antigo ou data fixa)
      const oldestClientDate = clients.reduce((oldest, client) => {
        const clientDate = new Date(client.createdAt || Date.now());
        return clientDate < oldest ? clientDate : oldest;
      }, new Date());
      
      const yearsInOperation = Math.max(1, 
        new Date().getFullYear() - oldestClientDate.getFullYear()
      );

      // Taxa de satisfação (baseada em agendamentos concluídos vs total)
      const completedAppointments = appointments.filter(apt => apt.status === 'completed').length;
      const satisfactionRate = totalAppointments > 0 ? 
        Math.round((completedAppointments / totalAppointments) * 100) : 0;

      return {
        totalAppointments,
        totalClients,
        yearsInOperation,
        staffCount,
        servicesOffered,
        satisfactionRate
      };

    } catch (error) {
      console.error('Erro ao calcular estatísticas da clínica:', error);
      throw new Error('Falha ao calcular estatísticas da clínica');
    }
  }

  async uploadLogo(logoData: string): Promise<string> {
    try {
      // Simular upload de logo - em implementação real salvaria no storage
      const logoUrl = `/uploads/logo-${Date.now()}.png`;
      
      // Atualizar informações da clínica com novo logo
      await this.updateClinicInfo({ logo: logoUrl });
      
      return logoUrl;
    } catch (error) {
      console.error('Erro ao fazer upload do logo:', error);
      throw new Error('Falha ao fazer upload do logo');
    }
  }
}