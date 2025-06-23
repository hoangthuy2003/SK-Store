import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminProductListComponent } from './product-list/product-list.component';
// Import các component admin khác ở đây khi tạo
// import { AdminDashboardComponent } from './dashboard/dashboard.component';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      // { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      // { path: 'dashboard', component: AdminDashboardComponent },
      { path: '', redirectTo: 'products', pathMatch: 'full' }, // Tạm thời trỏ về products
      { path: 'products', component: AdminProductListComponent },
      // Thêm các route cho user, order... ở đây
    ]
  }
];