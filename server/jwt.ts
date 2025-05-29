import jwt from "jsonwebtoken";
import { Request, Response, NextFunction } from "express";
import { User } from "@shared/schema";

// Chave secreta para assinar tokens JWT
const JWT_SECRET = process.env.JWT_SECRET || "dental-spa-secret-key-2024";
const JWT_EXPIRES_IN = process.env.JWT_EXPIRES_IN || "24h";

// Interface para payload do JWT
export interface JWTPayload {
  userId: number;
  username: string;
  role: string;
  email: string;
}

// Gerar token JWT
export function generateToken(user: User): string {
  const payload: JWTPayload = {
    userId: user.id,
    username: user.username,
    role: user.role,
    email: user.email
  };

  return jwt.sign(payload, JWT_SECRET, { expiresIn: JWT_EXPIRES_IN });
}

// Verificar token JWT
export function verifyToken(token: string): JWTPayload | null {
  try {
    return jwt.verify(token, JWT_SECRET) as JWTPayload;
  } catch (error) {
    return null;
  }
}

// Middleware de autenticação JWT
export function jwtAuthMiddleware(req: Request, res: Response, next: NextFunction) {
  const authHeader = req.headers.authorization;
  
  if (!authHeader || !authHeader.startsWith('Bearer ')) {
    return res.status(401).json({ message: "Token de acesso requerido" });
  }

  const token = authHeader.substring(7); // Remove 'Bearer '
  const payload = verifyToken(token);

  if (!payload) {
    return res.status(401).json({ message: "Token inválido ou expirado" });
  }

  // Adicionar dados do usuário ao request
  (req as any).user = payload;
  next();
}

// Middleware para verificar papel de administrador
export function jwtAdminMiddleware(req: Request, res: Response, next: NextFunction) {
  const user = (req as any).user;
  
  if (!user || user.role !== 'admin') {
    return res.status(403).json({ message: "Acesso negado. Permissões de administrador requeridas" });
  }
  
  next();
}

// Middleware para extrair token (opcional)
export function jwtOptionalMiddleware(req: Request, res: Response, next: NextFunction) {
  const authHeader = req.headers.authorization;
  
  if (authHeader && authHeader.startsWith('Bearer ')) {
    const token = authHeader.substring(7);
    const payload = verifyToken(token);
    
    if (payload) {
      (req as any).user = payload;
    }
  }
  
  next();
}