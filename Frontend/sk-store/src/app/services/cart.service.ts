import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError, map, switchMap, filter } from 'rxjs/operators'; // Thêm filter và switchMap
import { isPlatformBrowser } from '@angular/common';

import { environment } from '../../environments/environment';
import { CartDto, AddItemToCartDto, UpdateCartItemQuantityDto } from '../models/cart.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private platformId = inject(PLATFORM_ID);

  private apiUrl = `${environment.apiUrl}/api/cart`;

  private cartSubject = new BehaviorSubject<CartDto | null>(null);
  public cart$ = this.cartSubject.asObservable();

  public cartItemCount$ = this.cart$.pipe(
    map(cart => cart?.totalItems ?? 0)
  );

  constructor() {
    if (isPlatformBrowser(this.platformId)) {
      // Lắng nghe sự thay đổi trạng thái đăng nhập
      this.authService.currentUser$.pipe(
        // Chuyển từ observable user sang observable cart
        switchMap(user => {
          if (user) {
            // Nếu có user, gọi API để tải giỏ hàng
            return this.loadCartFromServer();
          } else {
            // Nếu không có user (logout), phát ra giá trị null cho giỏ hàng
            this.cartSubject.next(null);
            return of(null);
          }
        })
      ).subscribe();

      // <<< THÊM ĐOẠN NÀY ĐỂ XỬ LÝ KHI TẢI LẠI TRANG >>>
      // Chủ động kiểm tra trạng thái đăng nhập khi service được khởi tạo lần đầu
      if (this.authService.isAuthenticated()) {
        this.loadCartFromServer().subscribe();
      }
    }
  }

  // Đổi tên `loadCart` thành `loadCartFromServer` để rõ ràng hơn
  private loadCartFromServer(): Observable<CartDto | null> {
    return this.http.get<CartDto>(this.apiUrl).pipe(
      tap(cart => this.cartSubject.next(cart)),
      catchError(err => {
        console.error('Failed to load cart', err);
        this.cartSubject.next(null);
        return of(null);
      })
    );
  }

  // Thêm sản phẩm vào giỏ
  addItem(item: AddItemToCartDto): Observable<CartDto> {
    return this.http.post<CartDto>(`${this.apiUrl}/items`, item).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  // Cập nhật số lượng sản phẩm
  updateItemQuantity(productId: number, quantity: number): Observable<CartDto> {
    const updateDto: UpdateCartItemQuantityDto = { quantity };
    return this.http.put<CartDto>(`${this.apiUrl}/items/${productId}`, updateDto).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  // Xóa sản phẩm khỏi giỏ
  removeItem(productId: number): Observable<CartDto> {
    return this.http.delete<CartDto>(`${this.apiUrl}/items/${productId}`).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  // Làm trống giỏ hàng
  clearCart(): Observable<CartDto> {
    return this.http.delete<CartDto>(this.apiUrl).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }
}