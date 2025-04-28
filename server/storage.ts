import {
  users, User, InsertUser,
  clients, Client, InsertClient,
  services, Service, InsertService,
  staff, Staff, InsertStaff,
  appointments, Appointment, InsertAppointment,
  inventory, Inventory, InsertInventory,
  financialTransactions, FinancialTransaction, InsertFinancialTransaction,
  chatTemplates, ChatTemplate, InsertChatTemplate
} from "@shared/schema";

export interface IStorage {
  // Users
  getUser(id: number): Promise<User | undefined>;
  getUserByUsername(username: string): Promise<User | undefined>;
  getUserByEmail(email: string): Promise<User | undefined>;
  getUserByResetToken(token: string): Promise<User | undefined>;
  createUser(user: InsertUser): Promise<User>;
  updateUserPassword(id: number, password: string): Promise<User | undefined>;
  updateUserLastLogin(id: number): Promise<User | undefined>;
  updateUserResetToken(id: number, token: string, expiry: Date): Promise<User | undefined>;
  clearUserResetToken(id: number): Promise<User | undefined>;
  
  // Clients
  getClients(): Promise<Client[]>;
  getClient(id: number): Promise<Client | undefined>;
  createClient(client: InsertClient): Promise<Client>;
  updateClient(id: number, client: Partial<InsertClient>): Promise<Client | undefined>;
  deleteClient(id: number): Promise<boolean>;
  
  // Services
  getServices(): Promise<Service[]>;
  getService(id: number): Promise<Service | undefined>;
  createService(service: InsertService): Promise<Service>;
  updateService(id: number, service: Partial<InsertService>): Promise<Service | undefined>;
  deleteService(id: number): Promise<boolean>;
  
  // Staff
  getStaffMembers(): Promise<Staff[]>;
  getStaffMember(id: number): Promise<Staff | undefined>;
  createStaffMember(staffMember: InsertStaff): Promise<Staff>;
  updateStaffMember(id: number, staffMember: Partial<InsertStaff>): Promise<Staff | undefined>;
  deleteStaffMember(id: number): Promise<boolean>;
  
  // Appointments
  getAppointments(): Promise<Appointment[]>;
  getAppointment(id: number): Promise<Appointment | undefined>;
  getAppointmentsByClient(clientId: number): Promise<Appointment[]>;
  getAppointmentsByStaff(staffId: number): Promise<Appointment[]>;
  getAppointmentsByDateRange(startDate: Date, endDate: Date): Promise<Appointment[]>;
  createAppointment(appointment: InsertAppointment): Promise<Appointment>;
  updateAppointment(id: number, appointment: Partial<InsertAppointment>): Promise<Appointment | undefined>;
  deleteAppointment(id: number): Promise<boolean>;
  
  // Inventory
  getInventoryItems(): Promise<Inventory[]>;
  getInventoryItem(id: number): Promise<Inventory | undefined>;
  createInventoryItem(item: InsertInventory): Promise<Inventory>;
  updateInventoryItem(id: number, item: Partial<InsertInventory>): Promise<Inventory | undefined>;
  deleteInventoryItem(id: number): Promise<boolean>;
  
  // Financial Transactions
  getFinancialTransactions(): Promise<FinancialTransaction[]>;
  getFinancialTransaction(id: number): Promise<FinancialTransaction | undefined>;
  getFinancialTransactionsByDateRange(startDate: Date, endDate: Date): Promise<FinancialTransaction[]>;
  createFinancialTransaction(transaction: InsertFinancialTransaction): Promise<FinancialTransaction>;
  updateFinancialTransaction(id: number, transaction: Partial<InsertFinancialTransaction>): Promise<FinancialTransaction | undefined>;
  deleteFinancialTransaction(id: number): Promise<boolean>;
  
  // Chat Templates
  getChatTemplates(): Promise<ChatTemplate[]>;
  getChatTemplate(id: number): Promise<ChatTemplate | undefined>;
  getChatTemplatesByCategory(category: string): Promise<ChatTemplate[]>;
  createChatTemplate(template: InsertChatTemplate): Promise<ChatTemplate>;
  updateChatTemplate(id: number, template: Partial<InsertChatTemplate>): Promise<ChatTemplate | undefined>;
  deleteChatTemplate(id: number): Promise<boolean>;
  incrementChatTemplateUsage(id: number): Promise<ChatTemplate | undefined>;
}

