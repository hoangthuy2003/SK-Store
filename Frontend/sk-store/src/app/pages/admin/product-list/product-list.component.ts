import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router'; // <<< THÊM IMPORT

// Models
import { ProductDto, ProductFilterParameters } from '../../../models/product.model';

// Services
import { ProductService } from '../../../services/product.service';
import { NotificationService } from '../../../services/notification.service'; // <<< THÊM IMPORT
import { ImageService } from '../../../services/image.service';

// Pipes & Components
import { VndCurrencyPipe } from '../../../pipes/vnd-currency.pipe';
import { PaginationComponent } from '../../../components/pagination/pagination.component';

@Component({
  selector: 'app-admin-product-list',
  standalone: true,
  imports: [CommonModule, VndCurrencyPipe, PaginationComponent, RouterLink], // <<< THÊM RouterLink
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class AdminProductListComponent implements OnInit {
  private productService = inject(ProductService);
  private notifier = inject(NotificationService); // <<< THÊM INJECT
  private imageService = inject(ImageService);

  // State signals
  isLoading = signal(true);
  products = signal<ProductDto[]>([]);
  
  // Pagination signals
  currentPage = signal(1);
  pageSize = 5;
  totalProducts = signal(0);

  ngOnInit(): void {
    this.fetchProducts();
  }

  fetchProducts(): void {
    this.isLoading.set(true);
    const filters: ProductFilterParameters = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize,
      isActive: null // Chỉ lấy sản phẩm đang hoạt động
    };

    this.productService.getProductsWithCount(filters).subscribe({
      next: (response: HttpResponse<any>) => {
        this.products.set(response.body || []);
        const totalCount = response.headers.get('X-Total-Count');
        this.totalProducts.set(totalCount ? +totalCount : 0);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.notifier.showError("Không thể tải danh sách sản phẩm.");
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.fetchProducts();
  }

  onDeleteProduct(productId: number): void {
    if (confirm('Bạn có chắc chắn muốn ẩn sản phẩm này không?')) {
      this.productService.deleteProduct(productId).subscribe({
        next: () => {
          this.notifier.showSuccess('Ẩn sản phẩm thành công!'); // Sửa thông báo thành công
          // Tải lại danh sách sản phẩm ở trang hiện tại
          this.fetchProducts(); 
        },
        error: (err) => {
          this.notifier.showError(err.message);
        }
      });
    }
  }

  // Helper method để get image URL
  getImageUrl(imageUrl: string | undefined): string {
    if (!imageUrl) {
      return this.imageService.getPlaceholderUrl();
    }
    return this.imageService.getFullImageUrl(imageUrl);
  }
}