import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

declare var bootstrap: any;

interface Service {
  id?: number;
  name: string;
  description?: string;
  price: number;
  duration_minutes: number;
  category: string;
  is_active?: boolean;
  created_at?: string;
}

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html'
})
export class ServicesComponent implements OnInit {
  services: Service[] = [];
  filteredServices: Service[] = [];
  searchTerm = '';
  categoryFilter = '';
  loading = true;
  saving = false;
  isEditing = false;
  
  serviceForm: FormGroup;
  serviceModal: any;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.serviceForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      price: ['', [Validators.required, Validators.min(0)]],
      duration_minutes: ['', [Validators.required, Validators.min(15)]],
      category: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadServices();
  }

  loadServices() {
    this.loading = true;
    this.apiService.get<Service[]>('/services').subscribe({
      next: (services) => {
        this.services = services;
        this.filteredServices = services;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading services:', error);
        this.loading = false;
      }
    });
  }

  filterServices() {
    let filtered = this.services;

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(service => 
        service.name.toLowerCase().includes(term) ||
        service.description?.toLowerCase().includes(term) ||
        service.category.toLowerCase().includes(term)
      );
    }

    if (this.categoryFilter) {
      filtered = filtered.filter(service => 
        service.category.toLowerCase() === this.categoryFilter.toLowerCase()
      );
    }

    this.filteredServices = filtered;
  }

  getUniqueCategories(): string[] {
    const categories = [...new Set(this.services.map(service => service.category))];
    return categories.sort();
  }

  getServicesByCategory(category: string): Service[] {
    return this.services.filter(service => service.category === category);
  }

  getTotalRevenuePotential(): number {
    return this.services.reduce((total, service) => total + Number(service.price), 0);
  }

  openAddServiceModal() {
    this.isEditing = false;
    this.serviceForm.reset();
    this.openModal();
  }

  editService(service: Service) {
    this.isEditing = true;
    this.serviceForm.patchValue({
      name: service.name,
      description: service.description,
      price: service.price,
      duration_minutes: service.duration_minutes,
      category: service.category
    });
    this.openModal();
  }

  viewServiceStats(service: Service) {
    console.log('Viewing stats for service:', service);
  }

  scheduleService(service: Service) {
    console.log('Scheduling service:', service);
  }

  deactivateService(service: Service) {
    this.updateServiceStatus(service.id!, false);
  }

  activateService(service: Service) {
    this.updateServiceStatus(service.id!, true);
  }

  updateServiceStatus(serviceId: number, isActive: boolean) {
    this.apiService.put(`/services/${serviceId}`, { is_active: isActive }).subscribe({
      next: () => {
        this.loadServices();
      },
      error: (error) => {
        console.error('Error updating service status:', error);
      }
    });
  }

  saveService() {
    if (!this.serviceForm.valid) return;

    this.saving = true;
    const serviceData = this.serviceForm.value;

    const request = this.isEditing 
      ? this.apiService.put<Service>(`/services/${serviceData.id}`, serviceData)
      : this.apiService.post<Service>('/services', serviceData);

    request.subscribe({
      next: () => {
        this.saving = false;
        this.closeModal();
        this.loadServices();
      },
      error: (error) => {
        console.error('Error saving service:', error);
        this.saving = false;
      }
    });
  }

  openModal() {
    this.serviceModal = new bootstrap.Modal(document.getElementById('serviceModal'));
    this.serviceModal.show();
  }

  closeModal() {
    if (this.serviceModal) {
      this.serviceModal.hide();
    }
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}