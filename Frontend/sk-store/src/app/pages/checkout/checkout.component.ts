import { Component, OnInit, OnDestroy, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { ImageService } from '../../services/image.service';
import { OrderService } from '../../services/order.service';
import { UserAddressService, UserAddressDto } from '../../services/user-address.service';
import { CartDto } from '../../models/cart.model';
import { CreateOrderRequestDto } from '../../models/order.model';
import { Observable, Subject, takeUntil } from 'rxjs';
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
export class CheckoutComponent implements OnInit, OnDestroy {
  private paymentService = inject(PaymentService);
  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private imageService = inject(ImageService);
  private orderService = inject(OrderService);
  private userAddressService = inject(UserAddressService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  checkoutForm: FormGroup;
  cart$: Observable<CartDto | null> = this.cartService.cart$;
  isPayOsModalOpen = signal(false);
  payOsData = signal<PayOsPaymentResponse | null>(null);
  isProcessing = signal(false);
  errorMessage = signal<string | null>(null);
  
  // Address related signals
  addresses = signal<UserAddressDto[]>([]);
  selectedAddressId = signal<number | null>(null);
  showAddressForm = signal(false);
  loadingAddresses = signal(false);

  // Method to check if can submit
  canSubmitOrder(): boolean {
    if (this.isProcessing()) return false;
    
    // Always require payment method
    if (!this.checkoutForm.get('paymentMethod')?.value) return false;
    
    if (this.selectedAddressId()) {
      // Nếu đã chọn địa chỉ đã lưu, chỉ cần payment method
      return true;
    } else if (this.showAddressForm()) {
      // Nếu nhập thủ công, cần form valid
      return this.checkoutForm.valid;
    }
    
    return false;
  }

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
      }),
      takeUntil(this.destroy$)
    ).subscribe();

    // Load danh sách địa chỉ của user
    this.loadUserAddresses();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUserAddresses(): void {
    this.loadingAddresses.set(true);
    this.userAddressService.getUserAddresses()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (addresses) => {
          this.addresses.set(addresses);
          
          // Tự động chọn địa chỉ mặc định nếu có
          const defaultAddress = addresses.find(addr => addr.isDefault);
          if (defaultAddress) {
            this.selectAddress(defaultAddress.addressId);
          }
          
          this.loadingAddresses.set(false);
        },
        error: (error) => {
          console.error('Error loading addresses:', error);
          this.loadingAddresses.set(false);
        }
      });
  }

  selectAddress(addressId: number): void {
    const address = this.addresses().find(addr => addr.addressId === addressId);
    if (address) {
      this.selectedAddressId.set(addressId);
      this.showAddressForm.set(false);
      
      // Temporarily clear validators for saved address
      this.checkoutForm.get('recipientName')?.clearValidators();
      this.checkoutForm.get('recipientPhoneNumber')?.clearValidators();
      this.checkoutForm.get('shippingAddress')?.clearValidators();
      
      // Cập nhật form với thông tin địa chỉ đã chọn
      this.checkoutForm.patchValue({
        recipientName: address.recipientName || 'Người nhận',
        recipientPhoneNumber: address.recipientPhoneNumber || '0000000000',
        shippingAddress: address.addressLine
      });

      // Update validity
      this.checkoutForm.get('recipientName')?.updateValueAndValidity();
      this.checkoutForm.get('recipientPhoneNumber')?.updateValueAndValidity();
      this.checkoutForm.get('shippingAddress')?.updateValueAndValidity();
    }
  }

  useManualAddress(): void {
    this.selectedAddressId.set(null);
    this.showAddressForm.set(true);
    
    // Restore validators when using manual address
    this.checkoutForm.get('recipientName')?.setValidators([Validators.required, Validators.minLength(3)]);
    this.checkoutForm.get('recipientPhoneNumber')?.setValidators([Validators.required, Validators.pattern(/^0\d{9}$/)]);
    this.checkoutForm.get('shippingAddress')?.setValidators([Validators.required, Validators.minLength(10)]);
    
    // Clear form để người dùng nhập thủ công
    this.checkoutForm.patchValue({
      recipientName: '',
      recipientPhoneNumber: '',
      shippingAddress: ''
    });

    // Update validity
    this.checkoutForm.get('recipientName')?.updateValueAndValidity();
    this.checkoutForm.get('recipientPhoneNumber')?.updateValueAndValidity();
    this.checkoutForm.get('shippingAddress')?.updateValueAndValidity();
  }

  onSubmit(): void {
    // Validation tùy thuộc vào việc sử dụng địa chỉ đã lưu hay nhập thủ công
    if (this.selectedAddressId()) {
      // Sử dụng địa chỉ đã lưu, chỉ cần kiểm tra payment method
      if (!this.checkoutForm.get('paymentMethod')?.value) {
        this.errorMessage.set('Vui lòng chọn phương thức thanh toán.');
        return;
      }
    } else {
      // Nhập thủ công, cần validation đầy đủ
      if (this.checkoutForm.invalid) {
        this.checkoutForm.markAllAsTouched();
        return;
      }
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

  // Helper method để get image URL
  getImageUrl(imageUrl: string | undefined): string {
    if (!imageUrl) {
      return this.imageService.getPlaceholderUrl();
    }
    return this.imageService.getFullImageUrl(imageUrl);
  }

  
}