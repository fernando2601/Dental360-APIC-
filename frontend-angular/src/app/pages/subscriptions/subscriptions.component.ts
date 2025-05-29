import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface Subscription {
  id: number;
  name: string;
  price: string;
  features: string[];
  status: 'active' | 'inactive' | 'pending';
  startDate: string;
  endDate: string;
  billingCycle: 'monthly' | 'yearly';
}

@Component({
  selector: 'app-subscriptions',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-purple-50 to-indigo-100 p-6">
      <div class="max-w-7xl mx-auto">
        <!-- Header -->
        <div class="mb-8">
          <h1 class="text-4xl font-bold text-gray-900 mb-2">Assinatura</h1>
          <p class="text-gray-600">Gerencie seu plano de assinatura e funcionalidades</p>
        </div>

        <!-- Current Plan Card -->
        <div class="bg-white rounded-xl shadow-lg p-8 mb-8">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-2xl font-semibold text-gray-900">Plano Atual</h2>
            <span class="px-4 py-2 bg-green-100 text-green-800 rounded-full text-sm font-medium">
              Ativo
            </span>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="text-center">
              <div class="text-3xl font-bold text-purple-600 mb-2">DentalSpa Pro</div>
              <div class="text-gray-500">Plano Premium</div>
            </div>
            <div class="text-center">
              <div class="text-3xl font-bold text-gray-900 mb-2">R$ 299</div>
              <div class="text-gray-500">por mês</div>
            </div>
            <div class="text-center">
              <div class="text-3xl font-bold text-green-600 mb-2">∞</div>
              <div class="text-gray-500">Usuários ilimitados</div>
            </div>
          </div>

          <div class="mt-8 grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <h3 class="font-semibold text-gray-900 mb-4">Funcionalidades Incluídas</h3>
              <ul class="space-y-2">
                <li class="flex items-center text-gray-700">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  Gestão completa de pacientes
                </li>
                <li class="flex items-center text-gray-700">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  Agendamento inteligente
                </li>
                <li class="flex items-center text-gray-700">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  Gestão financeira avançada
                </li>
                <li class="flex items-center text-gray-700">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  Controle de estoque
                </li>
                <li class="flex items-center text-gray-700">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  Galeria Antes & Depois
                </li>
                <li class="flex items-center text-gray-700">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  Suporte prioritário 24/7
                </li>
              </ul>
            </div>
            
            <div>
              <h3 class="font-semibold text-gray-900 mb-4">Detalhes da Assinatura</h3>
              <div class="space-y-3">
                <div class="flex justify-between">
                  <span class="text-gray-600">Data de Início:</span>
                  <span class="font-medium">01/01/2024</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-gray-600">Próxima Cobrança:</span>
                  <span class="font-medium">01/02/2024</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-gray-600">Método de Pagamento:</span>
                  <span class="font-medium">Cartão •••• 4242</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-gray-600">Status:</span>
                  <span class="font-medium text-green-600">Ativo</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Usage Statistics -->
        <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div class="bg-white rounded-xl shadow-md p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-gray-600 text-sm">Pacientes</p>
                <p class="text-2xl font-bold text-gray-900">124</p>
              </div>
              <div class="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
                <svg class="w-6 h-6 text-blue-600" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
            </div>
            <p class="text-sm text-gray-500 mt-2">de ∞ disponíveis</p>
          </div>

          <div class="bg-white rounded-xl shadow-md p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-gray-600 text-sm">Agendamentos</p>
                <p class="text-2xl font-bold text-gray-900">89</p>
              </div>
              <div class="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
                <svg class="w-6 h-6 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clip-rule="evenodd"></path>
                </svg>
              </div>
            </div>
            <p class="text-sm text-gray-500 mt-2">este mês</p>
          </div>

          <div class="bg-white rounded-xl shadow-md p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-gray-600 text-sm">Armazenamento</p>
                <p class="text-2xl font-bold text-gray-900">2.4GB</p>
              </div>
              <div class="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
                <svg class="w-6 h-6 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z"></path>
                </svg>
              </div>
            </div>
            <p class="text-sm text-gray-500 mt-2">de 100GB disponíveis</p>
          </div>

          <div class="bg-white rounded-xl shadow-md p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-gray-600 text-sm">Usuários</p>
                <p class="text-2xl font-bold text-gray-900">3</p>
              </div>
              <div class="w-12 h-12 bg-orange-100 rounded-full flex items-center justify-center">
                <svg class="w-6 h-6 text-orange-600" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM9 9a3 3 0 11-6 0 3 3 0 016 0zM17 9a3 3 0 11-6 0 3 3 0 016 0zM12.93 14.07a.75.75 0 00-1.86 0l-.065.211a.75.75 0 01-1.42 0l-.064-.211a.75.75 0 00-1.86 0l-.064.211a.75.75 0 01-1.42 0l-.065-.211a.75.75 0 00-1.86 0A3 3 0 003 17v1a1 1 0 001 1h16a1 1 0 001-1v-1a3 3 0 00-5.07-2.93z"></path>
                </svg>
              </div>
            </div>
            <p class="text-sm text-gray-500 mt-2">de ∞ disponíveis</p>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex gap-4">
          <button class="bg-purple-600 hover:bg-purple-700 text-white font-medium py-3 px-6 rounded-lg transition-colors">
            Gerenciar Pagamento
          </button>
          <button class="bg-gray-100 hover:bg-gray-200 text-gray-700 font-medium py-3 px-6 rounded-lg transition-colors">
            Baixar Fatura
          </button>
          <button class="bg-red-100 hover:bg-red-200 text-red-700 font-medium py-3 px-6 rounded-lg transition-colors">
            Cancelar Assinatura
          </button>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class SubscriptionsComponent implements OnInit {

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    // Implementar carregamento de dados de assinatura
  }

}