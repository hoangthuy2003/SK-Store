import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';

// Models & Services
import { UserDto, UserFilterParameters } from '../../../models/user.model';
import { AdminUserService } from '../../../services/admin-user.service';
import { NotificationService } from '../../../services/notification.service';

// Components
import { PaginationComponent } from '../../../components/pagination/pagination.component';
import { AdminUserEditModalComponent } from '../user-edit-modal/user-edit-modal.component';

@Component({
  selector: 'app-admin-user-list',
  standalone: true,
  imports: [CommonModule, PaginationComponent, AdminUserEditModalComponent],
  templateUrl: './user-list.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminUserListComponent implements OnInit {
  private adminUserService = inject(AdminUserService);
  private notifier = inject(NotificationService);

  // State
  isLoading = signal(true);
  users = signal<UserDto[]>([]);
  
  // Pagination
  currentPage = signal(1);
  pageSize = 10;
  totalUsers = signal(0);

  // Modal State
  isEditModalOpen = signal(false);
  selectedUser = signal<UserDto | null>(null);

  ngOnInit(): void {
    this.fetchUsers();
  }

  // ... (bên trong class AdminUserListComponent)

fetchUsers(): void {
  this.isLoading.set(true);
  // <<< SỬA LẠI ĐỐI TƯỢNG FILTERS Ở ĐÂY >>>
  const filters: UserFilterParameters = {
    pageNumber: this.currentPage(),
    pageSize: this.pageSize,
    // Cung cấp giá trị mặc định cho các thuộc tính còn lại
    sortBy: 'RegistrationDate',
    sortDirection: 'desc'
  };

  this.adminUserService.getUsers(filters).subscribe({
    next: (response: HttpResponse<UserDto[]>) => {
      this.users.set(response.body || []);
      const totalCount = response.headers.get('X-Total-Count');
      this.totalUsers.set(totalCount ? +totalCount : 0);
      this.isLoading.set(false);
    },
    error: (err) => {
      this.notifier.showError("Không thể tải danh sách người dùng.");
      this.isLoading.set(false);
    }
  });
}

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.fetchUsers();
  }

  openEditModal(user: UserDto): void {
    this.selectedUser.set(user);
    this.isEditModalOpen.set(true);
  }

  closeEditModal(shouldReload: boolean): void {
    this.isEditModalOpen.set(false);
    this.selectedUser.set(null);
    if (shouldReload) {
      this.fetchUsers();
    }
  }
}