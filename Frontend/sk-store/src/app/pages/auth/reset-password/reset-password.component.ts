import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notifier = inject(NotificationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  isLoading = signal(false);
  resetPasswordForm: FormGroup;

  constructor() {
    this.resetPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      otp: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const email = params['email'];
      if (email) {
        this.resetPasswordForm.get('email')?.setValue(email);
      } else {
        this.notifier.showError('Không tìm thấy thông tin email. Vui lòng thử lại từ bước Quên mật khẩu.');
        this.router.navigate(['/forgot-password']);
      }
    });
  }

  onSubmit(): void {
    if (this.resetPasswordForm.invalid) return;
    this.isLoading.set(true);

    this.authService.resetPassword(this.resetPasswordForm.value).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.notifier.showSuccess(res.message || 'Mật khẩu đã được đặt lại thành công!');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.notifier.showError(err.error?.message || 'Đã có lỗi xảy ra.');
      }
    });
  }
}