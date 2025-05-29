import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen bg-gray-100">
      <!-- Header -->
      <header class="bg-white shadow">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between h-16">
            <div class="flex items-center">
              <h1 class="text-xl font-semibold">Sistema de Clínica Odontológica</h1>
            </div>
            <div class="flex items-center space-x-4">
              <span class="text-sm text-gray-700">
                Olá, {{ currentUser?.fullName }}
              </span>
              <button
                (click)="logout()"
                class="bg-red-600 hover:bg-red-700 text-white px-3 py-2 rounded-md text-sm font-medium"
              >
                Sair
              </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Main Content -->
      <div class="flex">
        <!-- Sidebar -->
        <nav class="bg-gray-800 w-64 min-h-screen">
          <div class="px-4 py-6">
            <ul class="space-y-2">
              <li>
                <a
                  routerLink="/dashboard"
                  routerLinkActive="bg-gray-900"
                  class="text-gray-300 hover:bg-gray-700 hover:text-white group flex items-center px-2 py-2 text-sm font-medium rounded-md"
                >
                  Dashboard
                </a>
              </li>
              <li>
                <a
                  routerLink="/agenda"
                  routerLinkActive="bg-gray-900"
                  class="text-gray-300 hover:bg-gray-700 hover:text-white group flex items-center px-2 py-2 text-sm font-medium rounded-md"
                >
                  Agenda
                </a>
              </li>
              <li>
                <a
                  routerLink="/pacientes"
                  routerLinkActive="bg-gray-900"
                  class="text-gray-300 hover:bg-gray-700 hover:text-white group flex items-center px-2 py-2 text-sm font-medium rounded-md"
                >
                  Pacientes
                </a>
              </li>
              <li>
                <a
                  routerLink="/inventario"
                  routerLinkActive="bg-gray-900"
                  class="text-gray-300 hover:bg-gray-700 hover:text-white group flex items-center px-2 py-2 text-sm font-medium rounded-md"
                >
                  Inventário
                </a>
              </li>
              <li>
                <a
                  routerLink="/financeiro"
                  routerLinkActive="bg-gray-900"
                  class="text-gray-300 hover:bg-gray-700 hover:text-white group flex items-center px-2 py-2 text-sm font-medium rounded-md"
                >
                  Financeiro
                </a>
              </li>
            </ul>
          </div>
        </nav>

        <!-- Content Area -->
        <main class="flex-1 p-6">
          <div class="max-w-7xl mx-auto">
            <h2 class="text-2xl font-bold text-gray-900 mb-6">Dashboard</h2>
            
            <!-- Stats Cards -->
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
              <div class="bg-white overflow-hidden shadow rounded-lg">
                <div class="p-5">
                  <div class="flex items-center">
                    <div class="flex-shrink-0">
                      <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                        <span class="text-white text-sm">👥</span>
                      </div>
                    </div>
                    <div class="ml-5 w-0 flex-1">
                      <dl>
                        <dt class="text-sm font-medium text-gray-500 truncate">
                          Total de Pacientes
                        </dt>
                        <dd class="text-lg font-medium text-gray-900">
                          {{ metrics.totalPatients }}
                        </dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>

              <div class="bg-white overflow-hidden shadow rounded-lg">
                <div class="p-5">
                  <div class="flex items-center">
                    <div class="flex-shrink-0">
                      <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                        <span class="text-white text-sm">📅</span>
                      </div>
                    </div>
                    <div class="ml-5 w-0 flex-1">
                      <dl>
                        <dt class="text-sm font-medium text-gray-500 truncate">
                          Agendamentos Hoje
                        </dt>
                        <dd class="text-lg font-medium text-gray-900">
                          {{ metrics.appointmentsToday }}
                        </dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>

              <div class="bg-white overflow-hidden shadow rounded-lg">
                <div class="p-5">
                  <div class="flex items-center">
                    <div class="flex-shrink-0">
                      <div class="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                        <span class="text-white text-sm">💰</span>
                      </div>
                    </div>
                    <div class="ml-5 w-0 flex-1">
                      <dl>
                        <dt class="text-sm font-medium text-gray-500 truncate">
                          Receita Mensal
                        </dt>
                        <dd class="text-lg font-medium text-gray-900">
                          R$ {{ metrics.monthlyRevenue?.toLocaleString('pt-BR') }}
                        </dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>

              <div class="bg-white overflow-hidden shadow rounded-lg">
                <div class="p-5">
                  <div class="flex items-center">
                    <div class="flex-shrink-0">
                      <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                        <span class="text-white text-sm">📊</span>
                      </div>
                    </div>
                    <div class="ml-5 w-0 flex-1">
                      <dl>
                        <dt class="text-sm font-medium text-gray-500 truncate">
                          Tratamentos Ativos
                        </dt>
                        <dd class="text-lg font-medium text-gray-900">
                          {{ metrics.activeTreatments }}
                        </dd>
                      </dl>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Quick Actions -->
            <div class="bg-white shadow rounded-lg p-6">
              <h3 class="text-lg font-medium text-gray-900 mb-4">Ações Rápidas</h3>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <button
                  (click)="navigateTo('/agenda')"
                  class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
                >
                  Novo Agendamento
                </button>
                <button
                  (click)="navigateTo('/pacientes')"
                  class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md text-sm font-medium"
                >
                  Cadastrar Paciente
                </button>
                <button
                  (click)="navigateTo('/financeiro')"
                  class="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md text-sm font-medium"
                >
                  Registrar Pagamento
                </button>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  currentUser = this.authService.getCurrentUser();
  metrics = {
    totalPatients: 0,
    appointmentsToday: 0,
    monthlyRevenue: 0,
    activeTreatments: 0
  };

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadMetrics();
  }

  private loadMetrics(): void {
    // Conectará com o backend .NET para buscar métricas reais
    this.metrics = {
      totalPatients: 0,
      appointmentsToday: 0,
      monthlyRevenue: 0,
      activeTreatments: 0
    };
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}