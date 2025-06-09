import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { LoginRequest, AuthResponse, AuthValidation } from '../../../models/auth.model';

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
    private route: ActivatedRoute
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    // Kiểm tra nếu đã đăng nhập thì chuyển về trang chủ
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/']);
      return;
    }

    // Lấy thông báo từ query params nếu có
    this.route.queryParams.subscribe(params => {
      if (params['message']) {
        this.successMessage.set(params['message']);
      }
    });

    // Check for success message in query params
    this.route.queryParams.subscribe(params => {
      if (params['registered']) {
        this.successMessage.set('Đăng ký thành công! Vui lòng đăng nhập.');
      }
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.loginForm.get(controlName);
    if (!control) return '';

    if (control.hasError('required')) {
      return 'Vui lòng nhập thông tin này';
    }
    if (control.hasError('email')) {
      return 'Email không đúng định dạng';
    }
    return '';
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const loginData: LoginRequest = {
      email: this.loginForm.get('email')?.value,
      password: this.loginForm.get('password')?.value
    };

    this.authService.login(loginData).subscribe({
      next: (response: AuthResponse) => {
        this.isLoading.set(false);
        this.router.navigate(['/']);
      },
      error: (error) => {
        this.isLoading.set(false);
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