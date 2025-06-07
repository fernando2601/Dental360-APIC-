import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-clients',
  template: `
    <div>
      <h2><i class="fas fa-users me-2"></i>Clientes</h2>
      <p>Gestão de clientes em desenvolvimento...</p>
    </div>
  `
})
export class ClientsComponent implements OnInit {
  constructor(private apiService: ApiService) {}
  
  ngOnInit() {}
}