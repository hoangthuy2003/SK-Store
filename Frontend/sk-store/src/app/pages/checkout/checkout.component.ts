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
import { PaymentService, PayOsPaymentResponse } from '../../services/payment.service';
import { PayosPaymentModalComponent } from '../payos-payment-modal/payos-payment-modal.component';
@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, VndCurrencyPipe, PayosPaymentModalComponent ],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private router = inject(Router);

  checkoutForm: FormGroup;
  cart$: Observable<CartDto | null> = this.cartService.cart$;
   isPayOsModalOpen = signal(false);
  payOsData = signal<PayOsPaymentResponse | null>(null);
  isProcessing = signal(false);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.checkoutForm = this.fb.group({
      recipientName: ['', [Validators.required, Validators.minLength(3)]],
      recipientPhoneNumber: ['', [Validators.required, Validators.pattern(/^0\d{9}$/)]],
      shippingAddress: ['', [Validators.required, Validators.minLength(10)]],
      notes: [''],
      paymentMethod: ['COD', Validators.required]
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

    const orderData: CreateOrderRequestDto = this.checkoutForm.value;

    this.orderService.createOrder(orderData).subscribe({
      next: (createdOrder) => {
        // Nếu là COD, xử lý như cũ
        if (orderData.paymentMethod === 'COD') {
          this.isProcessing.set(false);
          this.cartService.clearCart().subscribe();
          this.router.navigate(['/orders', createdOrder.orderId], { state: { success: true } });
        } 
        // Nếu là VNPay, gọi để lấy URL
        else if (orderData.paymentMethod === 'PayOS') {
         this.paymentService.createPayOsLink(createdOrder.orderId).subscribe({
            next: (res) => {
              this.isProcessing.set(false);
              this.payOsData.set(res);
              this.isPayOsModalOpen.set(true); // Mở modal
            },
            error: (err) => {
              this.isProcessing.set(false);
              this.errorMessage.set('Không thể tạo link thanh toán. Vui lòng thử lại.');
            }
          });
        }
      },
      error: (err) => {
        this.isProcessing.set(false);
        this.errorMessage.set(err.error?.message || 'Đã xảy ra lỗi khi đặt hàng.');
      }
    });
  }

  
}