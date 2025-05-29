export interface User {
  id: number;
  username: string;
  email: string;
  fullName: string;
  role: string;
  isActive: boolean;
  lastLogin?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface LoginRequest {
  username: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  token: string;
  user: User;
  expiresAt: string;
  refreshToken: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  fullName: string;
  password: string;
  confirmPassword: string;
  role?: string;
}

export interface DashboardMetrics {
  totalPatients: number;
  todayAppointments: number;
  pendingAppointments: number;
  todayRevenue: number;
  monthlyRevenue: number;
  lowStockAlerts: number;
  expirationAlerts: number;
  newPatientsThisMonth: number;
  appointmentCompletionRate: number;
  recentActivities: RecentActivity[];
  todayAppointmentsList: UpcomingAppointment[];
  quickStats: QuickStat[];
}

export interface RecentActivity {
  id: number;
  type: string;
  description: string;
  icon: string;
  color: string;
  timestamp: string;
  formattedTime: string;
  link?: string;
}

export interface UpcomingAppointment {
  id: number;
  patientName: string;
  serviceName: string;
  staffName: string;
  startTime: string;
  formattedTime: string;
  status: string;
  statusColor: string;
  room: string;
  duration: string;
}

export interface QuickStat {
  label: string;
  value: string;
  change: string;
  changeType: string;
  icon: string;
  color: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  email: string;
  newPassword: string;
  confirmPassword: string;
}

export interface UserProfile {
  id: number;
  username: string;
  email: string;
  fullName: string;
  role: string;
  phone?: string;
  avatar?: string;
  lastLogin?: string;
  createdAt: string;
  preferences: UserPreferences;
  statistics: UserStatistics;
}

export interface UserPreferences {
  theme: string;
  language: string;
  emailNotifications: boolean;
  smsNotifications: boolean;
  timeZone: string;
  dateFormat: string;
  timeFormat: string;
}

export interface UserStatistics {
  totalLogins: number;
  appointmentsCreated: number;
  patientsManaged: number;
  lastActivity?: string;
  daysActive: number;
}

export interface SystemInfo {
  version: string;
  environment: string;
  lastUpdate: string;
  totalUsers: number;
  activeSessions: number;
  databaseStatus: string;
  alerts: SystemAlert[];
}

export interface SystemAlert {
  type: string;
  message: string;
  severity: string;
  createdAt: string;
}