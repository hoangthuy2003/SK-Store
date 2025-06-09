// src/app/guards/auth.guard.ts

import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Đây là một CanActivateFn, một functional guard
export const AuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true; // Cho phép truy cập
  }

  // Nếu chưa đăng nhập, chuyển hướng về trang login và lưu lại URL người dùng định đến
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false; // Không cho phép truy cập
};