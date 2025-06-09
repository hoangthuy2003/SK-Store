import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, throwError } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.model';
import { environment } from '../../environments/environment';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<any | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'current_user';
  private isBrowser: boolean;

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
    if (this.isBrowser) {
      this.loadStoredUser();
    }
  }

  private loadStoredUser(): void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    const user = localStorage.getItem(this.USER_KEY);
    if (token && user) {
      this.currentUserSubject.next(JSON.parse(user));
    }
  }
// Thêm vào AuthService (src/app/services/auth.service.ts)
public isUserInRole(requiredRole: string): boolean {
  const user = this.currentUserSubject.value; // Lấy thông tin user từ BehaviorSubject
  if (!user || !user.role) {
    return false;
  }
  // Giả sử claim role trong token có tên là 'role'
  // Có thể là một mảng hoặc một chuỗi, cần xử lý cho cả 2 trường hợp
  if (Array.isArray(user.role)) {
    return user.role.includes(requiredRole);
  }
  return user.role === requiredRole;
}
  getToken(): string | null {
    if (this.isBrowser) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    // Kiểm tra token hết hạn
    try {
      const tokenData = JSON.parse(atob(token.split('.')[1]));
      return tokenData.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/api/auth/login`, request)
      .pipe(
        tap(response => {
          if (response.isSuccess && response.token) {
            if (this.isBrowser) {
              localStorage.setItem(this.TOKEN_KEY, response.token);
              // Lưu thông tin user từ token
              const tokenData = JSON.parse(atob(response.token.split('.')[1]));
              localStorage.setItem(this.USER_KEY, JSON.stringify(tokenData));
              this.currentUserSubject.next(tokenData);
            }
          }
        })
      );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/api/auth/register`, request);
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.USER_KEY);
    }
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/auth/forgot-password`, { email });
  }

  resetPassword(email: string, otp: string, newPassword: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/auth/reset-password`, {
      email,
      otp,
      newPassword
    });
  }

  verifyEmail(otp: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/auth/verify-email`, { otp });
  }

  sendVerificationEmail(): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/auth/send-verification-email`, {});
  }
} 