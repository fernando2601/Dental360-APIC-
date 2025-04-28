import passport from "passport";
import { Strategy as LocalStrategy } from "passport-local";
import { Express, Request, Response, NextFunction } from "express";
import session from "express-session";
import { scrypt, randomBytes, timingSafeEqual } from "crypto";
import { promisify } from "util";
import { storage } from "./storage";
import { User, insertUserSchema } from "@shared/schema";
import { z } from "zod";
import createMemoryStore from "memorystore";
import { v4 as uuidv4 } from "uuid";

const MemoryStore = createMemoryStore(session);

// Estendendo o namespace Express.User com nosso tipo User
declare global {
  namespace Express {
    interface User extends User {}
  }
}

// Promisificar o scrypt para usar com async/await
const scryptAsync = promisify(scrypt);

// Função para gerar hash de senha
export async function hashPassword(password: string): Promise<string> {
  const salt = randomBytes(16).toString("hex");
  const buf = (await scryptAsync(password, salt, 64)) as Buffer;
  return `${buf.toString("hex")}.${salt}`;
}

// Função para comparar senha fornecida com a armazenada
export async function comparePasswords(supplied: string, stored: string): Promise<boolean> {
  const [hashed, salt] = stored.split(".");
  const hashedBuf = Buffer.from(hashed, "hex");
  const suppliedBuf = (await scryptAsync(supplied, salt, 64)) as Buffer;
  return timingSafeEqual(hashedBuf, suppliedBuf);
}