export class MemStorage implements IStorage {
  private users: Map<number, User>;
  private clients: Map<number, Client>;
  private services: Map<number, Service>;
  private staffMembers: Map<number, Staff>;
  private appointments: Map<number, Appointment>;
  private inventoryItems: Map<number, Inventory>;
  private financialTransactions: Map<number, FinancialTransaction>;
  private chatTemplates: Map<number, ChatTemplate>;
  
  private currentUserID: number;
  private currentClientID: number;
  private currentServiceID: number;
  private currentStaffID: number;
  private currentAppointmentID: number;
  private currentInventoryID: number;
  private currentTransactionID: number;
  private currentTemplateID: number;

  constructor() {
    this.users = new Map();
    this.clients = new Map();
    this.services = new Map();
    this.staffMembers = new Map();
    this.appointments = new Map();
    this.inventoryItems = new Map();
    this.financialTransactions = new Map();
    this.chatTemplates = new Map();
    
    this.currentUserID = 1;
    this.currentClientID = 1;
    this.currentServiceID = 1;
    this.currentStaffID = 1;
    this.currentAppointmentID = 1;
    this.currentInventoryID = 1;
    this.currentTransactionID = 1;
    this.currentTemplateID = 1;
    
    // Initialize with some sample data (async)
    this.initializeData().catch(error => {
      console.error("Erro ao inicializar dados:", error);
    });
  }

