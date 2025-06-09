import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { CartDto, CartItemDto } from '../../models/cart.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  private cartService = inject(CartService);

  // Sử dụng observable trực tiếp từ service để luôn có dữ liệu mới nhất
  cart$: Observable<CartDto | null> = this.cartService.cart$;
  
  // Signal để theo dõi item nào đang được cập nhật
  updatingItemId = signal<number | null>(null);

  ngOnInit(): void {
    // Không cần làm gì ở đây vì cart$ đã được kết nối
  }

  updateQuantity(item: CartItemDto, newQuantity: number): void {
    if (newQuantity < 1) {
      this.removeItem(item.productId);
      return;
    }
    
    // Vượt quá tồn kho
    if (newQuantity > item.stockQuantity) {
      alert(`Số lượng tồn kho của sản phẩm "${item.productName}" không đủ. Chỉ còn ${item.stockQuantity}.`);
      return;
    }

    this.updatingItemId.set(item.productId);
    this.cartService.updateItemQuantity(item.productId, newQuantity).subscribe({
      next: () => this.updatingItemId.set(null),
      error: (err) => {
        alert('Lỗi khi cập nhật giỏ hàng. Vui lòng thử lại.');
        console.error(err);
        this.updatingItemId.set(null);
      }
    });
  }

  removeItem(productId: number): void {
    if (!confirm('Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng?')) {
      return;
    }
    this.updatingItemId.set(productId);
    this.cartService.removeItem(productId).subscribe({
      next: () => this.updatingItemId.set(null),
      error: (err) => {
        alert('Lỗi khi xóa sản phẩm. Vui lòng thử lại.');
        console.error(err);
        this.updatingItemId.set(null);
      }
    });
  }

  // Hàm tiện ích
  formatPrice(price: number): string {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);
  }
}