import { Component, OnInit, signal, inject, effect } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';

// Services
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { BrandService } from '../../services/brand.service';

// Models
import { ProductDto, ProductFilterParameters } from '../../models/product.model';
import { CategoryDto } from '../../models/category.model';
import { BrandDto } from '../../models/brand.model';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.css'],
  standalone: true,
  imports: [RouterLink, CommonModule]
})
export class ShopComponent implements OnInit {
  // Inject services
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private brandService = inject(BrandService);

  // State signals
  isLoading = signal(true);
  error = signal<string | null>(null);
  
  products = signal<ProductDto[]>([]);
  categories = signal<CategoryDto[]>([]);
  brands = signal<BrandDto[]>([]);

  // Filter signals
  activeCategoryId = signal<number | null>(null);
  activeBrandId = signal<number | null>(null);
  currentPage = signal(1);
  pageSize = 12; // Số sản phẩm mỗi trang

  constructor() {
    // Sử dụng effect để tự động gọi lại API khi filter thay đổi
    effect(() => {
      // Lấy giá trị của các signal filter
      const categoryId = this.activeCategoryId();
      const brandId = this.activeBrandId();
      const page = this.currentPage();
      
      // Gọi hàm fetch sản phẩm
      // Bỏ qua lần chạy đầu tiên khi component chưa init xong
      if (!this.isLoading()) {
         this.fetchProducts();
      }
    });
  }

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.isLoading.set(true);
    this.error.set(null);

    // Sử dụng forkJoin để gọi API lấy category và brand song song
    forkJoin({
      categories: this.categoryService.getCategories(),
      brands: this.brandService.getBrands()
    }).subscribe({
      next: (data) => {
        this.categories.set(data.categories);
        this.brands.set(data.brands);
        // Sau khi có category và brand, mới fetch sản phẩm
        this.fetchProducts();
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to load filter data.');
        this.isLoading.set(false);
      }
    });
  }

  fetchProducts(): void {
    const filters: ProductFilterParameters = {
      categoryId: this.activeCategoryId(),
      brandId: this.activeBrandId(),
      pageNumber: this.currentPage(),
      pageSize: this.pageSize,
      searchTerm: null // Sẽ thêm sau nếu có ô tìm kiếm
    };

    this.productService.getProducts(filters).subscribe({
      next: (data) => {
        this.products.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Failed to load products.');
        this.isLoading.set(false);
      }
    });
  }

  // Hàm xử lý khi người dùng click vào filter
  setCategoryFilter(categoryId: number | null): void {
    this.activeCategoryId.set(categoryId);
    this.currentPage.set(1); // Reset về trang 1 khi đổi filter
  }

  setBrandFilter(brandId: number | null): void {
    this.activeBrandId.set(brandId);
    this.currentPage.set(1); // Reset về trang 1 khi đổi filter
  }

  // Hàm tiện ích để format giá tiền
  formatPrice(price: number): string {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(price);
  }
}