// Função para configurar a autenticação no Express
export function setupAuth(app: Express) {
  const sessionSecret = process.env.SESSION_SECRET || randomBytes(32).toString("hex");

  // Configurações da sessão
  const sessionSettings: session.SessionOptions = {
    secret: sessionSecret,
    resave: false,
    saveUninitialized: false,
    cookie: {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      maxAge: 24 * 60 * 60 * 1000, // 24 horas
    },
    store: new MemoryStore({
      checkPeriod: 86400000, // Limpar sessões expiradas a cada 24h
    }),
  };

  // Configurar middleware de sessão
  app.set("trust proxy", 1);
  app.use(session(sessionSettings));
  app.use(passport.initialize());
  app.use(passport.session());

  // Configurar estratégia de autenticação local
  passport.use(
    new LocalStrategy(async (username, password, done) => {
      try {
        const user = await storage.getUserByUsername(username);
        if (!user || !(await comparePasswords(password, user.password))) {
          return done(null, false);
        }
        
        // Atualizar último login
        await storage.updateUserLastLogin(user.id);
        
        return done(null, user);
      } catch (error) {
        return done(error);
      }
    })
  );

  // Serialização e deserialização do usuário
  passport.serializeUser((user, done) => {
    done(null, user.id);
  });

  passport.deserializeUser(async (id: number, done) => {
    try {
      const user = await storage.getUser(id);
      done(null, user);
    } catch (error) {
      done(error);
    }
  });

  // Middleware para verificar autenticação
  const authMiddleware = (req: Request, res: Response, next: NextFunction) => {
    if (!req.isAuthenticated()) {
      return res.status(401).json({ message: "Não autenticado" });
    }
    next();
  };

  // Middleware para verificar se é admin
  const adminMiddleware = (req: Request, res: Response, next: NextFunction) => {
    if (!req.isAuthenticated() || req.user.role !== "admin") {
      return res.status(403).json({ message: "Acesso negado: privilégios de administrador necessários" });
    }
    next();
  };

  // Schema para validação de registro
  const registerSchema = z.object({
    username: z.string().min(3, "Nome de usuário deve ter pelo menos 3 caracteres"),
    password: z
      .string()
      .min(8, "Senha deve ter pelo menos 8 caracteres")
      .regex(/[A-Z]/, "Senha deve conter pelo menos uma letra maiúscula")
      .regex(/[a-z]/, "Senha deve conter pelo menos uma letra minúscula")
      .regex(/[0-9]/, "Senha deve conter pelo menos um número")
      .regex(/[^A-Za-z0-9]/, "Senha deve conter pelo menos um caractere especial"),
    fullName: z.string().min(3, "Nome completo deve ter pelo menos 3 caracteres"),
    email: z.string().email("Email inválido"),
    phone: z.string().optional(),
    role: z.enum(["admin", "staff"]).default("staff"),
  });

  const loginSchema = z.object({
    username: z.string().min(1, "Nome de usuário é obrigatório"),
    password: z.string().min(1, "Senha é obrigatória"),
  });

  const resetRequestSchema = z.object({
    email: z.string().email("Email inválido"),
  });

  const resetPasswordSchema = z.object({
    token: z.string().min(1, "Token é obrigatório"),
    password: z
      .string()
      .min(8, "Senha deve ter pelo menos 8 caracteres")
      .regex(/[A-Z]/, "Senha deve conter pelo menos uma letra maiúscula")
      .regex(/[a-z]/, "Senha deve conter pelo menos uma letra minúscula")
      .regex(/[0-9]/, "Senha deve conter pelo menos um número")
      .regex(/[^A-Za-z0-9]/, "Senha deve conter pelo menos um caractere especial"),
  });

  // Rota de registro
  app.post("/api/register", async (req: Request, res: Response) => {
    try {
      const validatedData = registerSchema.parse(req.body);
      
      // Verificar se usuário já existe
      const existingUsername = await storage.getUserByUsername(validatedData.username);
      if (existingUsername) {
        return res.status(400).json({ message: "Nome de usuário já está em uso" });
      }
      
      // Verificar se email já existe
      const existingEmail = await storage.getUserByEmail(validatedData.email);
      if (existingEmail) {
        return res.status(400).json({ message: "Email já está em uso" });
      }
      
      // Criar usuário
      const hashedPassword = await hashPassword(validatedData.password);
      const user = await storage.createUser({
        ...validatedData,
        password: hashedPassword,
      });
      
      // Autenticar novo usuário
      req.login(user, (err) => {
        if (err) {
          return res.status(500).json({ message: "Erro ao autenticar após registro" });
        }
        return res.status(201).json({
          id: user.id,
          username: user.username,
          fullName: user.fullName,
          email: user.email,
          role: user.role,
        });
      });
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ 
          message: "Dados de registro inválidos", 
          errors: error.errors.map(err => ({ path: err.path.join('.'), message: err.message })) 
        });
      }
      return res.status(500).json({ message: "Erro ao registrar usuário" });
    }
  });

  // Rota de login
  app.post("/api/login", (req: Request, res: Response, next: NextFunction) => {
    try {
      const validatedData = loginSchema.parse(req.body);
      
      passport.authenticate("local", (err: any, user: any, info: any) => {
        if (err) {
          return next(err);
        }
        if (!user) {
          return res.status(401).json({ message: "Credenciais inválidas" });
        }
        
        req.login(user, (loginErr) => {
          if (loginErr) {
            return next(loginErr);
          }
          return res.status(200).json({
            id: user.id,
            username: user.username,
            fullName: user.fullName,
            email: user.email,
            role: user.role,
          });
        });
      })(req, res, next);
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ 
          message: "Dados de login inválidos", 
          errors: error.errors.map(err => ({ path: err.path.join('.'), message: err.message })) 
        });
      }
      return res.status(500).json({ message: "Erro ao processar login" });
    }
  });

  // Rota de logout
  app.post("/api/logout", (req: Request, res: Response) => {
    req.logout((err) => {
      if (err) {
        return res.status(500).json({ message: "Erro ao fazer logout" });
      }
      res.status(200).json({ message: "Logout realizado com sucesso" });
    });
  });

  // Rota para obter usuário atual
  app.get("/api/me", authMiddleware, (req: Request, res: Response) => {
    res.status(200).json({
      id: req.user.id,
      username: req.user.username,
      fullName: req.user.fullName,
      email: req.user.email,
      role: req.user.role,
    });
  });

  // Rota para solicitar redefinição de senha
  app.post("/api/reset-password-request", async (req: Request, res: Response) => {
    try {
      const { email } = resetRequestSchema.parse(req.body);
      
      // Verificar se o email existe
      const user = await storage.getUserByEmail(email);
      if (!user) {
        // Por segurança, não informamos se o email existe ou não
        return res.status(200).json({ message: "Se o email estiver cadastrado, você receberá instruções para redefinir sua senha" });
      }
      
      // Gerar token
      const resetToken = uuidv4();
      const resetTokenExpiry = new Date();
      resetTokenExpiry.setHours(resetTokenExpiry.getHours() + 1); // Expira em 1 hora
      
      // Salvar token no usuário
      await storage.updateUserResetToken(user.id, resetToken, resetTokenExpiry);
      
      // Em um aplicativo real, enviaríamos um email com o token
      // Para fins de demonstração, retornamos o token na resposta
      console.log(`Token de redefinição para ${email}: ${resetToken}`);
      
      return res.status(200).json({ 
        message: "Se o email estiver cadastrado, você receberá instruções para redefinir sua senha",
        // Apenas para desenvolvimento
        _devToken: process.env.NODE_ENV === "development" ? resetToken : undefined
      });
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ 
          message: "Email inválido", 
          errors: error.errors.map(err => ({ path: err.path.join('.'), message: err.message })) 
        });
      }
      return res.status(500).json({ message: "Erro ao processar solicitação de redefinição de senha" });
    }
  });

  // Rota para redefinir senha com token
  app.post("/api/reset-password", async (req: Request, res: Response) => {
    try {
      const { token, password } = resetPasswordSchema.parse(req.body);
      
      // Buscar usuário com este token e verificar se o token é válido
      const user = await storage.getUserByResetToken(token);
      if (!user || !user.resetTokenExpiry || new Date(user.resetTokenExpiry) < new Date()) {
        return res.status(400).json({ message: "Token inválido ou expirado" });
      }
      
      // Atualizar senha e limpar token
      const hashedPassword = await hashPassword(password);
      await storage.updateUserPassword(user.id, hashedPassword);
      await storage.clearUserResetToken(user.id);
      
      return res.status(200).json({ message: "Senha redefinida com sucesso" });
    } catch (error) {
      if (error instanceof z.ZodError) {
        return res.status(400).json({ 
          message: "Dados inválidos", 
          errors: error.errors.map(err => ({ path: err.path.join('.'), message: err.message })) 
        });
      }
      return res.status(500).json({ message: "Erro ao redefinir senha" });
    }
  });

  // Exportando middlewares para uso em outras partes da aplicação
  return {
    authMiddleware,
    adminMiddleware,
  };
}