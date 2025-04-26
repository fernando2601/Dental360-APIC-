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
  createUser(user: InsertUser): Promise<User>;
  
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
    
    // Initialize with some sample data
    this.initializeData();
  }

  private initializeData(): void {
    // Sample services
    const services = [
      {
        name: "Dental Cleaning",
        category: "Dental",
        description: "Professional teeth cleaning to remove plaque and tartar.",
        duration: 45,
        price: 120,
        active: true
      },
      {
        name: "Botox Treatment",
        category: "Aesthetic",
        description: "Botox injections to reduce wrinkles and fine lines.",
        duration: 30,
        price: 350,
        active: true
      },
      {
        name: "Teeth Whitening",
        category: "Dental",
        description: "Professional teeth whitening for a brighter smile.",
        duration: 60,
        price: 250,
        active: true
      },
      {
        name: "Facial Harmonization",
        category: "Aesthetic",
        description: "Balanced facial proportions using dermal fillers.",
        duration: 90,
        price: 700,
        active: true
      }
    ];

    // Create admin user
    this.createUser({
      username: "admin",
      password: "admin123",
      fullName: "Admin User",
      role: "admin",
      email: "admin@dentalclinic.com",
      phone: "555-123-4567"
    });

    // Create a dentist
    const dentist = this.createUser({
      username: "drsmith",
      password: "password123",
      fullName: "Dr. Sarah Smith",
      role: "dentist",
      email: "smith@dentalclinic.com",
      phone: "555-987-6543"
    });

    // Create a staff member
    this.createStaffMember({
      userId: dentist.id,
      specialization: "General Dentistry",
      bio: "Dr. Smith specializes in general and cosmetic dentistry.",
      available: true
    });

    // Create services
    services.forEach(service => {
      this.createService(service);
    });

    // Create chat templates
    this.createChatTemplate({
      title: "Botox Procedure Information",
      category: "Treatment Information",
      content: "Our Botox procedure is a minimally invasive treatment that involves injecting small amounts of purified botulinum toxin into specific muscles to temporarily reduce the appearance of wrinkles and fine lines. The procedure takes approximately 15-20 minutes, and most patients experience minimal discomfort. Results typically appear within 3-5 days after treatment and last about 3-4 months.",
      active: true
    });

    this.createChatTemplate({
      title: "Appointment Scheduling Process",
      category: "Appointment Scheduling",
      content: "To schedule an appointment, you can use our online booking system at any time or call our office during business hours (Monday-Friday, 9am-5pm). We require a 24-hour notice for cancellations to avoid a cancellation fee.",
      active: true
    });

    // Create inventory items
    this.createInventoryItem({
      name: "Botox",
      category: "Injectables",
      description: "Botulinum toxin for wrinkle reduction",
      quantity: 50,
      unit: "units",
      threshold: 10,
      price: 12,
      lastRestocked: new Date()
    });

    this.createInventoryItem({
      name: "Dental Floss",
      category: "Dental Supplies",
      description: "Professional dental floss",
      quantity: 100,
      unit: "boxes",
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