  private async initializeData(): Promise<void> {
    const hashPassword = async (password: string): Promise<string> => {
      const salt = randomBytes(16).toString("hex");
      const buf = await scryptAsync(password, salt, 64) as Buffer;
      return `${buf.toString("hex")}.${salt}`;
    };
    
    // Importações necessárias para hash de senha
    const { randomBytes } = await import("crypto");
    const { promisify } = await import("util");
    const scryptAsync = promisify(randomBytes);
    // Sample services
    const services = [
      {
        name: "Limpeza Dental",
        category: "Odontológico",
        description: "Limpeza profissional dos dentes para remover placa e tártaro.",
        duration: 45,
        price: 120,
        active: true
      },
      {
        name: "Tratamento com Botox",
        category: "Estética",
        description: "Injeções de Botox para reduzir rugas e linhas finas.",
        duration: 30,
        price: 350,
        active: true
      },
      {
        name: "Clareamento Dental",
        category: "Odontológico",
        description: "Clareamento profissional dos dentes para um sorriso mais brilhante.",
        duration: 60,
        price: 250,
        active: true
      },
      {
        name: "Harmonização Facial",
        category: "Estética",
        description: "Proporções faciais equilibradas usando preenchedores dérmicos.",
        duration: 90,
        price: 700,
        active: true
      }
    ];

        // Criar usuários diretamente com senhas em texto puro para DEMO
    // Em um ambiente de produção real, NUNCA faça isso
    
    // Create admin user
    this.createUser({
      username: "admin",
      password: "admin123", // Senha em texto puro para DEMO
      fullName: "Administrador",
      role: "admin",
      email: "admin@clinicadental.com",
      phone: "555-123-4567"
    });
    
    // Create requested admin user
    this.createUser({
      username: "nerifernando",
      password: "@Brazucas", // Senha em texto puro para DEMO
      fullName: "Neri Fernando",
      role: "admin",
      email: "nerifernando2@gmail.com",
      phone: "555-987-1234"
    });
    
    // Create non-admin user with same password
    this.createUser({
      username: "funcionario",
      password: "@Brazucas", // Senha em texto puro para DEMO
      fullName: "Funcionário Padrão",
      role: "staff",
      email: "funcionario@clinicadental.com",
      phone: "555-456-7890"
    });
    
    // Imprimindo credenciais no console para facilitar debug
    console.log("USUÁRIOS CRIADOS COM SUCESSO:");
    console.log("- admin/admin123");
    console.log("- nerifernando/@Brazucas");
    console.log("- funcionario/@Brazucas");

    // Create a dentist
    const dentist = this.createUser({
      username: "drsilva",
      password: "password123", // Senha em texto puro para DEMO
      fullName: "Dra. Ana Silva",
      role: "dentista",
      email: "silva@clinicadental.com",
      phone: "555-987-6543"
    });

    // Create a staff member
    this.createStaffMember({
      userId: dentist.id,
      specialization: "Odontologia Geral",
      bio: "Dra. Silva é especialista em odontologia geral e estética.",
      available: true
    });

    // Create services
    services.forEach(service => {
      this.createService(service);
    });

    // Criar 10 pacientes fictícios
    const pacientes = [
      {
        fullName: "Roberto Almeida",
        email: "roberto.almeida@email.com",
        phone: "(11) 99827-6543",
        address: "Rua das Flores, 123 - Vila Mariana, São Paulo-SP",
        birthday: "1985-06-15",
        notes: "Prefere agendamentos no final da tarde. Frequência: Semestral. Instagram: @robalmeida. Cliente desde: jan/2019. Alérgico a anestesia com epinefrina."
      },
      {
        fullName: "Amanda Costa",
        email: "amanda.costa@email.com",
        phone: "(11) 98765-4321",
        address: "Av. Paulista, 1000 - Bela Vista, São Paulo-SP",
        birthday: "1990-03-25",
        notes: "Prefere pagamentos em PIX. Frequência: Trimestral. Instagram: @amandacosta90. Cliente desde: nov/2020. Indica muitos pacientes."
      },
      {
        fullName: "Carlos Eduardo Martins",
        email: "carlos.martins@email.com",
        phone: "(11) 97654-3210",
        address: "Rua Augusta, 789 - Consolação, São Paulo-SP",
        birthday: "1978-11-10",
        notes: "Tem ansiedade em tratamentos dentários. Frequência: Anual. Instagram: @carlosedu78. Cliente desde: mar/2018. Trabalha em horários flexíveis."
      },
      {
        fullName: "Juliana Peixoto",
        email: "juliana.peixoto@email.com",
        phone: "(11) 96543-2109",
        address: "Alameda Santos, 456 - Jardins, São Paulo-SP",
        birthday: "1992-08-17",
        notes: "Interessada em harmonização facial. Frequência: Bimestral. Instagram: @jujupeixoto. Cliente desde: fev/2021. Influenciadora digital com 50k seguidores."
      },
      {
        fullName: "Ricardo Oliveira",
        email: "ricardo.oliveira@email.com",
        phone: "(11) 95432-1098",
        address: "Rua Vergueiro, 321 - Vila Mariana, São Paulo-SP",
        birthday: "1975-04-30",
        notes: "Prefere atendimento com Dr. Silva. Frequência: Semestral. Instagram: @ricardooli. Cliente desde: mai/2017. Executivo, prefere horários no início da manhã."
      },
      {
        fullName: "Patrícia Mendes",
        email: "patricia.mendes@email.com",
        phone: "(11) 94321-0987",
        address: "Rua Oscar Freire, 654 - Jardins, São Paulo-SP",
        birthday: "1988-09-22",
        notes: "Histórico de bruxismo. Frequência: Trimestral. Instagram: @patimendes. Cliente desde: set/2019. Dificuldade com anestesia local."
      },
      {
        fullName: "Fernando Carvalho",
        email: "fernando.carvalho@email.com",
        phone: "(11) 93210-9876",
        address: "Av. Rebouças, 987 - Pinheiros, São Paulo-SP",
        birthday: "1983-12-05",
        notes: "Prefere receber lembretes por WhatsApp. Frequência: Semestral. Instagram: @fernandocarv. Cliente desde: jul/2020. Profissional autônomo."
      },
      {
        fullName: "Camila Rodrigues",
        email: "camila.rodrigues@email.com",
        phone: "(11) 92109-8765",
        address: "Rua Pamplona, 234 - Jardim Paulista, São Paulo-SP",
        birthday: "1995-02-14",
        notes: "Interesse em tratamentos estéticos. Frequência: Mensal. Instagram: @camilarodrigues95. Cliente desde: dez/2021. Prefere pagar em parcelas."
      },
      {
        fullName: "Marcelo Santos",
        email: "marcelo.santos@email.com",
        phone: "(11) 91098-7654",
        address: "Rua da Consolação, 543 - Consolação, São Paulo-SP",
        birthday: "1972-07-20",
        notes: "Hipertenso, toma medicação controlada. Frequência: Anual. Instagram: @celosantos72. Cliente desde: abr/2016. Professor universitário."
      },
      {
        fullName: "Aline Ferreira",
        email: "aline.ferreira@email.com",
        phone: "(11) 90987-6543",
        address: "Av. Brigadeiro Faria Lima, 876 - Itaim Bibi, São Paulo-SP",
        birthday: "1987-05-08",
        notes: "Indicada pela Juliana Peixoto. Frequência: Bimestral. Instagram: @alineferreira87. Cliente desde: out/2020. Sempre faz botox a cada 6 meses."
      }
    ];

    // Adicionar os pacientes ao sistema
    pacientes.forEach((paciente) => {
      this.createClient(paciente);
    });

    // Create chat templates
    this.createChatTemplate({
      title: "Informações sobre Procedimento de Botox",
      category: "Informações de Tratamento",
      content: "Nosso procedimento de Botox é um tratamento minimamente invasivo que envolve a injeção de pequenas quantidades de toxina botulínica purificada em músculos específicos para reduzir temporariamente a aparência de rugas e linhas finas. O procedimento leva aproximadamente 15-20 minutos, e a maioria dos pacientes experimenta desconforto mínimo. Os resultados geralmente aparecem dentro de 3-5 dias após o tratamento e duram cerca de 3-4 meses.",
      active: true
    });

    this.createChatTemplate({
      title: "Processo de Agendamento de Consultas",
      category: "Agendamento de Consultas",
      content: "Para agendar uma consulta, você pode usar nosso sistema de reservas online a qualquer momento ou ligar para nosso consultório durante o horário comercial (segunda a sexta, 9h-17h). Exigimos um aviso prévio de 24 horas para cancelamentos para evitar uma taxa de cancelamento.",
      active: true
    });

    // Create inventory items
    this.createInventoryItem({
      name: "Botox",
      category: "Injetáveis",
      description: "Toxina botulínica para redução de rugas",
      quantity: 50,
      unit: "unidades",
      threshold: 10,
      price: 12,
      lastRestocked: new Date()
    });

    this.createInventoryItem({
      name: "Fio Dental",
      category: "Suprimentos Odontológicos",
      description: "Fio dental profissional",
      quantity: 100,
      unit: "caixas",
      threshold: 20,
      price: 5,
      lastRestocked: new Date()
    });
  }

