import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { AppointmentService } from '../../services/appointment.service';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatInputModule,
    MatPaginatorModule,
    MatIconModule,
    MatChipsModule
  ],
  template: `
    <div class="appointments-container p-6">
      <h1 class="text-2xl font-bold mb-6 text-purple-700">Relatório de Agendamentos</h1>
      
      <!-- Filtros -->
      <div class="filters-section bg-white p-6 rounded-lg shadow-sm mb-6 border border-purple-100">
        <h2 class="text-lg font-semibold mb-4 text-gray-800">Filtros</h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
          <!-- Filtro de Data -->
          <mat-form-field appearance="outline">
            <mat-label>Data Início</mat-label>
            <input matInput [matDatepicker]="startPicker" [(ngModel)]="filters.startDate">
            <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
            <mat-datepicker #startPicker></mat-datepicker>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Data Fim</mat-label>
            <input matInput [matDatepicker]="endPicker" [(ngModel)]="filters.endDate">
            <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
            <mat-datepicker #endPicker></mat-datepicker>
          </mat-form-field>

          <!-- Filtro de Status -->
          <mat-form-field appearance="outline">
            <mat-label>Status</mat-label>
            <mat-select [(ngModel)]="filters.status" multiple>
              <mat-option value="scheduled">Agendado</mat-option>
              <mat-option value="confirmed">Confirmado</mat-option>
              <mat-option value="completed">Concluído</mat-option>
              <mat-option value="cancelled">Cancelado</mat-option>
              <mat-option value="no_show">Não compareceu</mat-option>
            </mat-select>
          </mat-form-field>

          <!-- Filtro de Profissional -->
          <mat-form-field appearance="outline">
            <mat-label>Profissional</mat-label>
            <mat-select [(ngModel)]="filters.professionalId">
              <mat-option value="">Todos</mat-option>
              <mat-option *ngFor="let prof of professionals" [value]="prof.id">
                {{prof.name}}
              </mat-option>
            </mat-select>
          </mat-form-field>

          <!-- Filtro de Convênio -->
          <mat-form-field appearance="outline">
            <mat-label>Convênio</mat-label>
            <mat-select [(ngModel)]="filters.convenio">
              <mat-option value="">Todos</mat-option>
              <mat-option value="Porto Seguro">Porto Seguro</mat-option>
              <mat-option value="SulAmérica">SulAmérica</mat-option>
              <mat-option value="Bradesco Saúde">Bradesco Saúde</mat-option>
              <mat-option value="Unimed">Unimed</mat-option>
              <mat-option value="Particular">Particular</mat-option>
            </mat-select>
          </mat-form-field>
        </div>

        <!-- Botões de Ação -->
        <div class="flex flex-wrap gap-2">
          <button mat-raised-button color="primary" (click)="applyFilters()" 
                  class="bg-purple-600 hover:bg-purple-700">
            <mat-icon>search</mat-icon>
            Filtrar
          </button>
          <button mat-stroked-button (click)="clearFilters()" 
                  class="border-purple-600 text-purple-600 hover:bg-purple-50">
            <mat-icon>clear</mat-icon>
            Limpar
          </button>
          <button mat-raised-button color="accent" (click)="downloadReport('pdf')"
                  class="bg-red-600 hover:bg-red-700">
            <mat-icon>picture_as_pdf</mat-icon>
            PDF
          </button>
          <button mat-raised-button color="accent" (click)="downloadReport('excel')"
                  class="bg-green-600 hover:bg-green-700">
            <mat-icon>table_view</mat-icon>
            Excel
          </button>
          <button mat-raised-button color="accent" (click)="downloadReport('csv')"
                  class="bg-blue-600 hover:bg-blue-700">
            <mat-icon>description</mat-icon>
            CSV
          </button>
        </div>
      </div>

      <!-- Resumo -->
      <div class="summary-section bg-gradient-to-r from-purple-50 to-blue-50 p-6 rounded-lg shadow-sm mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div class="text-center">
            <div class="text-2xl font-bold text-purple-600">{{summary.totalAppointments}}</div>
            <div class="text-sm text-gray-600">Total de Agendamentos</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-green-600">{{summary.completedAppointments}}</div>
            <div class="text-sm text-gray-600">Concluídos</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-red-600">{{summary.cancelledAppointments}}</div>
            <div class="text-sm text-gray-600">Cancelados</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-blue-600">{{summary.totalRevenue | currency:'BRL':'symbol':'1.2-2':'pt-BR'}}</div>
            <div class="text-sm text-gray-600">Receita Total</div>
          </div>
        </div>
      </div>

      <!-- Tabela de Resultados -->
      <div class="results-section bg-white rounded-lg shadow-sm border border-gray-200">
        <div class="p-6 border-b border-gray-200">
          <h2 class="text-lg font-semibold text-gray-800">
            Resultados ({{appointments.length}} agendamentos)
          </h2>
        </div>

        <div class="overflow-x-auto">
          <table mat-table [dataSource]="appointments" class="w-full">
            <!-- Coluna Procedimento -->
            <ng-container matColumnDef="procedimento">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Procedimento</th>
              <td mat-cell *matCellDef="let appointment" class="text-sm">{{appointment.procedimento}}</td>
            </ng-container>

            <!-- Coluna Paciente -->
            <ng-container matColumnDef="paciente">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Paciente</th>
              <td mat-cell *matCellDef="let appointment">
                <div class="flex flex-col">
                  <span class="font-medium text-sm">{{appointment.paciente.nome}}</span>
                  <span class="text-xs text-gray-500">{{appointment.paciente.cpf}}</span>
                  <span class="text-xs text-gray-500">{{appointment.paciente.telefone}}</span>
                </div>
              </td>
            </ng-container>

            <!-- Coluna Profissional -->
            <ng-container matColumnDef="profissional">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Profissional</th>
              <td mat-cell *matCellDef="let appointment">
                <div class="flex flex-col">
                  <span class="font-medium text-sm">{{appointment.profissional.nome}}</span>
                  <span class="text-xs text-gray-500">{{appointment.profissional.especialidade}}</span>
                </div>
              </td>
            </ng-container>

            <!-- Coluna Duração -->
            <ng-container matColumnDef="duracao">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Duração</th>
              <td mat-cell *matCellDef="let appointment" class="text-sm">{{appointment.duracao}} min</td>
            </ng-container>

            <!-- Coluna Data -->
            <ng-container matColumnDef="data">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Data</th>
              <td mat-cell *matCellDef="let appointment" class="text-sm">{{appointment.dataFormatada}}</td>
            </ng-container>

            <!-- Coluna Status -->
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Status</th>
              <td mat-cell *matCellDef="let appointment">
                <mat-chip [class]="getStatusClass(appointment.status)" class="text-xs">
                  {{appointment.statusLabel}}
                </mat-chip>
              </td>
            </ng-container>

            <!-- Coluna Convênio -->
            <ng-container matColumnDef="convenio">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Convênio</th>
              <td mat-cell *matCellDef="let appointment" class="text-sm">{{appointment.convenio}}</td>
            </ng-container>

            <!-- Coluna Sala -->
            <ng-container matColumnDef="sala">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Sala</th>
              <td mat-cell *matCellDef="let appointment" class="text-sm">{{appointment.sala}}</td>
            </ng-container>

            <!-- Coluna Comanda -->
            <ng-container matColumnDef="comanda">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Comanda</th>
              <td mat-cell *matCellDef="let appointment" class="text-sm font-mono">{{appointment.comanda}}</td>
            </ng-container>

            <!-- Coluna Valor -->
            <ng-container matColumnDef="valor">
              <th mat-header-cell *matHeaderCellDef class="font-semibold bg-gray-50">Valor</th>
              <td mat-cell *matCellDef="let appointment" class="font-medium text-sm text-green-600">
                {{appointment.valorFormatado}}
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;" 
                class="hover:bg-gray-50 transition-colors"></tr>
          </table>
        </div>

        <!-- Paginação -->
        <mat-paginator 
          [length]="totalItems"
          [pageSize]="pageSize"
          [pageSizeOptions]="[10, 25, 50, 100]"
          (page)="onPageChange($event)"
          showFirstLastButtons
          class="border-t border-gray-200">
        </mat-paginator>
      </div>
    </div>
  `,
  styles: [`
    .appointments-container {
      max-width: 1400px;
      margin: 0 auto;
      background-color: #f8fafc;
      min-height: 100vh;
    }
    
    .status-scheduled { 
      background-color: #e3f2fd; 
      color: #1976d2; 
      border: 1px solid #1976d2; 
    }
    .status-confirmed { 
      background-color: #f3e5f5; 
      color: #7b1fa2; 
      border: 1px solid #7b1fa2; 
    }
    .status-completed { 
      background-color: #e8f5e8; 
      color: #388e3c; 
      border: 1px solid #388e3c; 
    }
    .status-cancelled { 
      background-color: #ffebee; 
      color: #d32f2f; 
      border: 1px solid #d32f2f; 
    }
    .status-no_show { 
      background-color: #fff3e0; 
      color: #f57c00; 
      border: 1px solid #f57c00; 
    }

    ::ng-deep .mat-mdc-form-field {
      width: 100%;
    }

    ::ng-deep .mat-mdc-table {
      background: transparent;
    }

    ::ng-deep .mat-mdc-header-cell {
      background-color: #f9fafb !important;
      color: #374151 !important;
      font-weight: 600 !important;
    }
  `]
})
export class AppointmentsComponent implements OnInit {
  appointments: any[] = [];
  professionals: any[] = [];
  totalItems = 0;
  pageSize = 25;
  currentPage = 1;

