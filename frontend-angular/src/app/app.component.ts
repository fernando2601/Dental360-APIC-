import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subject, takeUntil, filter } from 'rxjs';
import { AuthService } from './core/services/auth.service';
import { User } from './core/models/auth.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  title = 'Clínica - Sistema de Gestão';
  currentUser: User | null = null;
  isAuthenticated = false;
  showSidebar = false;
  currentRoute = '';

  navigationItems = [
    { path: '/dashboard', label: 'Dashboard', icon: 'home', roles: ['admin', 'manager', 'user'] },
    { path: '/agenda', label: 'Agenda', icon: 'calendar', roles: ['admin', 'manager', 'user'] },
    { path: '/patients', label: 'Pacientes', icon: 'users', roles: ['admin', 'manager', 'user'] },
    { path: '/inventory', label: 'Estoque', icon: 'package', roles: ['admin', 'manager', 'user'] },
    { path: '/finance', label: 'Financeiro', icon: 'dollar-sign', roles: ['admin', 'manager'] }
  ];

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeAuth();
    this.trackRouteChanges();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeAuth(): void {
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.currentUser = user;
        this.isAuthenticated = !!user;
        this.showSidebar = this.isAuthenticated && !this.isAuthRoute();
      });
  }

  private trackRouteChanges(): void {
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe((event: NavigationEnd) => {
        this.currentRoute = event.url;
        this.showSidebar = this.isAuthenticated && !this.isAuthRoute();
      });
  }

  private isAuthRoute(): boolean {
    return this.currentRoute.includes('/auth/');
  }

  hasPermission(roles: string[]): boolean {
    if (!this.currentUser) return false;
    return roles.includes(this.currentUser.role);
  }

  isActiveRoute(path: string): boolean {
    return this.currentRoute.startsWith(path);
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: () => {
        // Mesmo com erro, limpar dados locais e redirecionar
        this.router.navigate(['/auth/login']);
      }
    });
  }

  toggleSidebar(): void {
    this.showSidebar = !this.showSidebar;
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  getUserDisplayName(): string {
    return this.currentUser?.fullName || this.currentUser?.username || 'Usuário';
  }

  getUserRole(): string {
    const roleLabels: { [key: string]: string } = {
      'admin': 'Administrador',
      'manager': 'Gerente',
      'user': 'Usuário'
    };
    return roleLabels[this.currentUser?.role || 'user'] || 'Usuário';
  }
}