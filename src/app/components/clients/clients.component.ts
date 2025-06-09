import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClientService } from '../../services/client.service';

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.scss']
})
export class ClientsComponent implements OnInit {
  clients: any[] = [];
  filteredClients: any[] = [];
  clientForm: FormGroup;
  selectedClient: any = null;
  showModal = false;
  isEditing = false;
  loading = false;
  searchTerm = '';

  constructor(
    private clientService: ClientService,
    private fb: FormBuilder
  ) {
    this.clientForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      cpf: [''],
      address: [''],
      city: [''],
      state: [''],
      zipCode: [''],
      birthDate: [''],
      notes: ['']
    });
  }

  ngOnInit() {
    this.loadClients();
  }

  loadClients() {
    this.loading = true;
    this.clientService.getAll().subscribe({
      next: (data) => {
        this.clients = data;
        this.filteredClients = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar clientes:', error);
        this.loading = false;
      }
    });
  }

  filterClients() {
    if (!this.searchTerm) {
      this.filteredClients = this.clients;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredClients = this.clients.filter(client =>
        client.name?.toLowerCase().includes(term) ||
        client.email?.toLowerCase().includes(term) ||
        client.phone?.includes(term)
      );
    }
  }

  openAddModal() {
    this.isEditing = false;
    this.selectedClient = null;
    this.clientForm.reset();
    this.showModal = true;
  }

  editClient(client: any) {
    this.isEditing = true;
    this.selectedClient = client;
    this.clientForm.patchValue(client);
    this.showModal = true;
  }

  deleteClient(id: number) {
    if (confirm('Tem certeza que deseja excluir este cliente?')) {
      this.clientService.delete(id).subscribe({
        next: () => {
          this.loadClients();
        },
        error: (error) => {
          console.error('Erro ao excluir cliente:', error);
        }
      });
    }
  }

  saveClient() {
    if (this.clientForm.valid) {
      const clientData = this.clientForm.value;
      
      if (this.isEditing) {
        this.clientService.update(this.selectedClient.id, clientData).subscribe({
          next: () => {
            this.closeModal();
            this.loadClients();
          },
          error: (error) => {
            console.error('Erro ao atualizar cliente:', error);
          }
        });
      } else {
        this.clientService.create(clientData).subscribe({
          next: () => {
            this.closeModal();
            this.loadClients();
          },
          error: (error) => {
            console.error('Erro ao criar cliente:', error);
          }
        });
      }
    }
  }

  closeModal() {
    this.showModal = false;
    this.selectedClient = null;
    this.clientForm.reset();
  }
}