import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminProductListComponent } from './product-list/product-list.component';
import { AdminProductEditComponent } from './product-edit/product-edit.component';
import { AdminCategoryListComponent } from './category-list/category-list.component';
import { AdminBrandListComponent } from './brand-list/brand-list.component';
import { AdminUserListComponent } from './user-list/user-list.component';
import { AdminOrderListComponent } from './order-list/order-list.component';
import { AdminDashboardComponent } from './dashboard/dashboard.component';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'products', component: AdminProductListComponent },
      { path: 'products/new', component: AdminProductEditComponent },
      { path: 'products/edit/:id', component: AdminProductEditComponent },
      { path: 'orders', component: AdminOrderListComponent },
      
      // <<< ĐẢM BẢO PHẦN CATEGORY CHỈ CÓ 1 DÒNG DUY NHẤT NHƯ THẾ NÀY >>>
      { path: 'categories', component: AdminCategoryListComponent },
      
      { path: 'users', component: AdminUserListComponent },
      { path: 'brands', component: AdminBrandListComponent },
      
    ]
  }
];