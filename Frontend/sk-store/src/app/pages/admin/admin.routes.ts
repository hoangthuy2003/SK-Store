import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminProductListComponent } from './product-list/product-list.component';
import { AdminProductEditComponent } from './product-edit/product-edit.component';
import { AdminCategoryListComponent } from './category-list/category-list.component';
import { AdminCategoryFormComponent } from './category-form/category-form.component';
import { AdminBrandListComponent } from './brand-list/brand-list.component';
import { AdminBrandFormComponent } from './brand-form/brand-form.component';
import { AdminUserListComponent } from './user-list/user-list.component';
import { AdminOrderListComponent } from './order-list/order-list.component';
export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', redirectTo: 'products', pathMatch: 'full' },
      { path: 'products', component: AdminProductListComponent },
      { path: 'products/new', component: AdminProductEditComponent },
      { path: 'products/edit/:id', component: AdminProductEditComponent },
      { path: 'orders', component: AdminOrderListComponent },
      { path: 'categories', component: AdminCategoryListComponent },
      { path: 'categories/new', component: AdminCategoryFormComponent },
      { path: 'categories/edit/:id', component: AdminCategoryFormComponent },
 { path: 'users', component: AdminUserListComponent },
      { path: 'brands', component: AdminBrandListComponent },
      { path: 'brands/new', component: AdminBrandFormComponent },
      { path: 'brands/edit/:id', component: AdminBrandFormComponent },
    ]
  }
];