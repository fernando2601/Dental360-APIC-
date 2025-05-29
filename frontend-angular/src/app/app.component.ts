import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <div class="app-container">
      <!-- Header/Navbar -->
      <mat-toolbar color="primary" class="app-header shadow-lg">
        <mat-toolbar-row class="px-6">
          <span class="text-xl font-bold text-white">Clínica Dental & Harmonização</span>
          <span class="spacer"></span>
          <button mat-icon-button class="text-white">
            <mat-icon>account_circle</mat-icon>
          </button>
        </mat-toolbar-row>
      </mat-toolbar>

      <!-- Sidebar + Content -->
      <mat-sidenav-container class="app-sidenav-container">
        <mat-sidenav #sidenav mode="side" opened class="app-sidenav">
          <mat-nav-list>
            <a mat-list-item routerLink="/dashboard" routerLinkActive="active-link">
              <mat-icon matListItemIcon>dashboard</mat-icon>
              <span matListItemTitle>Dashboard</span>
            </a>
            <a mat-list-item routerLink="/appointments" routerLinkActive="active-link">
              <mat-icon matListItemIcon>event</mat-icon>
              <span matListItemTitle>Agendamentos</span>
            </a>
            <a mat-list-item routerLink="/clients" routerLinkActive="active-link">
              <mat-icon matListItemIcon>people</mat-icon>
              <span matListItemTitle>Clientes</span>
            </a>
            <a mat-list-item routerLink="/services" routerLinkActive="active-link">
              <mat-icon matListItemIcon>medical_services</mat-icon>
              <span matListItemTitle>Serviços</span>
            </a>
            <a mat-list-item routerLink="/staff" routerLinkActive="active-link">
              <mat-icon matListItemIcon>person</mat-icon>
              <span matListItemTitle>Profissionais</span>
            </a>
            <a mat-list-item routerLink="/inventory" routerLinkActive="active-link">
              <mat-icon matListItemIcon>inventory</mat-icon>
              <span matListItemTitle>Estoque</span>
            </a>
            <a mat-list-item routerLink="/financial" routerLinkActive="active-link">
              <mat-icon matListItemIcon>attach_money</mat-icon>
              <span matListItemTitle>Financeiro</span>
            </a>
            <a mat-list-item routerLink="/reports" routerLinkActive="active-link">
              <mat-icon matListItemIcon>analytics</mat-icon>
              <span matListItemTitle>Relatórios</span>
            </a>
          </mat-nav-list>
        </mat-sidenav>

        <mat-sidenav-content class="app-content">
          <router-outlet></router-outlet>
        </mat-sidenav-content>
      </mat-sidenav-container>
    </div>
  `,
  styles: [`
    .app-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .app-header {
      background: linear-gradient(135deg, #7c3aed 0%, #a855f7 100%) !important;
      z-index: 1000;
    }

    .spacer {
      flex: 1 1 auto;
    }

    .app-sidenav-container {
      flex: 1;
    }

    .app-sidenav {
      width: 280px;
      background-color: #f8fafc;
      border-right: 1px solid #e2e8f0;
    }

    .app-content {
      background-color: #f1f5f9;
      min-height: 100%;
    }

    .active-link {
      background-color: #e5e7eb !important;
      color: #7c3aed !important;
    }

    .active-link mat-icon {
      color: #7c3aed !important;
    }

    ::ng-deep .mat-mdc-list-item {
      margin: 4px 8px;
      border-radius: 8px;
      transition: all 0.2s ease;
    }

    ::ng-deep .mat-mdc-list-item:hover {
      background-color: #e5e7eb;
    }

    ::ng-deep .mat-mdc-list-item-icon {
      color: #6b7280;
    }
  `]
})
export class AppComponent {
  title = 'clinic-frontend-angular';
}