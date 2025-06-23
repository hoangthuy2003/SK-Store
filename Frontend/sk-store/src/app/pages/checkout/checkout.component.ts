import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { CartDto } from '../../models/cart.model';
import { CreateOrderRequestDto } from '../../models/order.model';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';
@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule, VndCurrencyPipe],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private router = inject(Router);

  checkoutForm: FormGroup;
  cart$: Observable<CartDto | null> = this.cartService.cart$;
  
  isProcessing = signal(false);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.checkoutForm = this.fb.group({
      recipientName: ['', [Validators.required, Validators.minLength(3)]],
      recipientPhoneNumber: ['', [Validators.required, Validators.pattern(/^0\d{9}$/)]],
      shippingAddress: ['', [Validators.required, Validators.minLength(10)]],
      notes: ['']
    });
  }

  ngOnInit(): void {
    // Nếu giỏ hàng trống, điều hướng về trang giỏ hàng
    this.cart$.pipe(
      tap(cart => {
        if (!cart || cart.items.length === 0) {
          this.router.navigate(['/cart']);
        }
      })
    ).subscribe();
  }

  onSubmit(): void {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    this.isProcessing.set(true);
    this.errorMessage.set(null);

    const orderData: CreateOrderRequestDto = {
      ...this.checkoutForm.value,
      paymentMethod: 'COD' // Hiện tại chỉ hỗ trợ COD
    };

    this.orderService.createOrder(orderData).subscribe({
      next: (createdOrder) => {
        this.isProcessing.set(false);
        // Xóa giỏ hàng sau khi đặt hàng thành công
        this.cartService.clearCart().subscribe(); // Tải lại để cập nhật giỏ hàng trống
        // Điều hướng đến trang cảm ơn hoặc trang chi tiết đơn hàng
        this.router.navigate(['/orders', createdOrder.orderId], { state: { success: true } });
      },
      error: (err) => {
        this.isProcessing.set(false);
        this.errorMessage.set(err.error?.message || 'Đã xảy ra lỗi khi đặt hàng. Vui lòng thử lại.');
        console.error(err);
      }
    });
  }

  
}