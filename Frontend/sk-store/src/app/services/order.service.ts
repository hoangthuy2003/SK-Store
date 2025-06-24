import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  CreateOrderRequestDto, 
  OrderDto, 
  OrderFilterParameters, 
  UpdateOrderPaymentStatusDto, 
  UpdateOrderStatusDto 
} from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/orders`;

  // ===============================================
  // PHƯƠNG THỨC DÀNH CHO NGƯỜI DÙNG (CUSTOMER)
  // ===============================================

  /**
   * Tạo một đơn hàng mới.
   * @param orderData Dữ liệu để tạo đơn hàng.
   * @returns Observable chứa thông tin đơn hàng vừa tạo.
   */
  createOrder(orderData: CreateOrderRequestDto): Observable<OrderDto> {
    return this.http.post<OrderDto>(this.apiUrl, orderData);
  }

  /**
   * Lấy lịch sử đơn hàng của người dùng đang đăng nhập (có phân trang).
   * @param page Số trang.
   * @param pageSize Kích thước trang.
   * @returns Observable chứa HttpResponse với danh sách đơn hàng và header X-Total-Count.
   */
  getMyOrders(page: number, pageSize: number): Observable<HttpResponse<OrderDto[]>> {
    const params = new HttpParams()
      .set('pageNumber', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<OrderDto[]>(this.apiUrl, { params, observe: 'response' });
  }

  /**
   * Lấy chi tiết một đơn hàng theo ID.
   * API này được dùng chung cho cả user và admin. Backend sẽ kiểm tra quyền truy cập.
   * @param id ID của đơn hàng.
   * @returns Observable chứa thông tin chi tiết đơn hàng.
   */
  getOrderById(id: number): Observable<OrderDto> {
    return this.http.get<OrderDto>(`${this.apiUrl}/${id}`);
  }
updateOrderPaymentStatus(id: number, newStatus: string): Observable<void> {
    const body: UpdateOrderPaymentStatusDto = { newPaymentStatus: newStatus };
    return this.http.put<void>(`${this.apiUrl}/admin/${id}/payment-status`, body);
}

  // ===============================================
  // PHƯƠNG THỨC DÀNH CHO QUẢN TRỊ VIÊN (ADMIN)
  // ===============================================

  /**
   * [Admin] Lấy danh sách tất cả đơn hàng với bộ lọc và phân trang.
   * @param filters Các tham số để lọc và phân trang.
   * @returns Observable chứa HttpResponse với danh sách đơn hàng và header X-Total-Count.
   */
  getAdminOrders(filters: OrderFilterParameters): Observable<HttpResponse<OrderDto[]>> {
    let params = new HttpParams()
      .set('pageNumber', filters.pageNumber.toString())
      .set('pageSize', filters.pageSize.toString());

    if (filters.orderStatus) {
      params = params.append('orderStatus', filters.orderStatus);
    }
    if (filters.paymentStatus) {
      params = params.append('paymentStatus', filters.paymentStatus);
    }
    if (filters.searchTerm) {
      params = params.append('searchTerm', filters.searchTerm);
    }
    if (filters.fromDate) {
      params = params.append('fromDate', filters.fromDate);
    }
    if (filters.toDate) {
      params = params.append('toDate', filters.toDate);
    }

    // <<< THÊM DUY NHẤT DÒNG NÀY ĐỂ GIẢI QUYẾT VẤN ĐỀ >>>
    params = params.append('_cacheBuster', new Date().getTime().toString());

    return this.http.get<OrderDto[]>(`${this.apiUrl}/admin/all`, { params, observe: 'response' });
  }

  /**
   * [Admin] Cập nhật trạng thái của một đơn hàng.
   * @param id ID của đơn hàng.
   * @param newStatus Trạng thái mới.
   * @returns Observable<void> vì API trả về 204 No Content.
   */
  updateOrderStatus(id: number, newStatus: string): Observable<void> {
    const body: UpdateOrderStatusDto = { newStatus };
    return this.http.put<void>(`${this.apiUrl}/admin/${id}/status`, body);
  }
}