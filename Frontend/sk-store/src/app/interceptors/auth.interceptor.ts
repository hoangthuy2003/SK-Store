import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service'; // <-- IMPORT
import { catchError, throwError } from 'rxjs';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService); // <-- INJECT

  if (isPlatformBrowser(platformId)) {
    const token = authService.getToken();
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }
  }
  
  // Sử dụng pipe để bắt lỗi
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Chỉ xử lý và hiển thị thông báo cho các lỗi liên quan đến authentication/authorization
      if (error.status === 401) {
        // Lỗi 401 (Unauthorized) có thể do token hết hạn hoặc không hợp lệ
        const errorMessage = 'Phiên đăng nhập đã hết hạn hoặc không hợp lệ.';
        notificationService.showError(errorMessage);
        // authService.logout(); // Có thể tự động logout ở đây
      } else if (error.status === 403) {
        const errorMessage = 'Bạn không có quyền thực hiện hành động này.';
        notificationService.showError(errorMessage);
      } else if (error.status === 0) {
        const errorMessage = 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại mạng.';
        notificationService.showError(errorMessage);
      }
      // Không hiển thị thông báo cho các lỗi khác (400, 500, etc.) 
      // để component tự xử lý

      // Ném lỗi lại để các service/component khác có thể xử lý thêm nếu cần
      return throwError(() => error);
    })
  );
};