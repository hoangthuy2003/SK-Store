import { ChangeDetectionStrategy, Component, inject, OnInit, signal, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { ProductService } from '../../../services/product.service';
import { CategoryService } from '../../../services/category.service';
import { BrandService } from '../../../services/brand.service';
import { ImageService } from '../../../services/image.service';
import { NotificationService } from '../../../services/notification.service';
import { ProductDetailDto, CreateProductDto, UpdateProductDto, UpdateProductWithFilesDto, ProductImageDto } from '../../../models/product.model';
import { CategoryDto } from '../../../models/category.model';
import { BrandDto } from '../../../models/brand.model';

@Component({
  selector: 'app-product-edit',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.scss', '../admin-shared.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private location = inject(Location);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private brandService = inject(BrandService);
  private imageService = inject(ImageService);
  private notifier = inject(NotificationService);

  // Signals for reactive data
  productForm!: FormGroup;
  isEditMode = signal(false);
  currentProductId: number | null = null;
  categories = signal<CategoryDto[]>([]);
  brands = signal<BrandDto[]>([]);
  currentProduct = signal<ProductDetailDto | null>(null);
  isLoading = signal(false);

  // Image management signals
  currentPrimaryImage = signal<ProductImageDto | null>(null);
  secondaryImages = signal<(ProductImageDto | null)[]>([null, null, null, null]);
  
  // New image files to upload
  newPrimaryImageFile: File | null = null;
  newSecondaryImageFiles: (File | null)[] = [null, null, null, null];

  // Track deleted images and changes
  imagesToDelete: number[] = [];
  private _hasImageChanges = signal(false);
  
  // Public signal for template - workaround for VS Code issue
  public imageChangesSignal = signal(false);

  // Simple public properties for template
  get hasImageChanges(): boolean {
    return this._hasImageChanges();
  }

  hasImageChangesValue(): boolean {
    return this._hasImageChanges();
  }

  // Simple method for template
  checkImageChanges(): boolean {
    return this._hasImageChanges();
  }

  // ViewChildren for file inputs
  @ViewChildren('secondaryInput') secondaryImageInputs!: QueryList<ElementRef>;

  ngOnInit(): void {
    this.initializeForm();
    this.loadCategories();
    this.loadBrands();
    this.checkRouteParams();
  }

  initializeForm(): void {
    this.productForm = this.fb.group({
      productName: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: [null, Validators.required],
      brandId: [null, Validators.required],
      isActive: [true],
      attributes: this.fb.array([])
    });
  }

  get attributesArray(): FormArray {
    return this.productForm.get('attributes') as FormArray;
  }

  addAttribute(): void {
    const attributeGroup = this.fb.group({
      attributeName: ['', Validators.required],
      attributeValue: ['', Validators.required]
    });
    this.attributesArray.push(attributeGroup);
  }

  removeAttribute(index: number): void {
    this.attributesArray.removeAt(index);
  }

  private async loadCategories(): Promise<void> {
    try {
      const categories = await this.categoryService.getAllCategories().toPromise();
      if (categories) {
        this.categories.set(categories);
      }
    } catch (error) {
      console.error('Error loading categories:', error);
      this.notifier.showError('Không thể tải danh sách danh mục');
    }
  }

  private async loadBrands(): Promise<void> {
    try {
      const brands = await this.brandService.getAllBrands().toPromise();
      if (brands) {
        this.brands.set(brands);
      }
    } catch (error) {
      console.error('Error loading brands:', error);
      this.notifier.showError('Không thể tải danh sách thương hiệu');
    }
  }

  private checkRouteParams(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.currentProductId = parseInt(id, 10);
      this.isEditMode.set(true);
      this.loadProduct(this.currentProductId);
    }
  }

  private async loadProduct(id: number): Promise<void> {
    this.isLoading.set(true);
    try {
      const product = await this.productService.getProductById(id).toPromise();
      if (product) {
        this.currentProduct.set(product);
        this.populateForm(product);
        this.setupImageManagement(product);
      }
    } catch (error) {
      console.error('Error loading product:', error);
      this.notifier.showError('Không thể tải thông tin sản phẩm');
    } finally {
      this.isLoading.set(false);
    }
  }

  private populateForm(product: ProductDetailDto): void {
    this.productForm.patchValue({
      productName: product.productName,
      description: product.description,
      price: product.price,
      stockQuantity: product.stockQuantity,
      categoryId: product.categoryId,
      brandId: product.brandId,
      isActive: product.isActive
    });

    // Clear existing attributes
    while (this.attributesArray.length !== 0) {
      this.attributesArray.removeAt(0);
    }

    // Add product attributes
    if (product.productAttributes) {
      product.productAttributes.forEach(attr => {
        const attributeGroup = this.fb.group({
          attributeName: [attr.attributeName, Validators.required],
          attributeValue: [attr.attributeValue, Validators.required]
        });
        this.attributesArray.push(attributeGroup);
      });
    }
  }

  async onSubmit(): Promise<void> {
    if (this.productForm.invalid) {
      this.markFormGroupTouched(this.productForm);
      return;
    }

    this.isLoading.set(true);

    try {
      if (this.isEditMode()) {
        await this.updateProduct();
      } else {
        await this.createProduct();
      }
    } catch (error) {
      console.error('Error saving product:', error);
      this.notifier.showError('Có lỗi xảy ra khi lưu sản phẩm');
    } finally {
      this.isLoading.set(false);
    }
  }

  private async createProduct(): Promise<void> {
    const formData = this.productForm.value;
    const productData: CreateProductDto = {
      productName: formData.productName,
      description: formData.description,
      price: formData.price,
      stockQuantity: formData.stockQuantity,
      categoryId: formData.categoryId,
      brandId: formData.brandId,
      isActive: formData.isActive
    };

    // Collect all image files
    const imageFiles: File[] = [];
    if (this.newPrimaryImageFile) {
      imageFiles.push(this.newPrimaryImageFile);
    }
    this.newSecondaryImageFiles.forEach(file => {
      if (file) imageFiles.push(file);
    });

    try {
      const response = await this.productService.createProductWithFiles(productData, imageFiles).toPromise();
      
      if (response) {
        this.notifier.showSuccess('Tạo sản phẩm thành công');
        this.router.navigate(['/admin/products']);
      } else {
        throw new Error('Unknown error');
      }
    } catch (error) {
      throw error;
    }
  }

  private async updateProduct(): Promise<void> {
    if (!this.currentProductId) return;

    const formData = this.productForm.value;
    const productData: UpdateProductWithFilesDto = {
      productName: formData.productName,
      description: formData.description,
      price: formData.price,
      stockQuantity: formData.stockQuantity,
      categoryId: formData.categoryId,
      brandId: formData.brandId,
      isActive: formData.isActive,
      replaceAllImages: false, // Không thay thế tất cả, chỉ thêm ảnh mới
      imagesToDelete: [], // Ảnh đã được xóa realtime
      primaryImageIndex: 0 // Sẽ được xác định dựa trên ảnh nào được đặt primary
    };

    // Collect new image files
    const imageFiles: File[] = [];
    let primaryIndex = -1;

    // Thêm ảnh chính mới (nếu có)
    if (this.newPrimaryImageFile) {
      imageFiles.push(this.newPrimaryImageFile);
      primaryIndex = 0; // Ảnh chính luôn ở vị trí đầu
    }

    // Thêm ảnh phụ mới (nếu có)
    this.newSecondaryImageFiles.forEach(file => {
      if (file) {
        imageFiles.push(file);
      }
    });

    // Cập nhật primaryImageIndex
    if (primaryIndex >= 0) {
      productData.primaryImageIndex = primaryIndex;
    } else {
      // Không có ảnh chính mới, để backend xử lý
      productData.primaryImageIndex = -1;
    }

    try {
      const updatedProduct = await this.productService.updateProductWithFiles(
        this.currentProductId, 
        productData, 
        imageFiles.length > 0 ? imageFiles : undefined
      ).toPromise();

      if (updatedProduct) {
        // Cập nhật thông tin sản phẩm hiện tại
        this.currentProduct.set(updatedProduct);
        this.setupImageManagement(updatedProduct);
        
        // Reset trạng thái
        this.resetImageState();
        
        this.notifier.showSuccess('Cập nhật sản phẩm thành công');
        this.router.navigate(['/admin/products']);
      }
    } catch (error) {
      throw error;
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  goBack(): void {
    this.location.back();
  }

  // Helper method để get image URL
  getImageUrl(imageUrl: string): string {
    return this.imageService.getFullImageUrl(imageUrl);
  }

  // Setup image management for edit mode
  setupImageManagement(product: ProductDetailDto): void {
    if (product.productImages && product.productImages.length > 0) {
      // Find primary image
      const primaryImage = product.productImages.find(img => img.isPrimary);
      this.currentPrimaryImage.set(primaryImage || null);

      // Setup secondary images (max 4)
      const secondaryImages = product.productImages.filter(img => !img.isPrimary);
      const secondarySlots: (ProductImageDto | null)[] = [null, null, null, null];
      
      secondaryImages.slice(0, 4).forEach((img, index) => {
        secondarySlots[index] = img;
      });
      
      this.secondaryImages.set(secondarySlots);
    }
  }

  // Get secondary image slots for template
  secondaryImageSlots() {
    return this.secondaryImages().map((image, index) => ({ image, index }));
  }

  // Primary image handlers
  onPrimaryImageSelect(event: any): void {
    const file = event.target.files?.[0];
    if (file && this.validateImageFile(file)) {
      this.newPrimaryImageFile = file;
      this._hasImageChanges.set(true);
      this.imageChangesSignal.set(true);
      
      // Show preview by creating object URL
      const reader = new FileReader();
      reader.onload = (e) => {
        // Create a temporary image object for preview
        const tempImage: ProductImageDto = {
          imageId: 0, // 0 indicates new image
          imageUrl: e.target?.result as string,
          isPrimary: true
        };
        this.currentPrimaryImage.set(tempImage);
      };
      reader.readAsDataURL(file);
    }
    // Reset input
    event.target.value = '';
  }

  deletePrimaryImage(): void {
    if (confirm('Bạn có chắc chắn muốn xóa ảnh chính?')) {
      const currentImage = this.currentPrimaryImage();
      console.log('Deleting primary image:', currentImage);
      console.log('Current product ID:', this.currentProductId);
      
      if (currentImage && currentImage.imageId > 0) {
        console.log('Calling API to delete image ID:', currentImage.imageId);
        // Xóa ảnh ngay lập tức thay vì chờ đến khi save
        this.productService.deleteProductImage(this.currentProductId!, currentImage.imageId)
          .subscribe({
            next: () => {
              console.log('Successfully deleted primary image');
              this.currentPrimaryImage.set(null);
              this.newPrimaryImageFile = null;
              this._hasImageChanges.set(true);
              this.imageChangesSignal.set(true);
              this.notifier.showSuccess('Đã xóa ảnh chính');
            },
            error: (error) => {
              console.error('Error deleting primary image:', error);
              this.notifier.showError('Không thể xóa ảnh. Vui lòng thử lại.');
            }
          });
      } else {
        console.log('No valid image to delete, just clearing UI');
        // Nếu chỉ là ảnh tạm thời (chưa save)
        this.currentPrimaryImage.set(null);
        this.newPrimaryImageFile = null;
        this._hasImageChanges.set(true);
        this.imageChangesSignal.set(true);
      }
    }
  }

  // Secondary image handlers
  public triggerSecondaryInput(index: number): void {
    const input = document.getElementById(`secondaryInput${index}`) as HTMLInputElement;
    if (input) {
      input.click();
    }
  }

  onSecondaryImageButtonClick(index: number): void {
    const inputs = this.secondaryImageInputs.toArray();
    if (inputs[index]) {
      inputs[index].nativeElement.click();
    }
  }

  onSecondaryImageSelect(event: any, index: number): void {
    const file = event.target.files?.[0];
    if (file && this.validateImageFile(file)) {
      this.newSecondaryImageFiles[index] = file;
      this._hasImageChanges.set(true);
      this.imageChangesSignal.set(true);
      
      // Show preview
      const reader = new FileReader();
      reader.onload = (e) => {
        const currentSecondaryImages = this.secondaryImages();
        const tempImage: ProductImageDto = {
          imageId: 0, // 0 indicates new image
          imageUrl: e.target?.result as string,
          isPrimary: false
        };
        currentSecondaryImages[index] = tempImage;
        this.secondaryImages.set([...currentSecondaryImages]);
      };
      reader.readAsDataURL(file);
    }
    // Reset input
    event.target.value = '';
  }

  deleteSecondaryImage(index: number): void {
    if (confirm('Bạn có chắc chắn muốn xóa ảnh này?')) {
      const currentSecondaryImages = this.secondaryImages();
      const imageToDelete = currentSecondaryImages[index];
      
      if (imageToDelete && imageToDelete.imageId > 0) {
        // Xóa ảnh ngay lập tức
        this.productService.deleteProductImage(this.currentProductId!, imageToDelete.imageId)
          .subscribe({
            next: () => {
              currentSecondaryImages[index] = null;
              this.secondaryImages.set([...currentSecondaryImages]);
              this.newSecondaryImageFiles[index] = null;
              this._hasImageChanges.set(true);
              this.imageChangesSignal.set(true);
              this.notifier.showSuccess('Đã xóa ảnh');
            },
            error: (error) => {
              console.error('Error deleting image:', error);
              this.notifier.showError('Không thể xóa ảnh. Vui lòng thử lại.');
            }
          });
      } else {
        // Nếu chỉ là ảnh tạm thời (chưa save)
        currentSecondaryImages[index] = null;
        this.secondaryImages.set([...currentSecondaryImages]);
        this.newSecondaryImageFiles[index] = null;
        this._hasImageChanges.set(true);
        this.imageChangesSignal.set(true);
      }
    }
  }

  // File validation
  validateImageFile(file: File): boolean {
    const maxSize = 5 * 1024 * 1024; // 5MB
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];

    if (file.size > maxSize) {
      this.notifier.showError(`File ${file.name} vượt quá kích thước cho phép (5MB)`);
      return false;
    }

    if (!allowedTypes.includes(file.type.toLowerCase())) {
      this.notifier.showError(`File ${file.name} không đúng định dạng. Chỉ chấp nhận JPG, JPEG, PNG`);
      return false;
    }

    return true;
  }

  // Reset image state after successful update
  resetImageState(): void {
    this.newPrimaryImageFile = null;
    this.newSecondaryImageFiles = [null, null, null, null];
    this.imagesToDelete = [];
    this._hasImageChanges.set(false);
    this.imageChangesSignal.set(false);
  }

  // Check if there are any unsaved image changes
  hasUnsavedImageChanges(): boolean {
    return this._hasImageChanges() || 
           this.newPrimaryImageFile !== null ||
           this.newSecondaryImageFiles.some(file => file !== null) ||
           this.imagesToDelete.length > 0;
  }
}
