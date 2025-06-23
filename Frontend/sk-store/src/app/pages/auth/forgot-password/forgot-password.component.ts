import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notifier = inject(NotificationService);
  private router = inject(Router);

  isLoading = signal(false);
  forgotPasswordForm: FormGroup;

  constructor() {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotPasswordForm.invalid) return;
    this.isLoading.set(true);

    this.authService.forgotPassword(this.forgotPasswordForm.value).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.notifier.showSuccess(res.message || 'Yêu cầu đã được gửi đi.');
        // Chuyển hướng đến trang reset với email trong query params
        this.router.navigate(['/reset-password'], { queryParams: { email: this.forgotPasswordForm.get('email')?.value } });
      },
      error: (err) => {
        this.isLoading.set(false);
        this.notifier.showError(err.error?.message || 'Đã có lỗi xảy ra.');
      }
    });
  }
}