  // User methods
  async getUser(id: number): Promise<User | undefined> {
    return this.users.get(id);
  }

  async getUserByUsername(username: string): Promise<User | undefined> {
    return Array.from(this.users.values()).find(
      (user) => user.username === username
    );
  }

  async createUser(insertUser: InsertUser): Promise<User> {
    const id = this.currentUserID++;
    const now = new Date();
    const user: User = { ...insertUser, id, createdAt: now };
    this.users.set(id, user);
    return user;
  }

  async getUserByEmail(email: string): Promise<User | undefined> {
    return Array.from(this.users.values()).find(
      (user) => user.email === email
    );
  }

  async getUserByResetToken(token: string): Promise<User | undefined> {
    return Array.from(this.users.values()).find(
      (user) => user.resetToken === token
    );
  }

  async updateUserPassword(id: number, password: string): Promise<User | undefined> {
    const user = this.users.get(id);
    if (!user) return undefined;
    
    const updated = { ...user, password };
    this.users.set(id, updated);
    return updated;
  }

  async updateUserLastLogin(id: number): Promise<User | undefined> {
    const user = this.users.get(id);
    if (!user) return undefined;
    
    const updated = { ...user, lastLogin: new Date() };
    this.users.set(id, updated);
    return updated;
  }

  async updateUserResetToken(id: number, token: string, expiry: Date): Promise<User | undefined> {
    const user = this.users.get(id);
    if (!user) return undefined;
    
    const updated = { ...user, resetToken: token, resetTokenExpiry: expiry };
    this.users.set(id, updated);
    return updated;
  }

  async clearUserResetToken(id: number): Promise<User | undefined> {
    const user = this.users.get(id);
    if (!user) return undefined;
    
    const updated = { ...user, resetToken: null, resetTokenExpiry: null };
    this.users.set(id, updated);
    return updated;
  }

