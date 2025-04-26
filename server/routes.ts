import type { Express, Request, Response } from "express";
import { createServer, type Server } from "http";
import { storage } from "./storage";
import { z } from "zod";
import { 
  insertClientSchema, 
  insertServiceSchema, 
  insertAppointmentSchema,
  insertInventorySchema,
  insertChatTemplateSchema,
  insertFinancialTransactionSchema
} from "@shared/schema";

export async function registerRoutes(app: Express): Promise<Server> {
  // Client routes
  app.get("/api/clients", async (req: Request, res: Response) => {
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
  app.get("/api/financial-transactions", async (req: Request, res: Response) => {
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

  app.post("/api/financial-transactions", async (req: Request, res: Response) => {
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

  // Chat Template routes
  app.get("/api/chat-templates", async (req: Request, res: Response) => {
    try {
      const { category } = req.query;
      let templates;
      if (category) {
        templates = await storage.getChatTemplatesByCategory(category as string);
      } else {
        templates = await storage.getChatTemplates();
      }
      res.json(templates);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch chat templates" });
    }
  });

  app.get("/api/chat-templates/:id", async (req: Request, res: Response) => {
    try {
      const template = await storage.getChatTemplate(Number(req.params.id));
      if (!template) {
        return res.status(404).json({ message: "Chat template not found" });
      }
      res.json(template);
    } catch (error) {
      res.status(500).json({ message: "Failed to fetch chat template" });
    }
  });

  app.post("/api/chat-templates", async (req: Request, res: Response) => {
    try {
      const validatedData = insertChatTemplateSchema.parse(req.body);
      const template = await storage.createChatTemplate(validatedData);
      res.status(201).json(template);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to create chat template" });
    }
  });

  app.put("/api/chat-templates/:id", async (req: Request, res: Response) => {
    try {
      const validatedData = insertChatTemplateSchema.partial().parse(req.body);
      const template = await storage.updateChatTemplate(Number(req.params.id), validatedData);
      if (!template) {
        return res.status(404).json({ message: "Chat template not found" });
      }
      res.json(template);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ errors: error.errors });
      }
      res.status(500).json({ message: "Failed to update chat template" });
    }
  });

  app.post("/api/chat-templates/:id/use", async (req: Request, res: Response) => {
    try {
      const template = await storage.incrementChatTemplateUsage(Number(req.params.id));
      if (!template) {
        return res.status(404).json({ message: "Chat template not found" });
      }
      res.json(template);
    } catch (error) {
      res.status(500).json({ message: "Failed to increment chat template usage" });
    }
  });

  app.delete("/api/chat-templates/:id", async (req: Request, res: Response) => {
    try {
      const success = await storage.deleteChatTemplate(Number(req.params.id));
      if (!success) {
        return res.status(404).json({ message: "Chat template not found" });
      }
      res.status(204).end();
    } catch (error) {
      res.status(500).json({ message: "Failed to delete chat template" });
    }
  });

  // Simple AI chat endpoint that matches queries to templates
  app.post("/api/ai-chat", async (req: Request, res: Response) => {
    try {
      const { query } = req.body;
      if (!query) {
        return res.status(400).json({ message: "Query is required" });
      }

      const templates = await storage.getChatTemplates();
      
      // Simple keyword matching algorithm
      let bestMatch: { template: any; score: number } | null = null;
      
      templates.forEach(template => {
        if (!template.active) return;
        
        // Simple scoring based on keyword matches
        const keywords = template.title.toLowerCase().split(' ')
          .concat(template.category.toLowerCase().split(' '))
          .concat(template.content.toLowerCase().split(' '))
          .filter(word => word.length > 3); // Filter out short words
        
        const queryLower = query.toLowerCase();
        let matchScore = 0;
        
        keywords.forEach(keyword => {
          if (queryLower.includes(keyword)) {
            matchScore += 1;
          }
        });
        
        if (!bestMatch || matchScore > bestMatch.score) {
          bestMatch = { template, score: matchScore };
        }
      });
      
      if (bestMatch && bestMatch.score > 0) {
        // Calculate a confidence percentage
        const confidence = Math.min(bestMatch.score * 10, 100);
        
        // Increment the usage count
        await storage.incrementChatTemplateUsage(bestMatch.template.id);
        
        return res.json({
          response: bestMatch.template.content,
          confidence,
          template: bestMatch.template
        });
      }
      
      // No good match found
      res.json({
        response: "I'm sorry, I don't have specific information about that. Would you like to speak with one of our staff members for more details?",
        confidence: 0,
        template: null
      });
    } catch (error) {
      res.status(500).json({ message: "Failed to process AI chat request" });
    }
  });

  // Initialize the HTTP server
  const httpServer = createServer(app);
  return httpServer;
}
