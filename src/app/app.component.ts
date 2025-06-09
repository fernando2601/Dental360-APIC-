import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  template: `
    <div class="app-container">
      <!-- Sidebar -->
      <nav class="sidebar" *ngIf="isAuthenticated">
        <div class="sidebar-header">
          <div class="logo">
            <span class="logo-icon">DS</span>
            <span class="logo-text">DentalSpa</span>
          </div>
        </div>
        <ul class="nav-menu">
          <li><a routerLink="/dashboard" routerLinkActive="active"><i class="icon">📊</i> Dashboard</a></li>
          <li><a routerLink="/patients" routerLinkActive="active"><i class="icon">👥</i> Pacientes</a></li>
          <li><a routerLink="/appointments" routerLinkActive="active"><i class="icon">📅</i> Agendamentos</a></li>
          <li><a routerLink="/agenda" routerLinkActive="active"><i class="icon">🗓️</i> Agenda</a></li>
          <li><a routerLink="/services" routerLinkActive="active"><i class="icon">⚕️</i> Serviços</a></li>
          <li><a routerLink="/packages" routerLinkActive="active"><i class="icon">📦</i> Pacotes</a></li>
          <li><a routerLink="/staff" routerLinkActive="active"><i class="icon">👨‍⚕️</i> Equipe</a></li>
          <li><a routerLink="/financial" routerLinkActive="active"><i class="icon">💰</i> Financeiro</a></li>
          <li><a routerLink="/inventory" routerLinkActive="active"><i class="icon">📋</i> Estoque</a></li>
          <li><a routerLink="/before-after" routerLinkActive="active"><i class="icon">📸</i> Antes/Depois</a></li>
          <li><a routerLink="/learning" routerLinkActive="active"><i class="icon">📚</i> Aprendizado</a></li>
          <li><a routerLink="/analytics" routerLinkActive="active"><i class="icon">📈</i> Analytics</a></li>
          <li><a routerLink="/clinic-info" routerLinkActive="active"><i class="icon">🏥</i> Clínica</a></li>
          <li><a routerLink="/whatsapp" routerLinkActive="active"><i class="icon">💬</i> WhatsApp</a></li>
          <li><a (click)="logout()" class="logout-btn"><i class="icon">🚪</i> Sair</a></li>
        </ul>
      </nav>

      <!-- Main Content -->
      <main class="main-content" [class.full-width]="!isAuthenticated">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      display: flex;
      min-height: 100vh;
    }

    .sidebar {
      width: 250px;
      background: #1f2937;
      color: white;
      padding: 0;
      position: fixed;
      height: 100vh;
      overflow-y: auto;
    }

    .sidebar-header {
      padding: 1.5rem;
      border-bottom: 1px solid #374151;
    }

    .logo {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .logo-icon {
      width: 2rem;
      height: 2rem;
      background: #0ea5e9;
      border-radius: 0.5rem;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 600;
      font-size: 0.875rem;
    }

    .logo-text {
      font-size: 1.25rem;
      font-weight: 700;
    }

    .nav-menu {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .nav-menu li {
      margin: 0;
    }

    .nav-menu a {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.875rem 1.5rem;
      color: #d1d5db;
      text-decoration: none;
      transition: all 0.2s;
      cursor: pointer;
    }

    .nav-menu a:hover,
    .nav-menu a.active {
      background: #374151;
      color: white;
    }

    .nav-menu a.active {
      border-right: 3px solid #0ea5e9;
    }

    .icon {
      font-size: 1.125rem;
    }

    .logout-btn {
      border-top: 1px solid #374151;
      margin-top: 1rem;
    }

    .main-content {
      flex: 1;
      margin-left: 250px;
      padding: 2rem;
      background: #f9fafb;
      min-height: 100vh;
    }

    .main-content.full-width {
      margin-left: 0;
    }

    @media (max-width: 768px) {
      .sidebar {
        transform: translateX(-100%);
        transition: transform 0.3s;
      }
      
      .main-content {
        margin-left: 0;
      }
    }
  `]
})
export class AppComponent {
  title = 'DentalSpa';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  get isAuthenticated(): boolean {
    return true; // Temporariamente sempre autenticado para desenvolvimento
  }

  logout() {
    this.authService.logout();
  }
}