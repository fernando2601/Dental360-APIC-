import type { Express, Request, Response, NextFunction } from "express";
import { createServer, type Server } from "http";
import { storage } from "./storage";
import { setupAuth } from "./auth";
import { jwtAuthMiddleware, jwtAdminMiddleware, generateToken } from "./jwt";
import { z } from "zod";
import { DashboardService } from "./services/dashboard.service";
// AnalyticsService removed completely
import { PackagesService } from "./services/packages.service";
import { ClinicInfoService } from "./services/clinic-info.service";
import { AppointmentReportsService } from "./services/appointment-reports.service";
import { 
  insertClientSchema, 
  insertServiceSchema, 
  insertAppointmentSchema,
  insertInventorySchema,

  insertFinancialTransactionSchema
} from "@shared/schema";

export async function registerRoutes(app: Express): Promise<Server> {
  // Configurar autenticação
  const { authMiddleware, adminMiddleware } = setupAuth(app);

  // Inicializar serviços seguindo padrão SOLID
  const dashboardService = new DashboardService(storage);
  // AnalyticsService removed completely
  const packagesService = new PackagesService(storage);
  const clinicInfoService = new ClinicInfoService(storage);
  const appointmentReportsService = new AppointmentReportsService(storage);
  // Client routes
  app.get("/api/clients", authMiddleware, async (req: Request, res: Response) => {
    try {
      const clients = await storage.getClients();
      res.json(clients);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch clients" });
    }
  });

  app.get("/api/clients/:id", async (req: Request, res: Response) => {
    try {
      const client = await storage.getClient(Number(req.params.id));
      if (!client) {
        return res.status(404).json({ message: "Client not found" });
      }
      res.json(client);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch client" });
    }
  });

  app.post("/api/clients", async (req: Request, res: Response) => {
    try {
      const validatedData = insertClientSchema.parse(req.body);
      const client = await storage.createClient(validatedData);
      res.status(201).json(client);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to create client" });
    }
  });

  app.put("/api/clients/:id", async (req: Request, res: Response) => {
    try {
      const validatedData = insertClientSchema.partial().parse(req.body);
      const client = await storage.updateClient(Number(req.params.id), validatedData);
      if (!client) {
        return res.status(404).json({ message: "Client not found" });
      }
      res.json(client);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to update client" });
    }
  });

  app.delete("/api/clients/:id", async (req: Request, res: Response) => {
    try {
      const success = await storage.deleteClient(Number(req.params.id));
      if (!success) {
        return res.status(404).json({ message: "Client not found" });
      }
      res.status(204).end();
    } catch (error) {
      res.status(500).json({ message: "Failed to delete client" });
    }
  });

  // Service routes
  app.get("/api/services", async (req: Request, res: Response) => {
    try {
      const services = await storage.getServices();
      res.json(services);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch services" });
    }
  });

  app.get("/api/services/:id", async (req: Request, res: Response) => {
    try {
      const service = await storage.getService(Number(req.params.id));
      if (!service) {
        return res.status(404).json({ message: "Service not found" });
      }
      res.json(service);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch service" });
    }
  });

  app.post("/api/services", async (req: Request, res: Response) => {
    try {
      const validatedData = insertServiceSchema.parse(req.body);
      const service = await storage.createService(validatedData);
      res.status(201).json(service);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to create service" });
    }
  });

  app.put("/api/services/:id", async (req: Request, res: Response) => {
    try {
      const validatedData = insertServiceSchema.partial().parse(req.body);
      const service = await storage.updateService(Number(req.params.id), validatedData);
      if (!service) {
        return res.status(404).json({ message: "Service not found" });
      }
      res.json(service);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to update service" });
    }
  });

  app.delete("/api/services/:id", async (req: Request, res: Response) => {
    try {
      const success = await storage.deleteService(Number(req.params.id));
      if (!success) {
        return res.status(404).json({ message: "Service not found" });
      }
      res.status(204).end();
    } catch (error) {
      res.status(500).json({ message: "Failed to delete service" });
    }
  });

  // Staff routes
  app.get("/api/staff", async (req: Request, res: Response) => {
    try {
      const staffMembers = await storage.getStaffMembers();
      // Get full user details for each staff member
      const staffWithDetails = await Promise.all(
        staffMembers.map(async (staff) => {
          const user = await storage.getUser(staff.userId);
          return { ...staff, user };
        })
      );
      res.json(staffWithDetails);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch staff members" });
    }
  });

  // Appointment routes
  app.get("/api/appointments", async (req: Request, res: Response) => {
    try {
      const { start, end } = req.query;
      let appointments;
      if (start && end) {
        appointments = await storage.getAppointmentsByDateRange(
          new Date(start as string),
          new Date(end as string)
        );
      } else {
        appointments = await storage.getAppointments();
      }
      res.json(appointments);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch appointments" });
    }
  });

  app.get("/api/appointments/:id", async (req: Request, res: Response) => {
    try {
      const appointment = await storage.getAppointment(Number(req.params.id));
      if (!appointment) {
        return res.status(404).json({ message: "Appointment not found" });
      }
      res.json(appointment);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch appointment" });
    }
  });

  app.post("/api/appointments", async (req: Request, res: Response) => {
    try {
      const validatedData = insertAppointmentSchema.parse(req.body);
      const appointment = await storage.createAppointment(validatedData);
      res.status(201).json(appointment);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to create appointment" });
    }
  });

  app.put("/api/appointments/:id", async (req: Request, res: Response) => {
    try {
      const validatedData = insertAppointmentSchema.partial().parse(req.body);
      const appointment = await storage.updateAppointment(Number(req.params.id), validatedData);
      if (!appointment) {
        return res.status(404).json({ message: "Appointment not found" });
      }
      res.json(appointment);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to update appointment" });
    }
  });

  app.delete("/api/appointments/:id", async (req: Request, res: Response) => {
    try {
      const success = await storage.deleteAppointment(Number(req.params.id));
      if (!success) {
        return res.status(404).json({ message: "Appointment not found" });
      }
      res.status(204).end();
    } catch (error) {
      res.status(500).json({ message: "Failed to delete appointment" });
    }
  });

  // Inventory routes
  app.get("/api/inventory", async (req: Request, res: Response) => {
    try {
      const inventoryItems = await storage.getInventoryItems();
      res.json(inventoryItems);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch inventory items" });
    }
  });

  app.get("/api/inventory/:id", async (req: Request, res: Response) => {
    try {
      const item = await storage.getInventoryItem(Number(req.params.id));
      if (!item) {
        return res.status(404).json({ message: "Inventory item not found" });
      }
      res.json(item);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch inventory item" });
    }
  });

  app.post("/api/inventory", async (req: Request, res: Response) => {
    try {
      const validatedData = insertInventorySchema.parse(req.body);
      const item = await storage.createInventoryItem(validatedData);
      res.status(201).json(item);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to create inventory item" });
    }
  });

  app.put("/api/inventory/:id", async (req: Request, res: Response) => {
    try {
      const validatedData = insertInventorySchema.partial().parse(req.body);
      const item = await storage.updateInventoryItem(Number(req.params.id), validatedData);
      if (!item) {
        return res.status(404).json({ message: "Inventory item not found" });
      }
      res.json(item);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to update inventory item" });
    }
  });

  app.delete("/api/inventory/:id", async (req: Request, res: Response) => {
    try {
      const success = await storage.deleteInventoryItem(Number(req.params.id));
      if (!success) {
        return res.status(404).json({ message: "Inventory item not found" });
      }
      res.status(204).end();
    } catch (error) {
      res.status(500).json({ message: "Failed to delete inventory item" });
    }
  });

  // Financial Transaction routes
  app.get("/api/financial-transactions", authMiddleware, async (req: Request, res: Response) => {
    try {
      const { start, end } = req.query;
      let transactions;
      if (start && end) {
        transactions = await storage.getFinancialTransactionsByDateRange(
          new Date(start as string),
          new Date(end as string)
        );
      } else {
        transactions = await storage.getFinancialTransactions();
      }
      res.json(transactions);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch financial transactions" });
    }
  });

  app.post("/api/financial-transactions", authMiddleware, adminMiddleware, async (req: Request, res: Response) => {
    try {
      const validatedData = insertFinancialTransactionSchema.parse(req.body);
      const transaction = await storage.createFinancialTransaction(validatedData);
      res.status(201).json(transaction);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to create financial transaction" });
    }
  });

  // Chat template routes removed completely

  // All remaining chat template routes removed

  // AI chat endpoint removed completely

  // Endpoint para relatórios de agendamentos com filtros COMPLETOS
  app.get("/api/appointment-reports", authMiddleware, async (req: Request, res: Response) => {
    try {
      const { 
        startDate, 
        endDate, 
        status, 
        professionalId, 
        clientId, 
        convenio,
        sala,
        page = 1, 
        limit = 25 
      } = req.query;

      // Preparar filtros para o serviço
      const filters = {
        startDate: startDate as string,
        endDate: endDate as string,
        status: status ? (Array.isArray(status) ? status as string[] : [status as string]) : undefined,
        professionalId: professionalId ? Number(professionalId) : undefined,
        clientId: clientId ? Number(clientId) : undefined,
        convenio: convenio as string,
        sala: sala as string,
        page: Number(page),
        limit: Number(limit)
      };

      // Usar o serviço robusto para buscar dados completos
      const reportData = await appointmentReportsService.getAppointmentReports(filters);

      res.json(reportData);

    } catch (error) {
      console.error("Erro ao buscar relatórios de agendamentos:", error);
      res.status(500).json({ error: "Falha ao buscar relatórios de agendamentos" });
    }
  });

  // Endpoint para buscar opções de filtros
  app.get("/api/filter-options", authMiddleware, async (req: Request, res: Response) => {
    try {
      const clients = await storage.getClients();
      const staff = await storage.getStaffMembers();
      
      const convenios = [
        "Porto Seguro", "SulAmérica", "Bradesco Saúde", "Unimed", "Amil", 
        "NotreDame Intermédica", "Hapvida", "Prevent Senior", "São Cristóvão",
        "Golden Cross", "Allianz Saúde", "Particular"
      ];

      const statusOptions = [
        { key: "scheduled", label: "Agendado" },
        { key: "confirmed", label: "Confirmado" },
        { key: "rescheduled", label: "Remarcado" },
        { key: "completed", label: "Concluído" },
        { key: "cancelled", label: "Cancelado" },
        { key: "no_show", label: "Não compareceram" }
      ];

      res.json({
        clients: clients.map(c => ({
          id: c.id,
          fullName: c.fullName,
          email: c.email
        })),
        professionals: staff.map(s => ({
          id: s.id,
          specialization: s.specialization
        })),
        convenios,
        statusOptions
      });

    } catch (error) {
      console.error("Error fetching filter options:", error);
      res.status(500).json({ error: "Internal server error" });
    }
  });

  // ===========================================
  // ENDPOINTS PARA DASHBOARD - Seguindo SOLID
  // ===========================================
  app.get("/api/dashboard/metrics", authMiddleware, async (req: Request, res: Response) => {
    try {
      const metrics = await dashboardService.getDashboardMetrics();
      res.json(metrics);
    } catch (error) {
      console.error("Erro ao buscar métricas do dashboard:", error);
      res.status(500).json({ error: "Falha ao buscar métricas do dashboard" });
    }
  });

  // Analytics endpoints removed completely

  // ===========================================
  // ENDPOINTS PARA PACOTES - Seguindo SOLID
  // ===========================================
  app.get("/api/packages", authMiddleware, async (req: Request, res: Response) => {
    try {
      const packages = await packagesService.getAllPackages();
      res.json(packages);
    } catch (error) {
      console.error("Erro ao buscar pacotes:", error);
      res.status(500).json({ error: "Falha ao buscar pacotes" });
    }
  });

  app.get("/api/packages/:id", authMiddleware, async (req: Request, res: Response) => {
    try {
      const packageData = await packagesService.getPackageById(Number(req.params.id));
      if (!packageData) {
        return res.status(404).json({ error: "Pacote não encontrado" });
      }
      res.json(packageData);
    } catch (error) {
      console.error("Erro ao buscar pacote:", error);
      res.status(500).json({ error: "Falha ao buscar pacote" });
    }
  });

  app.post("/api/packages", authMiddleware, adminMiddleware, async (req: Request, res: Response) => {
    try {
      const newPackage = await packagesService.createPackage(req.body);
      res.status(201).json(newPackage);
    } catch (error) {
      console.error("Erro ao criar pacote:", error);
      res.status(500).json({ error: "Falha ao criar pacote" });
    }
  });

  app.put("/api/packages/:id", authMiddleware, adminMiddleware, async (req: Request, res: Response) => {
    try {
      const updatedPackage = await packagesService.updatePackage(Number(req.params.id), req.body);
      if (!updatedPackage) {
        return res.status(404).json({ error: "Pacote não encontrado" });
      }
      res.json(updatedPackage);
    } catch (error) {
      console.error("Erro ao atualizar pacote:", error);
      res.status(500).json({ error: "Falha ao atualizar pacote" });
    }
  });

  app.delete("/api/packages/:id", authMiddleware, adminMiddleware, async (req: Request, res: Response) => {
    try {
      const success = await packagesService.deletePackage(Number(req.params.id));
      if (!success) {
        return res.status(404).json({ error: "Pacote não encontrado" });
      }
      res.status(204).end();
    } catch (error) {
      console.error("Erro ao deletar pacote:", error);
      res.status(500).json({ error: "Falha ao deletar pacote" });
    }
  });

  app.get("/api/packages/stats", authMiddleware, async (req: Request, res: Response) => {
    try {
      const stats = await packagesService.getPackageStats();
      res.json(stats);
    } catch (error) {
      console.error("Erro ao buscar estatísticas de pacotes:", error);
      res.status(500).json({ error: "Falha ao buscar estatísticas de pacotes" });
    }
  });

  // ===========================================
  // ENDPOINTS PARA DADOS DA CLÍNICA - Seguindo SOLID
  // ===========================================
  app.get("/api/clinic-info", authMiddleware, async (req: Request, res: Response) => {
    try {
      const clinicInfo = await clinicInfoService.getClinicInfo();
      res.json(clinicInfo);
    } catch (error) {
      console.error("Erro ao buscar informações da clínica:", error);
      res.status(500).json({ error: "Falha ao buscar informações da clínica" });
    }
  });

  app.put("/api/clinic-info", authMiddleware, adminMiddleware, async (req: Request, res: Response) => {
    try {
      const updatedInfo = await clinicInfoService.updateClinicInfo(req.body);
      res.json(updatedInfo);
    } catch (error) {
      console.error("Erro ao atualizar informações da clínica:", error);
      res.status(500).json({ error: "Falha ao atualizar informações da clínica" });
    }
  });

  app.get("/api/clinic-info/stats", authMiddleware, async (req: Request, res: Response) => {
    try {
      const stats = await clinicInfoService.getClinicStats();
      res.json(stats);
    } catch (error) {
      console.error("Erro ao buscar estatísticas da clínica:", error);
      res.status(500).json({ error: "Falha ao buscar estatísticas da clínica" });
    }
  });

  app.post("/api/clinic-info/logo", authMiddleware, adminMiddleware, async (req: Request, res: Response) => {
    try {
      const { logoData } = req.body;
      const logoUrl = await clinicInfoService.uploadLogo(logoData);
      res.json({ logoUrl });
    } catch (error) {
      console.error("Erro ao fazer upload do logo:", error);
      res.status(500).json({ error: "Falha ao fazer upload do logo" });
    }
  });

  // ===========================================
  // ENDPOINTS PARA ASSINATURAS - Seguindo SOLID
  // ===========================================
  app.get("/api/subscriptions", authMiddleware, async (req: Request, res: Response) => {
    try {
      // Dados estruturados para assinaturas
      const subscriptions = [
        {
          id: 1,
          name: "Plano Básico",
          price: 99.99,
          interval: "monthly",
          features: ["Até 500 pacientes", "Agendamentos ilimitados", "Suporte por email"],
          isActive: true,
          currentPlan: true
        },
        {
          id: 2,
          name: "Plano Premium",
          price: 199.99,
          interval: "monthly",
          features: ["Pacientes ilimitados", "Analytics avançados", "Suporte prioritário", "Backup automático"],
          isActive: true,
          currentPlan: false
        }
      ];
      res.json(subscriptions);
    } catch (error) {
      console.error("Erro ao buscar assinaturas:", error);
      res.status(500).json({ error: "Falha ao buscar assinaturas" });
    }
  });

  // ===========================================
  // ENDPOINTS PARA ANTES & DEPOIS - Seguindo SOLID
  // ===========================================
  app.get("/api/before-after", authMiddleware, async (req: Request, res: Response) => {
    try {
      // Dados estruturados para antes & depois
      const beforeAfterCases = [
        {
          id: 1,
          clientId: 1,
          serviceId: 1,
          title: "Harmonização Facial Completa",
          description: "Tratamento de harmonização com botox e preenchimento",
          beforeImage: "/uploads/before-1.jpg",
          afterImage: "/uploads/after-1.jpg",
          treatmentDate: new Date("2024-12-01"),
          isPublic: true,
          createdAt: new Date()
        }
      ];
      res.json(beforeAfterCases);
    } catch (error) {
      console.error("Erro ao buscar casos antes & depois:", error);
      res.status(500).json({ error: "Falha ao buscar casos antes & depois" });
    }
  });

  app.post("/api/before-after", authMiddleware, adminMiddleware, async (req: Request, res: Response) => {
    try {
      const newCase = {
        id: Date.now(),
        ...req.body,
        createdAt: new Date()
      };
      res.status(201).json(newCase);
    } catch (error) {
      console.error("Erro ao criar caso antes & depois:", error);
      res.status(500).json({ error: "Falha ao criar caso antes & depois" });
    }
  });

  // Initialize the HTTP server
  const httpServer = createServer(app);
  return httpServer;
}
