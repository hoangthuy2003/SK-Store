import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';

// Import các layout component
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';

// Import các page component
import { LoginComponent } from './pages/auth/login/login.component';
import { RegisterComponent } from './pages/auth/register/register.component';
import { ForgotPasswordComponent } from './pages/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './pages/auth/reset-password/reset-password.component';
import { HomepageComponent } from './pages/homepage/homepage.component';
import { ShopComponent } from './pages/shop/shop.component';
import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { CartComponent } from './pages/cart/cart.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { OrderListComponent } from './pages/order-list/order-list.component';
import { OrderDetailComponent } from './pages/order-detail/order-detail.component';
import { ProfileComponent } from './pages/profile/profile.component';

export const routes: Routes = [
  // Route này không có layout cha, nó sẽ tự quản lý layout của riêng mình
  {
    path: 'admin',
    canActivate: [AuthGuard, AdminGuard],
    loadChildren: () => import('./pages/admin/admin.routes').then(m => m.ADMIN_ROUTES)
  },

  // Tất cả các route bên dưới sẽ được hiển thị bên trong MainLayoutComponent
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      // Public routes
      { path: '', component: HomepageComponent },
      { path: 'shop', component: ShopComponent },
      { path: 'products/:id', component: ProductDetailComponent },

      // Auth routes
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent },
      { path: 'reset-password', component: ResetPasswordComponent },

      // Protected user routes
      { path: 'cart', component: CartComponent, canActivate: [AuthGuard] },
      { path: 'checkout', component: CheckoutComponent, canActivate: [AuthGuard] },
      { path: 'orders', component: OrderListComponent, canActivate: [AuthGuard] },
      { path: 'orders/:id', component: OrderDetailComponent, canActivate: [AuthGuard] },
      { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
    ]
  },

  {
    path: '**',
    redirectTo: '' // Hoặc chuyển đến trang 404 tùy chỉnh
  }
];