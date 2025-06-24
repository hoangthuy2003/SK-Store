import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
// Thêm ActivatedRoute vào đây nếu chưa có
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { LoginRequest, AuthResponse, AuthValidation } from '../../../models/auth.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  hidePassword = signal(true);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute, // Đảm bảo đã inject ActivatedRoute
    private notifier: NotificationService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    // Logic ngOnInit giữ nguyên
    if (this.authService.isAuthenticated()) {
      // Nếu đã đăng nhập, kiểm tra vai trò và điều hướng
      if (this.authService.isUserInRole('Admin')) {
        this.router.navigate(['/admin/dashboard']);
      } else {
        this.router.navigate(['/']);
      }
      return;
    }

    this.route.queryParams.subscribe(params => {
      if (params['message']) {
        this.successMessage.set(params['message']);
      }
      if (params['registered']) {
        this.successMessage.set('Đăng ký thành công! Vui lòng đăng nhập.');
      }
    });
  }

  // getErrorMessage và các hàm khác giữ nguyên
  getErrorMessage(controlName: string): string {
    // ...
    return '';
  }

  // =================================================================
  // THAY THẾ TOÀN BỘ PHƯƠNG THỨC onSubmit BẰNG PHIÊN BẢN NÀY
  // =================================================================
  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    this.isLoading.set(true);

    const loginData: LoginRequest = {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value
    };

    this.authService.login(loginData).subscribe({
      next: (response: AuthResponse) => {
        this.isLoading.set(false);
        this.notifier.showSuccess('Đăng nhập thành công!');

        // **LOGIC ĐIỀU HƯỚNG MỚI**
        // Kiểm tra vai trò của người dùng vừa đăng nhập
        if (this.authService.isUserInRole('Admin')) {
          // Nếu là Admin, luôn điều hướng đến trang dashboard
          this.router.navigate(['/admin/dashboard']);
        } else {
          // Nếu là người dùng thường, kiểm tra xem có returnUrl không
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
          this.router.navigateByUrl(returnUrl);
        }
      },
      error: (error) => {
        this.isLoading.set(false);
        // Logic xử lý lỗi giữ nguyên
        if (error.error?.validation) {
          const validation = error.error.validation as AuthValidation;
          if (validation.email) {
            this.loginForm.get('email')?.setErrors({ serverError: validation.email });
          }
          if (validation.password) {
            this.loginForm.get('password')?.setErrors({ serverError: validation.password });
          }
        } else {
          this.errorMessage.set(error.error?.message || 'Đăng nhập thất bại. Vui lòng thử lại.');
        }
      }
    });
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(value => !value);
  }
}