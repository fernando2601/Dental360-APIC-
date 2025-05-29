export interface User {
  id: number;
  username: string;
  fullName: string;
  email: string;
  role: string;
  phone: string | null;
  lastLogin: Date | null;
  createdAt: Date | null;
}

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  limit: number;
}

export interface FilterParams {
  search?: string;
  category?: string;
  status?: string;
  startDate?: string;
  endDate?: string;
  page?: number;
  limit?: number;
}