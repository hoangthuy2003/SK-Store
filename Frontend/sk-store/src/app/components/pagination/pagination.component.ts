import { Component, Input, Output, EventEmitter, computed, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
})
export class PaginationComponent {
  @Input({ required: true }) currentPage!: number;
  @Input({ required: true }) totalItems!: number;
  @Input({ required: true }) pageSize!: number;
  @Output() pageChange = new EventEmitter<number>();

  // Thêm dòng này để "giới thiệu" đối tượng Math cho template
  protected readonly Math = Math;

  // Sử dụng computed signal để tính toán các trang cần hiển thị
  paginationData: Signal<{ totalPages: number, pages: (number | '...')[] }> = computed(() => {
    const totalPages = Math.ceil(this.totalItems / this.pageSize);
    const pages: (number | '...')[] = [];
    
    if (totalPages <= 7) {
      // Hiển thị tất cả các trang nếu ít hơn hoặc bằng 7
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Logic hiển thị phức tạp hơn với dấu "..."
      pages.push(1);
      if (this.currentPage > 3) {
        pages.push('...');
      }
      
      const startPage = Math.max(2, this.currentPage - 1);
      const endPage = Math.min(totalPages - 1, this.currentPage + 1);

      for (let i = startPage; i <= endPage; i++) {
        pages.push(i);
      }

      if (this.currentPage < totalPages - 2) {
        pages.push('...');
      }
      pages.push(totalPages);
    }

    return { totalPages, pages };
  });

  changePage(page: number | '...'): void {
    if (typeof page === 'number' && page !== this.currentPage) {
      this.pageChange.emit(page);
    }
  }

  goToPrevious(): void {
    if (this.currentPage > 1) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }

  goToNext(): void {
    const totalPages = this.paginationData().totalPages;
    if (this.currentPage < totalPages) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }
}