import { Component, OnInit, signal, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { switchMap, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { CartService } from '../../services/cart.service'; // <<< THÊM IMPORT
import { AddItemToCartDto } from '../../models/cart.model'; // <<< THÊM IMPORT
import { ProductService } from '../../services/product.service';
import { ImageService } from '../../services/image.service';
import { ReviewService, CreateReviewDto } from '../../services/review.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { ProductDetailDto } from '../../models/product.model';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';
@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, VndCurrencyPipe, ReactiveFormsModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private imageService = inject(ImageService);
  private cartService = inject(CartService);
  private reviewService = inject(ReviewService);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private fb = inject(FormBuilder);
  
  // Subject for managing subscriptions
  private destroy$ = new Subject<void>();
  
  // State signals
  product = signal<ProductDetailDto | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);
  isAddingToCart = signal(false);
  // Signal cho hình ảnh đang được chọn
  selectedImage = signal<string | undefined>(undefined);
  quantity = signal(1);
  
  // Review form signals
  reviewForm: FormGroup;
  isSubmittingReview = signal(false);
  showReviewForm = signal(false);
  
  constructor() {
    this.reviewForm = this.fb.group({
      rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: ['', [Validators.maxLength(1000)]]
    });
  }

  ngOnInit(): void {
    this.route.paramMap.pipe(
      switchMap(params => {
        const id = params.get('id');
        if (id) {
          this.isLoading.set(true);
          this.error.set(null);
          return this.productService.getProductById(+id);
        }
        // Nếu không có ID, trả về lỗi
        throw new Error('Product ID not found in URL.');
      })
    ).subscribe({
      next: (data) => {
        this.product.set(data);
        // Set ảnh chính làm ảnh được chọn ban đầu
        const primaryImage = data.productImages.find(img => img.isPrimary)?.imageUrl || data.productImages[0]?.imageUrl;
        this.selectedImage.set(primaryImage);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Không thể tải thông tin sản phẩm. Sản phẩm có thể không tồn tại.');
        console.error(err);
        this.isLoading.set(false);
      }
    });
  }
 addToCart(): void {
    const currentProduct = this.product();
    if (!currentProduct) return;

    this.isAddingToCart.set(true);

    const itemToAdd: AddItemToCartDto = {
      productId: currentProduct.productId,
      quantity: this.quantity()
    };

    this.cartService.addItem(itemToAdd).subscribe({
      next: (updatedCart) => {
        console.log('Added to cart:', updatedCart);
        // Có thể hiển thị thông báo thành công ở đây (ví dụ: dùng Toast)
        this.isAddingToCart.set(false);
      },
      error: (err) => {
        console.error('Failed to add to cart', err);
        // Hiển thị thông báo lỗi
        this.isAddingToCart.set(false);
      }
    });
  }
  // Thay đổi ảnh chính khi click vào thumbnail
  selectImage(imageUrl: string): void {
    this.selectedImage.set(imageUrl);
  }

  // Tăng/giảm số lượng
  increaseQuantity(): void {
    this.quantity.update(q => q + 1);
  }

  decreaseQuantity(): void {
    this.quantity.update(q => (q > 1 ? q - 1 : 1));
  }

  // Helper method để get image URL
  getImageUrl(imageUrl: string | undefined): string {
    if (!imageUrl) {
      return this.imageService.getPlaceholderUrl();
    }
    return this.imageService.getFullImageUrl(imageUrl);
  }

  getStarRating(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i < Math.round(rating) ? 1 : 0);
  }
  
  // Review related methods
  toggleReviewForm(): void {
    if (!this.authService.isAuthenticated()) {
      this.notificationService.showError('Bạn cần đăng nhập để đánh giá sản phẩm');
      return;
    }
    this.showReviewForm.update(show => !show);
  }
  
  submitReview(): void {
    if (this.reviewForm.invalid) {
      this.reviewForm.markAllAsTouched();
      return;
    }
    
    // Prevent multiple submissions
    if (this.isSubmittingReview()) {
      return;
    }
    
    const product = this.product();
    if (!product) return;
    
    this.isSubmittingReview.set(true);
    
    const reviewData: CreateReviewDto = {
      rating: this.reviewForm.value.rating,
      comment: this.reviewForm.value.comment?.trim() || undefined
    };
    
    this.reviewService.addReview(product.productId, reviewData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (newReview) => {
          this.notificationService.showSuccess('Đánh giá của bạn đã được thêm thành công!');
          
          // Reset form and hide it first
          this.reviewForm.reset({ rating: 5, comment: '' });
          this.showReviewForm.set(false);
          this.isSubmittingReview.set(false);
          
          // Then refresh product data to show new review
          this.productService.getProductById(product.productId)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: (updatedProduct) => {
                this.product.set(updatedProduct);
              },
              error: (refreshError) => {
                // Không hiển thị error cho việc refresh vì review đã thành công
                console.warn('Could not refresh product data:', refreshError);
              }
            });
        },
        error: (error) => {
          console.error('Error submitting review:', error);
          
          // Extract error message more carefully
          let errorMessage = 'Có lỗi xảy ra khi thêm đánh giá';
          
          if (error?.error?.message) {
            errorMessage = error.error.message;
          } else if (error?.message) {
            errorMessage = error.message;
          } else if (typeof error?.error === 'string') {
            errorMessage = error.error;
          }
          
          this.notificationService.showError(errorMessage);
          this.isSubmittingReview.set(false);
        }
      });
  }
  
  isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}