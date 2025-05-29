import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface StaffMember {
  id?: number;
  fullName: string;
  email: string;
  phone: string;
  position: string;
  specialization: string;
  department: string;
  salary: number;
  hireDate: string;
  isActive: boolean;
  bio: string;
  profileImageUrl: string;
  yearsOfExperience: number;
  license: string;
  managerId?: number;
  managerName?: string;
  certifications: string[];
  skills: string[];
  formattedSalary?: string;
  status?: string;
}

interface StaffStats {
  totalStaff: number;
  activeStaff: number;
  inactiveStaff: number;
  averageSalary: number;
  averageExperience: number;
  departmentBreakdown: DepartmentStats[];
  positionBreakdown: PositionStats[];
}

interface DepartmentStats {
  department: string;
  count: number;
  averageSalary: number;
}

interface PositionStats {
  position: string;
  count: number;
  averageSalary: number;
}

@Component({
  selector: 'app-staff',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <!-- Header com estilo DentalSpa -->
      <div class="bg-gradient-to-r from-purple-600 to-purple-700 text-white p-6 rounded-lg mb-6">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-bold">Diretório de Equipe</h1>
            <p class="text-purple-100">Gerencie a equipe e os profissionais da sua clínica.</p>
          </div>
          <div class="flex space-x-3">
            <button
              (click)="showNewStaffForm = !showNewStaffForm"
              class="bg-white text-purple-600 hover:bg-purple-50 px-4 py-2 rounded-md font-medium transition-colors"
            >
              + Adicionar Membro da Equipe
            </button>
            <button
              class="bg-purple-500 hover:bg-purple-400 text-white px-4 py-2 rounded-md font-medium transition-colors"
            >
              📋 Exportar
            </button>
          </div>
        </div>
      </div>

      <!-- Barra de Busca -->
      <div class="bg-white shadow rounded-lg p-4 mb-6">
        <div class="flex items-center space-x-4">
          <div class="flex-1 relative">
            <input
              type="text"
              [(ngModel)]="searchTerm"
              (input)="onSearch()"
              placeholder="Buscar no diretório de equipe..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
            />
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <span class="text-gray-400">🔍</span>
            </div>
          </div>
          <div class="flex space-x-2">
            <button
              (click)="viewMode = 'cards'"
              [class]="viewMode === 'cards' ? 'bg-purple-600 text-white' : 'bg-gray-200 text-gray-700'"
              class="px-3 py-2 rounded-md text-sm font-medium transition-colors"
            >
              Visualização em Cards
            </button>
            <button
              (click)="viewMode = 'list'"
              [class]="viewMode === 'list' ? 'bg-purple-600 text-white' : 'bg-gray-200 text-gray-700'"
              class="px-3 py-2 rounded-md text-sm font-medium transition-colors"
            >
              Visualização em Lista
            </button>
          </div>
        </div>
      </div>

      <!-- Formulário Novo Funcionário -->
      <div *ngIf="showNewStaffForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Cadastro de Funcionário</h3>
        <form [formGroup]="staffForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Nome Completo</label>
              <input
                type="text"
                formControlName="fullName"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Nome completo do funcionário"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Email</label>
              <input
                type="email"
                formControlName="email"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="email@dentalspa.com"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Telefone</label>
              <input
                type="tel"
                formControlName="phone"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="(11) 99999-9999"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Cargo</label>
              <select
                formControlName="position"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="">Selecione um cargo</option>
                <option value="Dentista">Dentista</option>
                <option value="Ortodontista">Ortodontista</option>
                <option value="Endodontista">Endodontista</option>
                <option value="Auxiliar de Enfermagem">Auxiliar de Enfermagem</option>
                <option value="Recepcionista">Recepcionista</option>
                <option value="Gerente">Gerente</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Especialização</label>
              <input
                type="text"
                formControlName="specialization"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ex: Odontologia Geral"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Departamento</label>
              <select
                formControlName="department"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="">Selecione um departamento</option>
                <option value="Odontologia">Odontologia</option>
                <option value="Apoio Clínico">Apoio Clínico</option>
                <option value="Administrativo">Administrativo</option>
                <option value="Gerência">Gerência</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Salário (R$)</label>
              <input
                type="number"
                step="0.01"
                formControlName="salary"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="0.00"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Data de Contratação</label>
              <input
                type="date"
                formControlName="hireDate"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Anos de Experiência</label>
              <input
                type="number"
                formControlName="yearsOfExperience"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="0"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Licença/Registro</label>
              <input
                type="text"
                formControlName="license"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ex: CRO-SP 12345"
              />
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700">Bio</label>
              <textarea
                formControlName="bio"
                rows="3"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Biografia e experiências do funcionário"
              ></textarea>
            </div>
            <div>
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isActive"
                  class="rounded border-gray-300 text-purple-600 shadow-sm focus:border-purple-300 focus:ring focus:ring-purple-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Funcionário ativo</span>
              </label>
            </div>
          </div>
          <div class="mt-6 flex justify-end space-x-3">
            <button
              type="button"
              (click)="cancelForm()"
              class="bg-gray-300 hover:bg-gray-400 text-gray-800 px-4 py-2 rounded-md text-sm font-medium"
            >
              Cancelar
            </button>
            <button
              type="submit"
              [disabled]="!staffForm.valid || isLoading"
              class="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : (editingStaff ? 'Atualizar' : 'Salvar') }}
            </button>
          </div>
        </form>
      </div>

      <!-- Conteúdo Principal -->
      <div [ngSwitch]="viewMode">
        
        <!-- Visualização em Cards -->
        <div *ngSwitchCase="'cards'">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <div *ngFor="let staff of filteredStaff" class="bg-white shadow rounded-lg overflow-hidden hover:shadow-lg transition-shadow">
              <!-- Avatar e Status -->
              <div class="p-4 text-center border-b border-gray-200">
                <div class="w-16 h-16 bg-purple-600 rounded-full mx-auto mb-3 flex items-center justify-center text-white text-lg font-semibold">
                  {{ getInitials(staff.fullName) }}
                </div>
                <span [class]="staff.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'" 
                      class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                  {{ staff.isActive ? 'Disponível' : 'Inativo' }}
                </span>
              </div>

              <!-- Informações -->
              <div class="p-4">
                <h3 class="text-lg font-semibold text-gray-900 mb-1">{{ staff.fullName }}</h3>
                <p class="text-sm text-gray-600 mb-2">{{ staff.specialization }}</p>
                <p class="text-xs text-gray-500 mb-3">{{ staff.department }}</p>
                
                <div class="space-y-1 text-xs text-gray-600 mb-4">
                  <p>📧 {{ staff.email }}</p>
                  <p>📞 {{ staff.phone }}</p>
                  <p *ngIf="staff.license">🏥 {{ staff.license }}</p>
                </div>

                <!-- Bio -->
                <p class="text-xs text-gray-600 mb-4 line-clamp-2">{{ staff.bio }}</p>

                <!-- Ações -->
                <div class="flex space-x-2">
                  <button
                    (click)="viewDetails(staff)"
                    class="flex-1 bg-gray-50 text-gray-600 hover:bg-gray-100 px-3 py-2 rounded text-xs font-medium"
                  >
                    👁️ Ver Detalhes
                  </button>
                  <button
                    (click)="editStaff(staff)"
                    class="flex-1 bg-blue-50 text-blue-600 hover:bg-blue-100 px-3 py-2 rounded text-xs font-medium"
                  >
                    ✏️ Edit
                  </button>
                </div>
              </div>
            </div>

            <!-- Card vazio quando não há funcionários -->
            <div *ngIf="filteredStaff.length === 0" class="col-span-full text-center py-12">
              <div class="text-gray-400 text-lg mb-2">👥</div>
              <p class="text-gray-500">Nenhum funcionário encontrado</p>
            </div>
          </div>
        </div>

        <!-- Visualização em Lista -->
        <div *ngSwitchCase="'list'">
          <div class="bg-white shadow rounded-lg overflow-hidden">
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Funcionário
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Cargo
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Departamento
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Contato
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Status
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Ações
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-200">
                  <tr *ngFor="let staff of filteredStaff">
                    <td class="px-6 py-4 whitespace-nowrap">
                      <div class="flex items-center">
                        <div class="flex-shrink-0 h-10 w-10">
                          <div class="h-10 w-10 bg-purple-600 rounded-full flex items-center justify-center text-white text-sm font-semibold">
                            {{ getInitials(staff.fullName) }}
                          </div>
                        </div>
                        <div class="ml-4">
                          <div class="text-sm font-medium text-gray-900">{{ staff.fullName }}</div>
                          <div class="text-sm text-gray-500">{{ staff.specialization }}</div>
                        </div>
                      </div>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {{ staff.position }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {{ staff.department }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      <div>{{ staff.email }}</div>
                      <div>{{ staff.phone }}</div>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <span [class]="staff.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'" 
                            class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                        {{ staff.isActive ? 'Disponível' : 'Inativo' }}
                      </span>
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <button
                        (click)="viewDetails(staff)"
                        class="text-purple-600 hover:text-purple-900 mr-3"
                      >
                        Ver Detalhes
                      </button>
                      <button
                        (click)="editStaff(staff)"
                        class="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        Editar
                      </button>
                      <button
                        (click)="deleteStaff(staff.id!)"
                        class="text-red-600 hover:text-red-900"
                      >
                        Excluir
                      </button>
                    </td>
                  </tr>
                  <tr *ngIf="filteredStaff.length === 0">
                    <td colspan="6" class="px-6 py-4 text-center text-sm text-gray-500">
                      Nenhum funcionário encontrado
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

      </div>
    </div>
  `
})
export class StaffComponent implements OnInit {
  staff: StaffMember[] = [];
  filteredStaff: StaffMember[] = [];
  staffForm: FormGroup;
  showNewStaffForm = false;
  isLoading = false;
  viewMode: 'cards' | 'list' = 'cards';
  searchTerm = '';
  editingStaff: StaffMember | null = null;
  stats: StaffStats | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.staffForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.maxLength(200)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required]],
      position: ['', [Validators.required]],
      specialization: ['', [Validators.required]],
      department: ['', [Validators.required]],
      salary: [0, [Validators.required, Validators.min(0)]],
      hireDate: ['', [Validators.required]],
      yearsOfExperience: [0, [Validators.min(0), Validators.max(50)]],
      license: [''],
      bio: [''],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadStaff();
    this.loadStats();
  }

  private loadStaff(): void {
    // Conectar com sistema atual enquanto migra para Angular/.NET
    this.http.get<any[]>('http://localhost:5000/api/staff').subscribe({
      next: (data) => {
        this.staff = data.map(member => ({
          id: member.id,
          fullName: member.fullName,
          email: member.email,
          phone: member.phone,
          position: member.position,
          specialization: member.specialization,
          department: member.department || 'Odontologia',
          salary: member.salary || 0,
          hireDate: member.hireDate || new Date().toISOString().split('T')[0],
          isActive: member.isActive !== false,
          bio: member.bio,
          profileImageUrl: member.profileImageUrl || '',
          yearsOfExperience: member.yearsOfExperience || 0,
          license: member.license || '',
          certifications: member.certifications || [],
          skills: member.skills || []
        }));
        this.applyFilter();
      },
      error: (error) => {
        console.error('Erro ao carregar funcionários:', error);
        this.staff = [];
        this.applyFilter();
      }
    });
  }

  private loadStats(): void {
    this.http.get<StaffStats>('https://localhost:5001/api/staff/stats').subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  onSearch(): void {
    this.applyFilter();
  }

  private applyFilter(): void {
    if (!this.searchTerm.trim()) {
      this.filteredStaff = [...this.staff];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredStaff = this.staff.filter(member =>
        member.fullName.toLowerCase().includes(term) ||
        member.email.toLowerCase().includes(term) ||
        member.position.toLowerCase().includes(term) ||
        member.specialization.toLowerCase().includes(term) ||
        member.department.toLowerCase().includes(term)
      );
    }
  }

  onSubmit(): void {
    if (this.staffForm.valid) {
      this.isLoading = true;
      const staffData = this.staffForm.value;

      const request = this.editingStaff
        ? this.http.put<any>(`http://localhost:5000/api/staff/${this.editingStaff.id}`, staffData)
        : this.http.post<any>('http://localhost:5000/api/staff', staffData);

      request.subscribe({
        next: (response) => {
          const mappedStaff = {
            id: response.id,
            fullName: response.fullName,
            email: response.email,
            phone: response.phone,
            position: response.position,
            specialization: response.specialization,
            department: response.department,
            salary: response.salary,
            hireDate: response.hireDate,
            isActive: response.isActive,
            bio: response.bio,
            profileImageUrl: response.profileImageUrl || '',
            yearsOfExperience: response.yearsOfExperience || 0,
            license: response.license || '',
            certifications: response.certifications || [],
            skills: response.skills || []
          };

          if (this.editingStaff) {
            const index = this.staff.findIndex(s => s.id === this.editingStaff!.id);
            if (index !== -1) {
              this.staff[index] = mappedStaff;
            }
          } else {
            this.staff.push(mappedStaff);
          }
          this.applyFilter();
          this.cancelForm();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao salvar funcionário:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editStaff(staff: StaffMember): void {
    this.editingStaff = staff;
    this.staffForm.patchValue(staff);
    this.showNewStaffForm = true;
  }

  deleteStaff(id: number): void {
    if (confirm('Tem certeza que deseja excluir este funcionário?')) {
      this.http.delete(`http://localhost:5000/api/staff/${id}`).subscribe({
        next: () => {
          this.staff = this.staff.filter(s => s.id !== id);
          this.applyFilter();
        },
        error: (error) => {
          console.error('Erro ao excluir funcionário:', error);
        }
      });
    }
  }

  viewDetails(staff: StaffMember): void {
    // Implementar modal de detalhes ou navegação para página de detalhes
    console.log('Ver detalhes de:', staff);
  }

  cancelForm(): void {
    this.showNewStaffForm = false;
    this.editingStaff = null;
    this.staffForm.reset({
      fullName: '',
      email: '',
      phone: '',
      position: '',
      specialization: '',
      department: '',
      salary: 0,
      hireDate: '',
      yearsOfExperience: 0,
      license: '',
      bio: '',
      isActive: true
    });
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map(n => n.charAt(0))
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }
}