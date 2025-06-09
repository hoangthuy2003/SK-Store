import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { RegisterRequest, AuthResponse, AuthValidation } from '../../../models/auth.model';

type RegisterFormKeys = 'email' | 'password' | 'confirmPassword' | 'firstName' | 'lastName' | 'phoneNumber' | 'gender' | 'dateOfBirth';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  hidePassword = signal(true);
  hideConfirmPassword = signal(true);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^0\d{9}$/)]],
      gender: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    // Nếu đã đăng nhập, chuyển hướng về trang chủ
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/']);
    }
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword?.setErrors(null);
    }
  }

  getErrorMessage(controlName: string): string {
    const control = this.registerForm.get(controlName);
    if (!control) return '';

    if (control.hasError('required')) {
      return 'Vui lòng nhập thông tin này';
    }
    if (control.hasError('email')) {
      return 'Email không đúng định dạng';
    }
    if (control.hasError('minlength')) {
      const minLength = control.errors?.['minlength'].requiredLength;
      return `Phải có ít nhất ${minLength} ký tự`;
    }
    if (control.hasError('maxlength')) {
      const maxLength = control.errors?.['maxlength'].requiredLength;
      return `Không được vượt quá ${maxLength} ký tự`;
    }
    if (control.hasError('pattern')) {
      if (controlName === 'phoneNumber') {
        return 'Số điện thoại phải bắt đầu bằng số 0 và có 10 chữ số';
      }
      return 'Định dạng không hợp lệ';
    }
    if (control.hasError('passwordMismatch')) {
      return 'Mật khẩu xác nhận không khớp';
    }
    return '';
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched(this.registerForm);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    const registerData: RegisterRequest = {
      email: this.registerForm.get('email')?.value,
      password: this.registerForm.get('password')?.value,
      firstName: this.registerForm.get('firstName')?.value,
      lastName: this.registerForm.get('lastName')?.value,
      phoneNumber: this.registerForm.get('phoneNumber')?.value,
      gender: this.registerForm.get('gender')?.value,
      dateOfBirth: this.registerForm.get('dateOfBirth')?.value
    };

    this.authService.register(registerData).subscribe({
      next: (response: AuthResponse) => {
        this.isLoading.set(false);
        this.successMessage.set('Đăng ký thành công! Vui lòng đăng nhập.');
        setTimeout(() => {
          this.router.navigate(['/login'], { queryParams: { registered: true } });
        }, 2000);
      },
      error: (error) => {
        this.isLoading.set(false);
        if (error.error?.validation) {
          const validation = error.error.validation as AuthValidation;
          (Object.keys(validation) as RegisterFormKeys[]).forEach(key => {
            const control = this.registerForm.get(key);
            if (control && validation[key]) {
              control.setErrors({ serverError: validation[key] });
            }
          });
        } else {
          this.errorMessage.set(error.error?.message || 'Đăng ký thất bại. Vui lòng thử lại.');
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

  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword.update(value => !value);
  }
} 