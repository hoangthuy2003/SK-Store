import { Component, Input, Output, EventEmitter, inject, OnInit, signal } from '@angular/core'; // Thêm OnInit, signal
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
// <<< THÊM 2 IMPORT QUAN TRỌNG NÀY >>>
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { PayOsPaymentResponse } from '../../services/payment.service';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';
import { CartService } from '../../services/cart.service';
import QRCode from 'qrcode';
@Component({
  selector: 'app-payos-payment-modal',
  standalone: true,
  imports: [CommonModule, VndCurrencyPipe],
  templateUrl: './payos-payment-modal.component.html',
  styleUrls: ['../admin/admin-shared.css']
})
export class PayosPaymentModalComponent implements OnInit { // Implement OnInit
  @Input({ required: true }) paymentData!: PayOsPaymentResponse;
  @Output() close = new EventEmitter<void>();

  private router = inject(Router);
  private cartService = inject(CartService);
  private sanitizer = inject(DomSanitizer); // <<< INJECT DOMSANITIZER >>>

  // <<< TẠO MỘT SIGNAL ĐỂ LƯU URL AN TOÀN >>>
  safeQrCodeUrl = signal<SafeResourceUrl | null>(null);

  ngOnInit(): void {
    // <<< SỬA LẠI TOÀN BỘ LOGIC Ở ĐÂY >>>
    if (this.paymentData?.data?.qrCode) {
      // Sử dụng thư viện QRCode để chuyển đổi chuỗi VietQR thành một Data URL (base64)
      QRCode.toDataURL(this.paymentData.data.qrCode)
        .then(url => {
          // Sau khi có Data URL, bảo Angular tin tưởng nó
          this.safeQrCodeUrl.set(this.sanitizer.bypassSecurityTrustResourceUrl(url));
        })
        .catch(err => {
          console.error('Failed to generate QR code', err);
        });
    }
  }

  closeModal(): void {
    this.close.emit();
  }

  checkOrderStatus(): void {
    this.cartService.clearCart().subscribe();
    this.router.navigate(['/orders', this.paymentData.data?.orderCode]);
    this.closeModal();
  }
}