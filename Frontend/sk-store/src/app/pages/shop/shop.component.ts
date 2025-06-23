import { Component, OnInit, signal, inject, effect } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { HttpResponse } from '@angular/common/http';

// Pipes
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe'; 

// Services
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { BrandService } from '../../services/brand.service';

// Models
import { ProductDto, ProductFilterParameters } from '../../models/product.model';
import { CategoryDto } from '../../models/category.model';
import { BrandDto } from '../../models/brand.model';

// Components
import { PaginationComponent } from '../../components/pagination/pagination.component';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.css'],
  standalone: true,
  imports: [RouterLink, CommonModule, VndCurrencyPipe, PaginationComponent]
})
export class ShopComponent implements OnInit {
  // Inject services
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private brandService = inject(BrandService);

  // State signals
  isLoading = signal(true);
  isFetchingProducts = signal(false); // Signal riêng cho việc tải sản phẩm
  error = signal<string | null>(null);
  
  products = signal<ProductDto[]>([]);
  categories = signal<CategoryDto[]>([]);
  brands = signal<BrandDto[]>([]);

  // State cho phân trang
  currentPage = signal(1);
  pageSize = 12;
  totalProducts = signal(0);

  // Filter signals
  activeCategoryId = signal<number | null>(null);
  activeBrandId = signal<number | null>(null);
  
  constructor() {
    // Sử dụng effect để tự động gọi lại API khi filter hoặc trang thay đổi
    effect(() => {
      // Lấy giá trị của các signal
      const categoryId = this.activeCategoryId();
      const brandId = this.activeBrandId();
      const page = this.currentPage();
      
      // Bỏ qua lần chạy đầu tiên khi component chưa load xong dữ liệu ban đầu
      if (!this.isLoading()) {
         this.fetchProducts();
      }
    }, { allowSignalWrites: true }); // Cho phép effect thay đổi signal khác
  }

  ngOnInit(): void {
    this.loadInitialData();
  }

  /**
   * Tải dữ liệu ban đầu cho các bộ lọc (categories, brands) và sau đó tải sản phẩm.
   */
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
        // Sau khi có category và brand, mới fetch sản phẩm lần đầu
        this.fetchProducts();
      },
      error: (err) => {
        this.error.set(err.message || 'Không thể tải dữ liệu cho bộ lọc.');
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Lấy danh sách sản phẩm dựa trên các bộ lọc và trang hiện tại.
   */
  fetchProducts(): void {
    this.isFetchingProducts.set(true); // Bắt đầu tải sản phẩm

    const filters: ProductFilterParameters = {
      categoryId: this.activeCategoryId(),
      brandId: this.activeBrandId(),
      pageNumber: this.currentPage(),
      pageSize: this.pageSize,
      searchTerm: null // Sẽ thêm sau nếu có ô tìm kiếm
    };

    // Chỉ gọi một lần duy nhất đến getProductsWithCount
    this.productService.getProductsWithCount(filters).subscribe({
      next: (response: HttpResponse<ProductDto[]>) => {
        this.products.set(response.body || []);
        
        const totalCount = response.headers.get('X-Total-Count');
        this.totalProducts.set(totalCount ? +totalCount : 0);
        
        // Đánh dấu đã tải xong
        this.isLoading.set(false); 
        this.isFetchingProducts.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Không thể tải danh sách sản phẩm.');
        this.isLoading.set(false);
        this.isFetchingProducts.set(false);
      }
    });
  }

  /**
   * Xử lý sự kiện thay đổi trang từ component phân trang.
   * @param page Số trang mới.
   */
  onPageChange(page: number): void {
    this.currentPage.set(page);
    // effect sẽ tự động gọi lại fetchProducts
  }

  /**
   * Đặt bộ lọc theo danh mục.
   * @param categoryId ID của danh mục hoặc null để xóa bộ lọc.
   */
  setCategoryFilter(categoryId: number | null): void {
    // Chỉ thực hiện nếu giá trị thay đổi để tránh gọi API không cần thiết
    if (this.activeCategoryId() !== categoryId) {
      this.activeCategoryId.set(categoryId);
      this.currentPage.set(1); // Reset về trang 1 khi đổi filter
    }
  }

  /**
   * Đặt bộ lọc theo thương hiệu.
   * @param brandId ID của thương hiệu hoặc null để xóa bộ lọc.
   */
  setBrandFilter(brandId: number | null): void {
    // Chỉ thực hiện nếu giá trị thay đổi
    if (this.activeBrandId() !== brandId) {
      this.activeBrandId.set(brandId);
      this.currentPage.set(1); // Reset về trang 1 khi đổi filter
    }
  }
}