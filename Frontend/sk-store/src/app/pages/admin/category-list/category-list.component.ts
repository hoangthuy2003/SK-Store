import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpResponse } from '@angular/common/http';
import { CategoryService } from '../../../services/category.service';
import { NotificationService } from '../../../services/notification.service';
import { CategoryDto } from '../../../models/category.model';
import { PaginationComponent } from '../../../components/pagination/pagination.component';
import { CategoryModalComponent } from '../category-modal/category-modal.component'; // <<< THÊM IMPORT

@Component({
  selector: 'app-admin-category-list',
  standalone: true,
  // <<< THÊM CategoryModalComponent VÀO IMPORTS >>>
  imports: [CommonModule, PaginationComponent, CategoryModalComponent],
  templateUrl: './category-list.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminCategoryListComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private notifier = inject(NotificationService);
  
  categories = signal<CategoryDto[]>([]);
  isLoading = signal(true);
  
  currentPage = signal(1);
  pageSize = 6;
  totalItems = signal(0);

  // <<< THÊM CÁC STATE CHO MODAL >>>
  isModalOpen = signal(false);
  selectedCategory = signal<CategoryDto | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.categoryService.getPagedCategories(this.currentPage(), this.pageSize).subscribe({
      next: (response: HttpResponse<CategoryDto[]>) => {
        this.categories.set(response.body || []);
        const totalCount = response.headers.get('X-Total-Count');
        this.totalItems.set(totalCount ? +totalCount : 0);
        this.isLoading.set(false);
      },
      error: () => {
        this.notifier.showError("Không thể tải danh sách danh mục.");
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadCategories();
  }

  // <<< THÊM CÁC HÀM XỬ LÝ MODAL >>>
  openModal(category: CategoryDto | null = null): void {
    this.selectedCategory.set(category);
    this.isModalOpen.set(true);
  }

  handleModalClose(shouldReload: boolean): void {
    this.isModalOpen.set(false);
    if (shouldReload) {
      this.loadCategories();
    }
  }

  onDelete(id: number): void {
    // Bạn có thể thay thế confirm() bằng một modal xác nhận đẹp hơn ở bước sau
    if (confirm('Bạn có chắc muốn xóa danh mục này? Thao tác này không thể hoàn tác.')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          this.notifier.showSuccess('Xóa danh mục thành công!');
          this.loadCategories();
        },
        error: (err) => this.notifier.showError(err.message)
      });
    }
  }
}