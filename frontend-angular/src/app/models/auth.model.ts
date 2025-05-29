export interface User {
  id: number;
  username: string;
  fullName: string;
  email: string;
  role: 'admin' | 'staff' | 'user';
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  user: User;
  token: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  fullName: string;
  email: string;
  role?: 'admin' | 'staff' | 'user';
}