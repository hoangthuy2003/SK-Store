import { Component, OnInit, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

// Models & Services
import { UserDto, RoleDto, UserForAdminUpdateDto } from '../../../models/user.model';
import { AdminUserService } from '../../../services/admin-user.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-admin-user-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-edit-modal.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminUserEditModalComponent implements OnInit {
  @Input({ required: true }) user!: UserDto;
  @Output() close = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private adminUserService = inject(AdminUserService);
  private notifier = inject(NotificationService);

  userForm: FormGroup;
  isSaving = signal(false);
  roles = signal<RoleDto[]>([]);

  constructor() {
    this.userForm = this.fb.group({
      roleId: ['', Validators.required],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.adminUserService.getRoles().subscribe(data => this.roles.set(data));
    if (this.user) {
      this.userForm.patchValue({
        roleId: this.user.roleId,
        isActive: this.user.isActive
      });
    }
  }

  onSubmit(): void {
    if (this.userForm.invalid) return;
    this.isSaving.set(true);

    const updateData: UserForAdminUpdateDto = this.userForm.value;

    this.adminUserService.updateUser(this.user.userId, updateData).subscribe({
      next: () => {
        this.notifier.showSuccess('Cập nhật người dùng thành công!');
        this.isSaving.set(false);
        this.close.emit(true); // Gửi tín hiệu cần tải lại danh sách
      },
      error: (err) => {
        this.notifier.showError('Cập nhật thất bại.');
        this.isSaving.set(false);
      }
    });
  }
}