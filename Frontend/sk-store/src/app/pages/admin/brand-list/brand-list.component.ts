import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpResponse } from '@angular/common/http';
import { BrandService } from '../../../services/brand.service';
import { NotificationService } from '../../../services/notification.service';
import { BrandDto } from '../../../models/brand.model';
import { PaginationComponent } from '../../../components/pagination/pagination.component';

@Component({
  selector: 'app-admin-brand-list',
  standalone: true,
  imports: [CommonModule, RouterLink, PaginationComponent],
  templateUrl: './brand-list.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminBrandListComponent implements OnInit {
  private brandService = inject(BrandService);
  private notifier = inject(NotificationService);
  
  brands = signal<BrandDto[]>([]);
  isLoading = signal(true);
  
  // Pagination signals
  currentPage = signal(1);
  pageSize = 6;
  totalItems = signal(0);

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.isLoading.set(true);
    // Gọi đúng phương thức getPagedBrands với các tham số phân trang
    this.brandService.getPagedBrands(this.currentPage(), this.pageSize).subscribe({
      next: (response: HttpResponse<BrandDto[]>) => {
        this.brands.set(response.body || []);
        const totalCount = response.headers.get('X-Total-Count');
        this.totalItems.set(totalCount ? +totalCount : 0);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.notifier.showError("Không thể tải danh sách thương hiệu.");
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadBrands();
  }

  onDelete(id: number): void {
    if (confirm('Bạn có chắc muốn xóa thương hiệu này?')) {
      this.brandService.deleteBrand(id).subscribe({
        next: () => {
          this.notifier.showSuccess('Xóa thương hiệu thành công!');
          // Tải lại dữ liệu sau khi xóa
          this.loadBrands();
        },
        error: (err) => this.notifier.showError(err.message)
      });
    }
  }
}