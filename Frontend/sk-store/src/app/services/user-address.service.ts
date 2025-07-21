import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface UserAddressDto {
  addressId: number;
  userId: number;
  addressLine: string;
  recipientName?: string;
  recipientPhoneNumber?: string;
  isDefault: boolean;
}

export interface CreateUserAddressDto {
  addressLine: string;
  recipientName?: string;
  recipientPhoneNumber?: string;
  isDefault?: boolean;
}

export interface UpdateUserAddressDto {
  addressLine: string;
  recipientName?: string;
  recipientPhoneNumber?: string;
  isDefault?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UserAddressService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Lấy tất cả địa chỉ của người dùng hiện tại
  getUserAddresses(): Observable<UserAddressDto[]> {
    return this.http.get<UserAddressDto[]>(`${this.apiUrl}/api/UserAddress`);
  }

  // Lấy địa chỉ mặc định
  getDefaultAddress(): Observable<UserAddressDto> {
    return this.http.get<UserAddressDto>(`${this.apiUrl}/api/UserAddress/default`);
  }

  // Lấy địa chỉ theo ID
  getAddressById(addressId: number): Observable<UserAddressDto> {
    return this.http.get<UserAddressDto>(`${this.apiUrl}/api/UserAddress/${addressId}`);
  }

  // Thêm địa chỉ mới
  addAddress(addressData: CreateUserAddressDto): Observable<UserAddressDto> {
    return this.http.post<UserAddressDto>(`${this.apiUrl}/api/UserAddress`, addressData);
  }

  // Cập nhật địa chỉ
  updateAddress(addressId: number, addressData: UpdateUserAddressDto): Observable<UserAddressDto> {
    return this.http.put<UserAddressDto>(`${this.apiUrl}/api/UserAddress/${addressId}`, addressData);
  }

  // Xóa địa chỉ
  deleteAddress(addressId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/UserAddress/${addressId}`);
  }

  // Đặt địa chỉ làm mặc định
  setDefaultAddress(addressId: number): Observable<{message: string}> {
    return this.http.put<{message: string}>(`${this.apiUrl}/api/UserAddress/${addressId}/set-default`, {});
  }
}
