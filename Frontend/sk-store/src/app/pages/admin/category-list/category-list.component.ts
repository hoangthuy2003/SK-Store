import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpResponse } from '@angular/common/http'; // Thêm import
import { CategoryService } from '../../../services/category.service';
import { NotificationService } from '../../../services/notification.service';
import { CategoryDto } from '../../../models/category.model';
import { PaginationComponent } from '../../../components/pagination/pagination.component'; // Thêm import

@Component({
  selector: 'app-admin-category-list',
  standalone: true,
  imports: [CommonModule, RouterLink, PaginationComponent], // Thêm PaginationComponent
  templateUrl: './category-list.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminCategoryListComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private notifier = inject(NotificationService);
  
  categories = signal<CategoryDto[]>([]);
  isLoading = signal(true);
  
  // <<< THÊM CÁC SIGNAL PHÂN TRANG >>>
  currentPage = signal(1);
  pageSize = 6;
  totalItems = signal(0);

  ngOnInit(): void {
    this.loadCategories();
  }

 loadCategories(): void {
    this.isLoading.set(true);
    // <<< SỬA LẠI LỜI GỌI SERVICE Ở ĐÂY >>>
    this.categoryService.getPagedCategories(this.currentPage(), this.pageSize).subscribe({
      next: (response: HttpResponse<CategoryDto[]>) => {
        this.categories.set(response.body || []);
        const totalCount = response.headers.get('X-Total-Count');
        this.totalItems.set(totalCount ? +totalCount : 0);
        this.isLoading.set(false);
      },
      // ...
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadCategories();
  }

  onDelete(id: number): void {
    if (confirm('Bạn có chắc muốn xóa danh mục này?')) {
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