  summary = {
    totalAppointments: 0,
    totalRevenue: 0,
    completedAppointments: 0,
    cancelledAppointments: 0
  };

  displayedColumns = [
    'procedimento', 'paciente', 'profissional', 'duracao',
    'data', 'status', 'convenio', 'sala', 'comanda', 'valor'
  ];

  filters = {
    startDate: null,
    endDate: null,
    status: [],
    professionalId: null,
    convenio: null
  };

  constructor(private appointmentService: AppointmentService) {}

  ngOnInit() {
    this.loadAppointments();
    this.loadProfessionals();
  }

  loadAppointments() {
    this.appointmentService.getAppointmentReports(
      this.filters.startDate,
      this.filters.endDate,
      this.filters.status,
      this.filters.professionalId,
      null,
      this.filters.convenio,
      null,
      this.currentPage,
      this.pageSize
    ).subscribe(data => {
      this.appointments = data.appointments;
      this.totalItems = data.pagination.totalItems;
      this.summary = data.summary;
    });
  }

  loadProfessionals() {
    this.professionals = [
      { id: 1, name: 'Dr. João Silva' },
      { id: 2, name: 'Dra. Maria Santos' },
      { id: 3, name: 'Dr. Pedro Costa' }
    ];
  }

  applyFilters() {
    this.currentPage = 1;
    this.loadAppointments();
  }

  clearFilters() {
    this.filters = {
      startDate: null,
      endDate: null,
      status: [],
      professionalId: null,
      convenio: null
    };
    this.applyFilters();
  }

  onPageChange(event: any) {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadAppointments();
  }

  getStatusClass(status: string): string {
    return `status-${status}`;
  }

  downloadReport(format: string) {
    this.appointmentService.downloadReport(format, this.filters).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `relatorio-agendamentos.${format}`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }
}