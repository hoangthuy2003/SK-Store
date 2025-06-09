import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { CartService } from '../../services/cart.service'; // <<< THÊM IMPORT
import { AddItemToCartDto } from '../../models/cart.model'; // <<< THÊM IMPORT
import { ProductService } from '../../services/product.service';
import { ProductDetailDto } from '../../models/product.model';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
private cartService = inject(CartService); 
  // State signals
  product = signal<ProductDetailDto | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);
  isAddingToCart = signal(false);
  // Signal cho hình ảnh đang được chọn
  selectedImage = signal<string | undefined>(undefined);
  quantity = signal(1);

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
        this.selectedImage.set(data.productImages.find(img => img.isPrimary)?.imageUrl || data.productImages[0]?.imageUrl);
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

  // Hàm tiện ích
  formatPrice(price: number): string {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);
  }

  getStarRating(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i < Math.round(rating) ? 1 : 0);
  }
}