import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html'
})
export class ServicesComponent implements OnInit {
  services: any[] = [];
  filteredServices: any[] = [];
  searchTerm = '';
  categoryFilter = '';
  loading = true;
  saving = false;
  isEditing = false;
  showModal = false;
  activeMenuId: number | null = null;
  
  serviceForm: FormGroup;
  selectedService: any = null;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.serviceForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      price: ['', [Validators.required, Validators.min(0)]],
      duration_minutes: ['', [Validators.required, Validators.min(15)]],
      category: ['', Validators.required],
      is_active: [true]
    });
  }

  ngOnInit() {
    this.loadServices();
  }

  loadServices() {
    this.loading = true;
    this.apiService.get('/services').subscribe({
      next: (services: any[]) => {
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
        service.category === this.categoryFilter
      );
    }

    this.filteredServices = filtered;
  }

  getUniqueCategories(): string[] {
    const categories = [...new Set(this.services.map(service => service.category))];
    return categories.sort();
  }

  toggleServiceMenu(serviceId: number) {
    this.activeMenuId = this.activeMenuId === serviceId ? null : serviceId;
  }

  openAddServiceModal() {
    this.isEditing = false;
    this.selectedService = null;
    this.serviceForm.reset({
      is_active: true
    });
    this.showModal = true;
    this.activeMenuId = null;
  }

  editService(service: any) {
    this.isEditing = true;
    this.selectedService = service;
    this.serviceForm.patchValue({
      name: service.name,
      description: service.description,
      price: service.price,
      duration_minutes: service.duration_minutes,
      category: service.category,
      is_active: service.is_active
    });
    this.showModal = true;
    this.activeMenuId = null;
  }

  duplicateService(service: any) {
    this.isEditing = false;
    this.selectedService = null;
    this.serviceForm.patchValue({
      name: service.name + ' (Cópia)',
      description: service.description,
      price: service.price,
      duration_minutes: service.duration_minutes,
      category: service.category,
      is_active: true
    });
    this.showModal = true;
    this.activeMenuId = null;
  }

  deleteService(service: any) {
    if (confirm(`Tem certeza que deseja excluir o serviço "${service.name}"?`)) {
      this.apiService.delete(`/services/${service.id}`).subscribe({
        next: () => {
          this.loadServices();
        },
        error: (error) => {
          console.error('Error deleting service:', error);
        }
      });
    }
    this.activeMenuId = null;
  }

  saveService() {
    if (!this.serviceForm.valid) return;

    this.saving = true;
    const serviceData = this.serviceForm.value;

    const request = this.isEditing 
      ? this.apiService.put(`/services/${this.selectedService?.id}`, serviceData)
      : this.apiService.post('/services', serviceData);

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

  closeModal() {
    this.showModal = false;
    this.activeMenuId = null;
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}