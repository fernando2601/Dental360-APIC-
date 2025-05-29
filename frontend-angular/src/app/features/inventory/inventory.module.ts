import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Components will be created next
// import { InventoryListComponent } from './components/inventory-list.component';
// import { InventoryDetailsComponent } from './components/inventory-details.component';
// import { InventoryFormComponent } from './components/inventory-form.component';
// import { StockMovementsComponent } from './components/stock-movements.component';

const routes: Routes = [
  { path: '', redirectTo: '/inventory/list', pathMatch: 'full' },
  // { path: 'list', component: InventoryListComponent },
  // { path: 'new', component: InventoryFormComponent },
  // { path: ':id', component: InventoryDetailsComponent },
  // { path: 'edit/:id', component: InventoryFormComponent },
  // { path: 'movements', component: StockMovementsComponent }
];

@NgModule({
  declarations: [
    // InventoryListComponent,
    // InventoryDetailsComponent,
    // InventoryFormComponent,
    // StockMovementsComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class InventoryModule { }