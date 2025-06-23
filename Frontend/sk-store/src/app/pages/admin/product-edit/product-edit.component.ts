import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { switchMap } from 'rxjs/operators';

// Models
import { CategoryDto } from '../../../models/category.model';
import { BrandDto } from '../../../models/brand.model';

// Services
import { ProductService } from '../../../services/product.service';
import { CategoryService } from '../../../services/category.service';
import { BrandService } from '../../../services/brand.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-product-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.css']
})
export class AdminProductEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private brandService = inject(BrandService);
  private notifier = inject(NotificationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private location = inject(Location);

  productForm: FormGroup;
  isEditMode = false;
  currentProductId: number | null = null;

  // Signals
  isLoading = signal(true);
  isSaving = signal(false);
  categories = signal<CategoryDto[]>([]);
  brands = signal<BrandDto[]>([]);

  constructor() {
    this.productForm = this.fb.group({
      productName: ['', Validators.required],
      description: [''],
      price: [0, [Validators.required, Validators.min(1)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: [null, Validators.required],
      brandId: [null, Validators.required],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!productId;
    this.currentProductId = productId ? +productId : null;

    // <<< SỬA LẠI CÁC LỜI GỌI SERVICE Ở ĐÂY >>>
    const categories$ = this.categoryService.getAllCategories();
    const brands$ = this.brandService.getAllBrands();

    forkJoin({ categories: categories$, brands: brands$ }).subscribe({
      next: (data) => {
        // Giờ đây data.categories và data.brands là mảng, không phải HttpResponse
        this.categories.set(data.categories);
        this.brands.set(data.brands);

        if (this.isEditMode && this.currentProductId) {
          this.productService.getProductById(this.currentProductId).subscribe(product => {
            this.productForm.patchValue(product);
            this.isLoading.set(false);
          });
        } else {
          this.isLoading.set(false);
        }
      },
      // ...
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.notifier.showError("Vui lòng điền đầy đủ các trường bắt buộc.");
      return;
    }
    this.isSaving.set(true);

    const formData = this.productForm.value;

    if (this.isEditMode && this.currentProductId) {
      // Update mode
      this.productService.updateProduct(this.currentProductId, formData).subscribe({
        next: () => {
          this.notifier.showSuccess("Cập nhật sản phẩm thành công!");
          this.router.navigate(['/admin/products']);
        },
        error: (err) => {
          this.notifier.showError(err.message);
          this.isSaving.set(false);
        }
      });
    } else {
      // Create mode
      this.productService.createProduct(formData).subscribe({
        next: () => {
          this.notifier.showSuccess("Thêm sản phẩm mới thành công!");
          this.router.navigate(['/admin/products']);
        },
        error: (err) => {
          this.notifier.showError(err.message);
          this.isSaving.set(false);
        }
      });
    }
  }

  goBack(): void {
    this.location.back();
  }
}