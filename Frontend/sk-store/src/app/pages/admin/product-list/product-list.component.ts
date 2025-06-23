import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';

// Models
import { ProductDto, ProductFilterParameters } from '../../../models/product.model';

// Services
import { ProductService } from '../../../services/product.service';

// Pipes & Components
import { VndCurrencyPipe } from '../../../pipes/vnd-currency.pipe';
import { PaginationComponent } from '../../../components/pagination/pagination.component';

@Component({
  selector: 'app-admin-product-list',
  standalone: true,
  imports: [CommonModule, VndCurrencyPipe, PaginationComponent],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class AdminProductListComponent implements OnInit {
  private productService = inject(ProductService);

  // State signals
  isLoading = signal(true);
  products = signal<ProductDto[]>([]);
  
  // Pagination signals
  currentPage = signal(1);
  pageSize = 10; // Hiển thị 10 sản phẩm mỗi trang cho admin
  totalProducts = signal(0);

  ngOnInit(): void {
    this.fetchProducts();
  }

  fetchProducts(): void {
    this.isLoading.set(true);
    const filters: ProductFilterParameters = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize
    };

    this.productService.getProductsWithCount(filters).subscribe({
      next: (response: HttpResponse<any>) => {
        // Backend trả về ProductDto, nhưng chúng ta cần thêm stockQuantity và isActive
        // Giả sử API getProducts trả về đủ thông tin này
        this.products.set(response.body || []);
        const totalCount = response.headers.get('X-Total-Count');
        this.totalProducts.set(totalCount ? +totalCount : 0);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error("Failed to fetch products for admin", err);
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.fetchProducts();
  }
}