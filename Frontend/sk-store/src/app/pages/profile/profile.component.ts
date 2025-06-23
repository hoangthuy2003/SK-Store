import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule, formatDate } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { UserProfile } from '../../models/user.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private notifier = inject(NotificationService);

  // Signals for state management
  isLoading = signal(true);
  isUpdatingProfile = signal(false);
  isChangingPassword = signal(false);
  userProfile = signal<UserProfile | null>(null);

  profileForm: FormGroup;
  passwordForm: FormGroup;

  constructor() {
    this.profileForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^0\d{9}$/)]],
      gender: ['', Validators.required],
      dateOfBirth: ['', Validators.required]
    });

    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile(): void {
    this.isLoading.set(true);
    this.userService.getUserProfile().subscribe({
      next: (profile) => {
        this.userProfile.set(profile);
        this.profileForm.patchValue({
          ...profile,
          // Format date for the input type="date" which requires 'yyyy-MM-dd'
          dateOfBirth: formatDate(profile.dateOfBirth, 'yyyy-MM-dd', 'en-US')
        });
        this.isLoading.set(false);
      },
      error: (err) => {
        this.notifier.showError('Không thể tải thông tin tài khoản.');
        this.isLoading.set(false);
      }
    });
  }

  onUpdateProfile(): void {
    if (this.profileForm.invalid) return;
    this.isUpdatingProfile.set(true);
    this.userService.updateUserProfile(this.profileForm.value).subscribe({
      next: () => {
        this.notifier.showSuccess('Cập nhật thông tin thành công!');
        this.isUpdatingProfile.set(false);
        // Cập nhật lại thông tin hiển thị nếu cần
        this.loadUserProfile(); 
      },
      error: (err) => {
        this.notifier.showError(err.error?.message || 'Cập nhật thất bại.');
        this.isUpdatingProfile.set(false);
      }
    });
  }

  onChangePassword(): void {
    if (this.passwordForm.invalid) return;
    this.isChangingPassword.set(true);
    this.userService.changePassword(this.passwordForm.value).subscribe({
      next: () => {
        this.notifier.showSuccess('Đổi mật khẩu thành công!');
        this.passwordForm.reset();
        this.isChangingPassword.set(false);
      },
      error: (err) => {
        this.notifier.showError(err.error?.message || 'Đổi mật khẩu thất bại.');
        this.isChangingPassword.set(false);
      }
    });
  }

  private passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword')?.value;
    const confirmNewPassword = form.get('confirmNewPassword')?.value;
    if (newPassword !== confirmNewPassword) {
      form.get('confirmNewPassword')?.setErrors({ mismatch: true });
    } else {
      return null;
    }
    return null;
  }
}