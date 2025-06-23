import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

// <<< THÊM INTERFACE NÀY >>>
export interface PayOsPaymentResponse {
  code: string;
  desc: string;
  data: {
    bin: string;
    accountNumber: string;
    accountName: string;
    amount: number;
    description: string;
    orderCode: number;
    paymentLinkId: string;
    status: string;
    checkoutUrl: string;
    qrCode: string; // Đây là ảnh QR dạng base64
  } | null;
  signature: string;
}

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/payment`;

  // <<< SỬA LẠI PHƯƠNG THỨC NÀY >>>
  createPayOsLink(orderId: number): Observable<PayOsPaymentResponse> {
    return this.http.post<PayOsPaymentResponse>(`${this.apiUrl}/payos/${orderId}`, {});
  }
}