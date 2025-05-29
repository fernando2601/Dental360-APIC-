import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface ClinicInfo {
  id: number;
  name: string;
  description: string;
  address: string;
  phone: string;
  email: string;
  website: string;
  logo: string;
  workingHours: {
    monday: string;
    tuesday: string;
    wednesday: string;
    thursday: string;
    friday: string;
    saturday: string;
    sunday: string;
  };
  specialties: string[];
  socialMedia: {
    instagram: string;
    facebook: string;
    whatsapp: string;
  };
}

@Component({
  selector: 'app-clinic-info',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-cyan-100 p-6">
      <div class="max-w-6xl mx-auto">
        <!-- Header -->
        <div class="mb-8">
          <h1 class="text-4xl font-bold text-gray-900 mb-2">Dados da Clínica</h1>
          <p class="text-gray-600">Configure as informações básicas da sua clínica</p>
        </div>

        <!-- Main Form -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          
          <!-- Form Section -->
          <div class="lg:col-span-2">
            <form [formGroup]="clinicForm" (ngSubmit)="onSubmit()" class="space-y-6">
              
              <!-- Basic Information Card -->
              <div class="bg-white rounded-xl shadow-lg p-6">
                <h2 class="text-xl font-semibold text-gray-900 mb-6">Informações Básicas</h2>
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Nome da Clínica</label>
                    <input
                      type="text"
                      formControlName="name"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="DentalSpa Clinic"
                    />
                  </div>
                  
                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Descrição</label>
                    <textarea
                      formControlName="description"
                      rows="3"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="Clínica especializada em odontologia estética e harmonização facial..."
                    ></textarea>
                  </div>

                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Endereço Completo</label>
                    <input
                      type="text"
                      formControlName="address"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="Rua das Flores, 123 - Centro - São Paulo/SP"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Telefone</label>
                    <input
                      type="tel"
                      formControlName="phone"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="(11) 99999-9999"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">E-mail</label>
                    <input
                      type="email"
                      formControlName="email"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="contato@dentalspa.com"
                    />
                  </div>

                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Website</label>
                    <input
                      type="url"
                      formControlName="website"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="https://www.dentalspa.com"
                    />
                  </div>
                </div>
              </div>

              <!-- Working Hours Card -->
              <div class="bg-white rounded-xl shadow-lg p-6">
                <h2 class="text-xl font-semibold text-gray-900 mb-6">Horário de Funcionamento</h2>
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Segunda-feira</label>
                    <input
                      type="text"
                      formControlName="mondayHours"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="08:00 - 18:00"
                    />
                  </div>
                  
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Terça-feira</label>
                    <input
                      type="text"
                      formControlName="tuesdayHours"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="08:00 - 18:00"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Quarta-feira</label>
                    <input
                      type="text"
                      formControlName="wednesdayHours"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="08:00 - 18:00"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Quinta-feira</label>
                    <input
                      type="text"
                      formControlName="thursdayHours"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="08:00 - 18:00"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Sexta-feira</label>
                    <input
                      type="text"
                      formControlName="fridayHours"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="08:00 - 18:00"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Sábado</label>
                    <input
                      type="text"
                      formControlName="saturdayHours"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="08:00 - 14:00"
                    />
                  </div>

                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">Domingo</label>
                    <input
                      type="text"
                      formControlName="sundayHours"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="Fechado"
                    />
                  </div>
                </div>
              </div>

              <!-- Social Media Card -->
              <div class="bg-white rounded-xl shadow-lg p-6">
                <h2 class="text-xl font-semibold text-gray-900 mb-6">Redes Sociais</h2>
                
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Instagram</label>
                    <input
                      type="text"
                      formControlName="instagram"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="@dentalspa"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">Facebook</label>
                    <input
                      type="text"
                      formControlName="facebook"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="DentalSpa Clinic"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">WhatsApp</label>
                    <input
                      type="tel"
                      formControlName="whatsapp"
                      class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="(11) 99999-9999"
                    />
                  </div>
                </div>
              </div>

              <!-- Submit Button -->
              <div class="flex justify-end space-x-4">
                <button
                  type="button"
                  class="px-6 py-3 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 font-medium transition-colors"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  [disabled]="!clinicForm.valid"
                  class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  Salvar Alterações
                </button>
              </div>
            </form>
          </div>

          <!-- Side Panel -->
          <div class="space-y-6">
            
            <!-- Logo Upload Card -->
            <div class="bg-white rounded-xl shadow-lg p-6">
              <h3 class="text-lg font-semibold text-gray-900 mb-4">Logo da Clínica</h3>
              
              <div class="text-center">
                <div class="w-32 h-32 mx-auto bg-gray-100 rounded-lg flex items-center justify-center mb-4">
                  <svg class="w-12 h-12 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z" clip-rule="evenodd"></path>
                  </svg>
                </div>
                
                <button class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                  Upload Logo
                </button>
                <p class="text-sm text-gray-500 mt-2">PNG, JPG até 2MB</p>
              </div>
            </div>

            <!-- Quick Stats Card -->
            <div class="bg-white rounded-xl shadow-lg p-6">
              <h3 class="text-lg font-semibold text-gray-900 mb-4">Estatísticas Rápidas</h3>
              
              <div class="space-y-4">
                <div class="flex justify-between items-center">
                  <span class="text-gray-600">Pacientes Ativos</span>
                  <span class="font-semibold text-blue-600">124</span>
                </div>
                
                <div class="flex justify-between items-center">
                  <span class="text-gray-600">Agendamentos Hoje</span>
                  <span class="font-semibold text-green-600">8</span>
                </div>
                
                <div class="flex justify-between items-center">
                  <span class="text-gray-600">Profissionais</span>
                  <span class="font-semibold text-purple-600">3</span>
                </div>
                
                <div class="flex justify-between items-center">
                  <span class="text-gray-600">Serviços Disponíveis</span>
                  <span class="font-semibold text-orange-600">12</span>
                </div>
              </div>
            </div>

            <!-- Specialties Card -->
            <div class="bg-white rounded-xl shadow-lg p-6">
              <h3 class="text-lg font-semibold text-gray-900 mb-4">Especialidades</h3>
              
              <div class="flex flex-wrap gap-2">
                <span class="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm">Odontologia Estética</span>
                <span class="px-3 py-1 bg-purple-100 text-purple-800 rounded-full text-sm">Harmonização Facial</span>
                <span class="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm">Implantodontia</span>
                <span class="px-3 py-1 bg-orange-100 text-orange-800 rounded-full text-sm">Ortodontia</span>
                <span class="px-3 py-1 bg-pink-100 text-pink-800 rounded-full text-sm">Periodontia</span>
              </div>
              
              <button class="w-full mt-4 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors">
                Gerenciar Especialidades
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class ClinicInfoComponent implements OnInit {
  clinicForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadClinicData();
  }

  private initializeForm(): void {
    this.clinicForm = this.fb.group({
      name: ['DentalSpa Clinic', [Validators.required]],
      description: ['Clínica especializada em odontologia estética e harmonização facial com tecnologia de ponta e profissionais altamente qualificados.'],
      address: ['Rua das Flores, 123 - Centro - São Paulo/SP - CEP: 01234-567'],
      phone: ['(11) 99999-9999', [Validators.required]],
      email: ['contato@dentalspa.com', [Validators.required, Validators.email]],
      website: ['https://www.dentalspa.com'],
      mondayHours: ['08:00 - 18:00'],
      tuesdayHours: ['08:00 - 18:00'],
      wednesdayHours: ['08:00 - 18:00'],
      thursdayHours: ['08:00 - 18:00'],
      fridayHours: ['08:00 - 18:00'],
      saturdayHours: ['08:00 - 14:00'],
      sundayHours: ['Fechado'],
      instagram: ['@dentalspa'],
      facebook: ['DentalSpa Clinic'],
      whatsapp: ['(11) 99999-9999']
    });
  }

  private loadClinicData(): void {
    // Implementar carregamento dos dados da clínica
    this.http.get<ClinicInfo>('/api/clinic-info').subscribe({
      next: (data) => {
        // Preencher formulário com dados carregados
      },
      error: (error) => {
        console.error('Erro ao carregar dados da clínica:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.clinicForm.valid) {
      const formData = this.clinicForm.value;
      
      this.http.put('/api/clinic-info', formData).subscribe({
        next: (response) => {
          console.log('Dados da clínica salvos com sucesso', response);
          // Mostrar mensagem de sucesso
        },
        error: (error) => {
          console.error('Erro ao salvar dados da clínica:', error);
          // Mostrar mensagem de erro
        }
      });
    }
  }
}