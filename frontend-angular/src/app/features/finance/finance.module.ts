import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Components will be created next
// import { FinanceOverviewComponent } from './components/finance-overview.component';
// import { TransactionsListComponent } from './components/transactions-list.component';
// import { TransactionFormComponent } from './components/transaction-form.component';
// import { ReportsComponent } from './components/reports.component';

const routes: Routes = [
  { path: '', redirectTo: '/finance/overview', pathMatch: 'full' },
  // { path: 'overview', component: FinanceOverviewComponent },
  // { path: 'transactions', component: TransactionsListComponent },
  // { path: 'new-transaction', component: TransactionFormComponent },
  // { path: 'reports', component: ReportsComponent }
];

@NgModule({
  declarations: [
    // FinanceOverviewComponent,
    // TransactionsListComponent,
    // TransactionFormComponent,
    // ReportsComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class FinanceModule { }