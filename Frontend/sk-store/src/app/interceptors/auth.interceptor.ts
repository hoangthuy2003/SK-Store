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
      // Lấy thông điệp lỗi từ response của backend
      // API của bạn trả về lỗi trong `error.error.message`
      let errorMessage = error.error?.message || 'Đã có lỗi không mong muốn xảy ra.';

      // Xử lý các trường hợp đặc biệt
      if (error.status === 0) {
        errorMessage = 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại mạng.';
      } else if (error.status === 401) {
        // Lỗi 401 (Unauthorized) có thể do token hết hạn hoặc không hợp lệ
        // Không hiển thị toast cho lỗi này vì thường sẽ có logic redirect về trang login
        // authService.logout(); // Có thể tự động logout ở đây
        errorMessage = 'Phiên đăng nhập đã hết hạn hoặc không hợp lệ.';
      } else if (error.status === 403) {
        errorMessage = 'Bạn không có quyền thực hiện hành động này.';
      }

      // Hiển thị thông báo lỗi
      notificationService.showError(errorMessage);

      // Ném lỗi lại để các service/component khác có thể xử lý thêm nếu cần
      return throwError(() => error);
    })
  );
};