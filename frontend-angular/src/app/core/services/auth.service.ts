import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, throwError } from 'rxjs';
import { 
  User, 
  LoginRequest, 
  LoginResponse, 
  RegisterRequest, 
  DashboardMetrics, 
  UserProfile,
  ChangePasswordRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  SystemInfo
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = '/api/auth';
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private tokenSubject = new BehaviorSubject<string | null>(null);

  public currentUser$ = this.currentUserSubject.asObservable();
  public token$ = this.tokenSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadStoredAuth();
  }

  // Authentication Methods
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.API_URL}/login`, credentials)
      .pipe(
        tap(response => this.setAuthData(response)),
        catchError(this.handleError)
      );
  }

  register(userData: RegisterRequest): Observable<User> {
    return this.http.post<User>(`${this.API_URL}/register`, userData)
      .pipe(
        catchError(this.handleError)
      );
  }

  logout(): Observable<any> {
    return this.http.post(`${this.API_URL}/logout`, {})
      .pipe(
        tap(() => this.clearAuthData()),
        catchError(() => {
          this.clearAuthData();
          return throwError(() => new Error('Logout failed'));
        })
      );
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = localStorage.getItem('refresh_token');
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<LoginResponse>(`${this.API_URL}/refresh-token`, refreshToken)
      .pipe(
        tap(response => this.setAuthData(response)),
        catchError(this.handleError)
      );
  }

  // User Profile Methods
  getCurrentUser(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.API_URL}/me`)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateProfile(profile: UserProfile): Observable<any> {
    return this.http.put(`${this.API_URL}/profile`, profile)
      .pipe(
        catchError(this.handleError)
      );
  }

  changePassword(passwordData: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/change-password`, passwordData)
      .pipe(
        catchError(this.handleError)
      );
  }

  // Password Reset Methods
  forgotPassword(emailData: ForgotPasswordRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/forgot-password`, emailData)
      .pipe(
        catchError(this.handleError)
      );
  }

  resetPassword(resetData: ResetPasswordRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/reset-password`, resetData)
      .pipe(
        catchError(this.handleError)
      );
  }

  validateResetToken(token: string, email: string): Observable<{isValid: boolean}> {
    return this.http.get<{isValid: boolean}>(`${this.API_URL}/validate-reset-token`, {
      params: { token, email }
    }).pipe(
      catchError(this.handleError)
    );
  }

  // Dashboard Methods
  getDashboardMetrics(): Observable<DashboardMetrics> {
    return this.http.get<DashboardMetrics>(`${this.API_URL}/dashboard`)
      .pipe(
        catchError(this.handleError)
      );
  }

  getRecentActivities(limit: number = 10): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/recent-activities`, {
      params: { limit: limit.toString() }
    }).pipe(
      catchError(this.handleError)
    );
  }

  getSystemInfo(): Observable<SystemInfo> {
    return this.http.get<SystemInfo>(`${this.API_URL}/system-info`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // User Management (Admin)
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.API_URL}/users`)
      .pipe(
        catchError(this.handleError)
      );
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.API_URL}/users/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateUser(id: number, userData: User): Observable<any> {
    return this.http.put(`${this.API_URL}/users/${id}`, userData)
      .pipe(
        catchError(this.handleError)
      );
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.API_URL}/users/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // Validation Methods
  checkUsernameAvailability(username: string): Observable<{isAvailable: boolean}> {
    return this.http.get<{isAvailable: boolean}>(`${this.API_URL}/check-username`, {
      params: { username }
    }).pipe(
      catchError(this.handleError)
    );
  }

  checkEmailAvailability(email: string): Observable<{isAvailable: boolean}> {
    return this.http.get<{isAvailable: boolean}>(`${this.API_URL}/check-email`, {
      params: { email }
    }).pipe(
      catchError(this.handleError)
    );
  }

  // Session Management
  revokeAllSessions(): Observable<any> {
    return this.http.post(`${this.API_URL}/revoke-all-sessions`, {})
      .pipe(
        catchError(this.handleError)
      );
  }

  getUserStatistics(): Observable<any> {
    return this.http.get(`${this.API_URL}/statistics`)
      .pipe(
        catchError(this.handleError)
      );
  }

  getActivityLog(startDate?: string, endDate?: string): Observable<any[]> {
    let params: any = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;

    return this.http.get<any[]>(`${this.API_URL}/activity-log`, { params })
      .pipe(
        catchError(this.handleError)
      );
  }

  // Utility Methods
  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(token);
  }

  getToken(): string | null {
    return this.tokenSubject.value || localStorage.getItem('access_token');
  }

  getCurrentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUserValue();
    return user?.role === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUserValue();
    return user ? roles.includes(user.role) : false;
  }

  isAdmin(): boolean {
    return this.hasRole('admin');
  }

  isManager(): boolean {
    return this.hasAnyRole(['admin', 'manager']);
  }

  // Private Methods
  private setAuthData(response: LoginResponse): void {
    localStorage.setItem('access_token', response.token);
    localStorage.setItem('refresh_token', response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response.user));
    
    this.tokenSubject.next(response.token);
    this.currentUserSubject.next(response.user);
  }

  private clearAuthData(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
    
    this.tokenSubject.next(null);
    this.currentUserSubject.next(null);
  }

  private loadStoredAuth(): void {
    const token = localStorage.getItem('access_token');
    const userStr = localStorage.getItem('user');
    
    if (token && userStr && !this.isTokenExpired(token)) {
      try {
        const user = JSON.parse(userStr);
        this.tokenSubject.next(token);
        this.currentUserSubject.next(user);
      } catch (error) {
        this.clearAuthData();
      }
    } else {
      this.clearAuthData();
    }
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      return payload.exp < currentTime;
    } catch (error) {
      return true;
    }
  }

  private handleError(error: any) {
    let errorMessage = 'Ocorreu um erro inesperado';
    
    if (error.error?.message) {
      errorMessage = error.error.message;
    } else if (error.message) {
      errorMessage = error.message;
    } else if (typeof error.error === 'string') {
      errorMessage = error.error;
    }

    return throwError(() => new Error(errorMessage));
  }
}