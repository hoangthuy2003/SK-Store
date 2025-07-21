import { Component, OnInit, signal, inject, effect, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { forkJoin, Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { HttpResponse } from '@angular/common/http';

// Pipes
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe'; 

// Services
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { BrandService } from '../../services/brand.service';
import { ImageService } from '../../services/image.service';

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
export class ShopComponent implements OnInit, OnDestroy {
  // Inject services
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private brandService = inject(BrandService);
  private imageService = inject(ImageService);

  // State signals
  isLoading = signal(true);
  isFetchingProducts = signal(false);
  error = signal<string | null>(null);
  
  products = signal<ProductDto[]>([]);
  categories = signal<CategoryDto[]>([]);
  brands = signal<BrandDto[]>([]);

  // State cho phân trang
  currentPage = signal(1);
  pageSize = 9;
  totalProducts = signal(0);

  // --- CẬP NHẬT CHO TÌM KIẾM ---
  // Filter signals
  activeCategoryId = signal<number | null>(null);
  activeBrandId = signal<number | null>(null);
  searchTerm = signal<string>(''); // Signal để lưu từ khóa tìm kiếm
  
  // Sorting signals
  sortBy = signal<string>(''); // '', 'price', 'name', 'created', 'popular'
  sortOrder = signal<string>('asc'); // 'asc' hoặc 'desc'

  // Subject để quản lý input tìm kiếm với debounce
  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;
  // --- KẾT THÚC CẬP NHẬT ---
  
  constructor() {
    // Sử dụng effect để tự động gọi lại API khi filter hoặc trang thay đổi
    effect(() => {
      // Lấy giá trị của các signal
      const categoryId = this.activeCategoryId();
      const brandId = this.activeBrandId();
      const page = this.currentPage();
      const search = this.searchTerm(); // Thêm searchTerm vào danh sách phụ thuộc
      const sortBy = this.sortBy(); // Thêm sorting vào danh sách phụ thuộc
      const sortOrder = this.sortOrder();
      
      // Bỏ qua lần chạy đầu tiên khi component chưa load xong dữ liệu ban đầu
      if (!this.isLoading()) {
         this.fetchProducts();
      }
    });
  }

  ngOnInit(): void {
    this.loadInitialData();

    // --- CẬP NHẬT CHO TÌM KIẾM ---
    // Thiết lập subscription cho debounced search
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(400), // Chờ 400ms sau khi người dùng ngừng gõ
      distinctUntilChanged() // Chỉ gửi request nếu giá trị thay đổi
    ).subscribe(searchValue => {
      console.log('🔍 Search term updated via debounce:', searchValue);
      this.searchTerm.set(searchValue);
      this.currentPage.set(1); // Reset về trang 1 khi tìm kiếm
    });
    // --- KẾT THÚC CẬP NHẬT ---
  }

  ngOnDestroy(): void {
    // Hủy subscription để tránh memory leak
    this.searchSubscription?.unsubscribe();
  }

  /**
   * Tải dữ liệu ban đầu cho các bộ lọc (categories, brands) và sau đó tải sản phẩm.
   */
  loadInitialData(): void {
    this.isLoading.set(true);
    this.error.set(null);

    // <<< SỬA LẠI CÁC LỜI GỌI SERVICE Ở ĐÂY >>>
    forkJoin({
      categories: this.categoryService.getAllCategories(),
      brands: this.brandService.getAllBrands()
    }).subscribe({
      next: (data) => {
        // Giờ đây data.categories và data.brands là mảng
        this.categories.set(data.categories);
        this.brands.set(data.brands);
        this.fetchProducts();
      },
      // ...
    });
  }

  /**
   * Lấy danh sách sản phẩm dựa trên các bộ lọc và trang hiện tại.
   */
 fetchProducts(): void {
  this.isFetchingProducts.set(true);

  const filters: ProductFilterParameters = {
    categoryId: this.activeCategoryId(),
    brandId: this.activeBrandId(),
    pageNumber: this.currentPage(),
    pageSize: this.pageSize,
    searchTerm: this.searchTerm(),
    // Chỉ gửi sortBy nếu nó có giá trị
    ...(this.sortBy() && { sortBy: this.sortBy() }),
    sortOrder: this.sortOrder(),
    // <<< THÊM DÒNG NÀY ĐỂ TRANG SHOP CHỈ LẤY SẢN PHẨM HOẠT ĐỘNG >>>
    isActive: true 
  };

  // Debug logging
  console.log('🔍 Shop fetchProducts called with filters:', filters);

  this.productService.getProductsWithCount(filters).subscribe({
    next: (response: HttpResponse<ProductDto[]>) => {
      this.products.set(response.body || []);
      
      const totalCount = response.headers.get('X-Total-Count');
      this.totalProducts.set(totalCount ? +totalCount : 0);
      
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

  // --- CẬP NHẬT CHO TÌM KIẾM ---
  /**
   * Xử lý sự kiện input từ ô tìm kiếm.
   * @param event Event từ thẻ input.
   */
  onSearch(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    console.log('🔍 Search input changed to:', inputElement.value);
    this.searchSubject.next(inputElement.value);
  }
  // --- KẾT THÚC CẬP NHẬT ---

  /**
   * Xử lý sự kiện thay đổi trang từ component phân trang.
   * @param page Số trang mới.
   */
  onPageChange(page: number): void {
    this.currentPage.set(page);
  }

  /**
   * Đặt bộ lọc theo danh mục.
   * @param categoryId ID của danh mục hoặc null để xóa bộ lọc.
   */
  setCategoryFilter(categoryId: number | null): void {
    console.log('📂 Category filter changed to:', categoryId);
    if (this.activeCategoryId() !== categoryId) {
      this.activeCategoryId.set(categoryId);
      this.currentPage.set(1);
    }
  }

  /**
   * Đặt bộ lọc theo thương hiệu.
   * @param brandId ID của thương hiệu hoặc null để xóa bộ lọc.
   */
  setBrandFilter(brandId: number | null): void {
    console.log('🏷️ Brand filter changed to:', brandId);
    if (this.activeBrandId() !== brandId) {
      this.activeBrandId.set(brandId);
      this.currentPage.set(1);
    }
  }

  /**
   * Xử lý thay đổi sắp xếp.
   * @param sortValue Giá trị sắp xếp từ dropdown
   */
  onSortChange(sortValue: string): void {
    console.log('🔄 Sort changed to:', sortValue);
    
    switch(sortValue) {
      case 'popular':
        this.sortBy.set('popular');
        this.sortOrder.set('desc');
        break;
      case 'price-asc':
        this.sortBy.set('price');
        this.sortOrder.set('asc');
        break;
      case 'price-desc':
        this.sortBy.set('price');
        this.sortOrder.set('desc');
        break;
      case 'newest':
        this.sortBy.set('created');
        this.sortOrder.set('desc');
        break;
      default:
        this.sortBy.set('');
        this.sortOrder.set('asc');
        break;
    }
    
    console.log('🔄 Sort signals updated:', { 
      sortBy: this.sortBy(), 
      sortOrder: this.sortOrder() 
    });
    
    this.currentPage.set(1); // Reset về trang 1 khi thay đổi sắp xếp
  }

  // Helper method để get image URL
  getImageUrl(imageUrl: string | undefined): string {
    if (!imageUrl) {
      return this.imageService.getPlaceholderUrl();
    }
    return this.imageService.getFullImageUrl(imageUrl);
  }
}