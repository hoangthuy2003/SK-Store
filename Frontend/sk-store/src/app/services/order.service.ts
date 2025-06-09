import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateOrderRequestDto, OrderDto } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/orders`;

  // Tạo một đơn hàng mới
  createOrder(orderData: CreateOrderRequestDto): Observable<OrderDto> {
    return this.http.post<OrderDto>(this.apiUrl, orderData);
  }

  // Lấy tất cả đơn hàng của người dùng hiện tại
  getMyOrders(): Observable<OrderDto[]> {
    return this.http.get<OrderDto[]>(this.apiUrl);
  }

  // Lấy chi tiết một đơn hàng theo ID
  getOrderById(id: number): Observable<OrderDto> {
    return this.http.get<OrderDto>(`${this.apiUrl}/${id}`);
  }
}