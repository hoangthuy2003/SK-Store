import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserProfile, UpdateUserProfileDto, ChangePasswordDto } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private profileApiUrl = `${environment.apiUrl}/api/profile`;
  private usersApiUrl = `${environment.apiUrl}/api/users`; // Nếu cần cho các chức năng admin sau này

  // Lấy thông tin hồ sơ người dùng
  getUserProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(this.profileApiUrl);
  }

  // Cập nhật thông tin hồ sơ
  updateUserProfile(data: UpdateUserProfileDto): Observable<any> {
    return this.http.put(this.profileApiUrl, data);
  }

  // Thay đổi mật khẩu
  changePassword(data: ChangePasswordDto): Observable<any> {
    return this.http.post(`${this.profileApiUrl}/change-password`, data);
  }
}