  // Client methods
  async getClients(): Promise<Client[]> {
    return Array.from(this.clients.values());
  }

  async getClient(id: number): Promise<Client | undefined> {
    return this.clients.get(id);
  }

  async createClient(insertClient: InsertClient): Promise<Client> {
    const id = this.currentClientID++;
    const now = new Date();
    const client: Client = { ...insertClient, id, createdAt: now };
    this.clients.set(id, client);
    return client;
  }

  async updateClient(id: number, updatedClient: Partial<InsertClient>): Promise<Client | undefined> {
    const client = this.clients.get(id);
    if (!client) return undefined;
    
    const updated = { ...client, ...updatedClient };
    this.clients.set(id, updated);
    return updated;
  }

  async deleteClient(id: number): Promise<boolean> {
    return this.clients.delete(id);
  }

  // Service methods
  async getServices(): Promise<Service[]> {
    return Array.from(this.services.values());
  }

  async getService(id: number): Promise<Service | undefined> {
    return this.services.get(id);
  }

  async createService(insertService: InsertService): Promise<Service> {
    const id = this.currentServiceID++;
    const service: Service = { ...insertService, id };
    this.services.set(id, service);
    return service;
  }

  async updateService(id: number, updatedService: Partial<InsertService>): Promise<Service | undefined> {
    const service = this.services.get(id);
    if (!service) return undefined;
    
    const updated = { ...service, ...updatedService };
    this.services.set(id, updated);
    return updated;
  }

  async deleteService(id: number): Promise<boolean> {
    return this.services.delete(id);
  }

  // Staff methods
  async getStaffMembers(): Promise<Staff[]> {
    return Array.from(this.staffMembers.values());
  }

  async getStaffMember(id: number): Promise<Staff | undefined> {
    return this.staffMembers.get(id);
  }

  async createStaffMember(insertStaff: InsertStaff): Promise<Staff> {
    const id = this.currentStaffID++;
    const staff: Staff = { ...insertStaff, id };
    this.staffMembers.set(id, staff);
    return staff;
  }

  async updateStaffMember(id: number, updatedStaff: Partial<InsertStaff>): Promise<Staff | undefined> {
    const staff = this.staffMembers.get(id);
    if (!staff) return undefined;
    
    const updated = { ...staff, ...updatedStaff };
    this.staffMembers.set(id, updated);
    return updated;
  }

  async deleteStaffMember(id: number): Promise<boolean> {
    return this.staffMembers.delete(id);
  }

  // Appointment methods
  async getAppointments(): Promise<Appointment[]> {
    return Array.from(this.appointments.values());
  }

  async getAppointment(id: number): Promise<Appointment | undefined> {
    return this.appointments.get(id);
  }

  async getAppointmentsByClient(clientId: number): Promise<Appointment[]> {
    return Array.from(this.appointments.values()).filter(
      (appointment) => appointment.clientId === clientId
    );
  }

  async getAppointmentsByStaff(staffId: number): Promise<Appointment[]> {
    return Array.from(this.appointments.values()).filter(
      (appointment) => appointment.staffId === staffId
    );
  }

  async getAppointmentsByDateRange(startDate: Date, endDate: Date): Promise<Appointment[]> {
    return Array.from(this.appointments.values()).filter(
      (appointment) => {
        const appointmentStart = new Date(appointment.startTime);
        return appointmentStart >= startDate && appointmentStart <= endDate;
      }
    );
  }

  async createAppointment(insertAppointment: InsertAppointment): Promise<Appointment> {
    const id = this.currentAppointmentID++;
    const now = new Date();
    const appointment: Appointment = { ...insertAppointment, id, createdAt: now };
    this.appointments.set(id, appointment);
    return appointment;
  }

  async updateAppointment(id: number, updatedAppointment: Partial<InsertAppointment>): Promise<Appointment | undefined> {
    const appointment = this.appointments.get(id);
    if (!appointment) return undefined;
    
    const updated = { ...appointment, ...updatedAppointment };
    this.appointments.set(id, updated);
    return updated;
  }

  async deleteAppointment(id: number): Promise<boolean> {
    return this.appointments.delete(id);
  }

