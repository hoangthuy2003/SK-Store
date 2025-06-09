// src/app/guards/admin.guard.ts

import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const AdminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // isAuthenticated đã kiểm tra token hợp lệ
  if (authService.isAuthenticated() && authService.isUserInRole('Admin')) {
    return true;
  }
  
  // Nếu không phải admin, chuyển về trang chủ hoặc trang "access denied"
  router.navigate(['/']); 
  return false;
};