import { pgTable, text, serial, integer, boolean, timestamp, date, decimal } from "drizzle-orm/pg-core";
import { createInsertSchema } from "drizzle-zod";
import { z } from "zod";

// Users
export const users = pgTable("users", {
  id: serial("id").primaryKey(),
  username: text("username").notNull().unique(),
  password: text("password").notNull(),
  fullName: text("fullName").notNull(),
  role: text("role").notNull().default("staff"), // admin, staff
  email: text("email").notNull().unique(),
  phone: text("phone"),
  resetToken: text("resetToken"),
  resetTokenExpiry: timestamp("resetTokenExpiry"),
  lastLogin: timestamp("lastLogin"),
  createdAt: timestamp("createdAt").defaultNow(),
});

export const insertUserSchema = createInsertSchema(users).pick({
  username: true,
  password: true,
  fullName: true,
  role: true,
  email: true,
  phone: true,
});

// Clients
export const clients = pgTable("clients", {
  id: serial("id").primaryKey(),
  fullName: text("fullName").notNull(),
  email: text("email").notNull(),
  phone: text("phone").notNull(),
  address: text("address"),
  birthday: date("birthday"),
  notes: text("notes"),
  createdAt: timestamp("createdAt").defaultNow(),
});

export const insertClientSchema = createInsertSchema(clients).pick({
  fullName: true,
  email: true,
  phone: true,
  address: true,
  birthday: true,
  notes: true,
});

// Services
export const services = pgTable("services", {
  id: serial("id").primaryKey(),
  name: text("name").notNull(),
  category: text("category").notNull(),
  description: text("description").notNull(),
  duration: integer("duration").notNull(), // in minutes
  price: decimal("price", { precision: 10, scale: 2 }).notNull(),
  active: boolean("active").notNull().default(true),
});

export const insertServiceSchema = createInsertSchema(services).pick({
  name: true,
  category: true,
  description: true,
  duration: true,
  price: true,
  active: true,
});

// Staff
export const staff = pgTable("staff", {
  id: serial("id").primaryKey(),
  userId: integer("userId").notNull(), // Reference to users table
  specialization: text("specialization").notNull(),
  bio: text("bio"),
  available: boolean("available").notNull().default(true),
});

export const insertStaffSchema = createInsertSchema(staff).pick({
  userId: true,
  specialization: true,
  bio: true,
  available: true,
});

// Appointments
export const appointments = pgTable("appointments", {
  id: serial("id").primaryKey(),
  clientId: integer("clientId").notNull(), // Reference to clients table
  staffId: integer("staffId").notNull(), // Reference to staff table
  serviceId: integer("serviceId").notNull(), // Reference to services table
  startTime: timestamp("startTime").notNull(),
  endTime: timestamp("endTime").notNull(),
  status: text("status").notNull().default("scheduled"), // scheduled, completed, cancelled, no-show
  notes: text("notes"),
  createdAt: timestamp("createdAt").defaultNow(),
});

export const insertAppointmentSchema = createInsertSchema(appointments).pick({
  clientId: true,
  staffId: true,
  serviceId: true,
  startTime: true,
  endTime: true,
  status: true,
  notes: true,
});

// Inventory
export const inventory = pgTable("inventory", {
  id: serial("id").primaryKey(),
  name: text("name").notNull(),
  category: text("category").notNull(),
  description: text("description"),
  quantity: integer("quantity").notNull().default(0),
  unit: text("unit").notNull(), // e.g., ml, syringe, etc.
  threshold: integer("threshold"), // Minimum quantity before reordering
  price: decimal("price", { precision: 10, scale: 2 }),
  lastRestocked: timestamp("lastRestocked"),
});

export const insertInventorySchema = createInsertSchema(inventory).pick({
  name: true,
  category: true,
  description: true,
  quantity: true,
  unit: true,
  threshold: true,
  price: true,
  lastRestocked: true,
});

// Financial Transactions
export const financialTransactions = pgTable("financialTransactions", {
  id: serial("id").primaryKey(),
  appointmentId: integer("appointmentId"), // Can be null for non-appointment transactions
  clientId: integer("clientId"), // Can be null for non-client transactions
  type: text("type").notNull(), // income, expense
  category: text("category").notNull(), // service, product, rent, salary, etc.
  amount: decimal("amount", { precision: 10, scale: 2 }).notNull(),
  date: timestamp("date").notNull(),
  description: text("description"),
  paymentMethod: text("paymentMethod"), // cash, credit card, etc.
});

export const insertFinancialTransactionSchema = createInsertSchema(financialTransactions).pick({
  appointmentId: true,
  clientId: true,
  type: true,
  category: true,
  amount: true,
  date: true,
  description: true,
  paymentMethod: true,
});

// Chat templates removed as requested

// Types
export type User = typeof users.$inferSelect;
export type InsertUser = z.infer<typeof insertUserSchema>;

export type Client = typeof clients.$inferSelect;
export type InsertClient = z.infer<typeof insertClientSchema>;

export type Service = typeof services.$inferSelect;
export type InsertService = z.infer<typeof insertServiceSchema>;

export type Staff = typeof staff.$inferSelect;
export type InsertStaff = z.infer<typeof insertStaffSchema>;

export type Appointment = typeof appointments.$inferSelect;
export type InsertAppointment = z.infer<typeof insertAppointmentSchema>;

export type Inventory = typeof inventory.$inferSelect;
export type InsertInventory = z.infer<typeof insertInventorySchema>;

export type FinancialTransaction = typeof financialTransactions.$inferSelect;
export type InsertFinancialTransaction = z.infer<typeof insertFinancialTransactionSchema>;

// ChatTemplate types removed