  // Inventory methods
  async getInventoryItems(): Promise<Inventory[]> {
    return Array.from(this.inventoryItems.values());
  }

  async getInventoryItem(id: number): Promise<Inventory | undefined> {
    return this.inventoryItems.get(id);
  }

  async createInventoryItem(insertInventory: InsertInventory): Promise<Inventory> {
    const id = this.currentInventoryID++;
    const inventory: Inventory = { ...insertInventory, id };
    this.inventoryItems.set(id, inventory);
    return inventory;
  }

  async updateInventoryItem(id: number, updatedInventory: Partial<InsertInventory>): Promise<Inventory | undefined> {
    const inventory = this.inventoryItems.get(id);
    if (!inventory) return undefined;
    
    const updated = { ...inventory, ...updatedInventory };
    this.inventoryItems.set(id, updated);
    return updated;
  }

  async deleteInventoryItem(id: number): Promise<boolean> {
    return this.inventoryItems.delete(id);
  }

  // Financial Transaction methods
  async getFinancialTransactions(): Promise<FinancialTransaction[]> {
    return Array.from(this.financialTransactions.values());
  }

  async getFinancialTransaction(id: number): Promise<FinancialTransaction | undefined> {
    return this.financialTransactions.get(id);
  }

  async getFinancialTransactionsByDateRange(startDate: Date, endDate: Date): Promise<FinancialTransaction[]> {
    return Array.from(this.financialTransactions.values()).filter(
      (transaction) => {
        const transactionDate = new Date(transaction.date);
        return transactionDate >= startDate && transactionDate <= endDate;
      }
    );
  }

  async createFinancialTransaction(insertTransaction: InsertFinancialTransaction): Promise<FinancialTransaction> {
    const id = this.currentTransactionID++;
    const transaction: FinancialTransaction = { ...insertTransaction, id };
    this.financialTransactions.set(id, transaction);
    return transaction;
  }

  async updateFinancialTransaction(id: number, updatedTransaction: Partial<InsertFinancialTransaction>): Promise<FinancialTransaction | undefined> {
    const transaction = this.financialTransactions.get(id);
    if (!transaction) return undefined;
    
    const updated = { ...transaction, ...updatedTransaction };
    this.financialTransactions.set(id, updated);
    return updated;
  }

  async deleteFinancialTransaction(id: number): Promise<boolean> {
    return this.financialTransactions.delete(id);
  }

  // Chat Template methods
  async getChatTemplates(): Promise<ChatTemplate[]> {
    return Array.from(this.chatTemplates.values());
  }

  async getChatTemplate(id: number): Promise<ChatTemplate | undefined> {
    return this.chatTemplates.get(id);
  }

  async getChatTemplatesByCategory(category: string): Promise<ChatTemplate[]> {
    return Array.from(this.chatTemplates.values()).filter(
      (template) => template.category === category
    );
  }

  async createChatTemplate(insertTemplate: InsertChatTemplate): Promise<ChatTemplate> {
    const id = this.currentTemplateID++;
    const now = new Date();
    const template: ChatTemplate = { 
      ...insertTemplate, 
      id, 
      usageCount: 0, 
      lastUsed: null, 
      createdAt: now, 
      updatedAt: now 
    };
    this.chatTemplates.set(id, template);
    return template;
  }

  async updateChatTemplate(id: number, updatedTemplate: Partial<InsertChatTemplate>): Promise<ChatTemplate | undefined> {
    const template = this.chatTemplates.get(id);
    if (!template) return undefined;
    
    const now = new Date();
    const updated = { ...template, ...updatedTemplate, updatedAt: now };
    this.chatTemplates.set(id, updated);
    return updated;
  }

  async deleteChatTemplate(id: number): Promise<boolean> {
    return this.chatTemplates.delete(id);
  }

  async incrementChatTemplateUsage(id: number): Promise<ChatTemplate | undefined> {
    const template = this.chatTemplates.get(id);
    if (!template) return undefined;
    
    const now = new Date();
    const updated = { 
      ...template, 
      usageCount: template.usageCount + 1,
      lastUsed: now,
      updatedAt: now
    };
    this.chatTemplates.set(id, updated);
    return updated;
  }
}

export const storage = new MemStorage();
