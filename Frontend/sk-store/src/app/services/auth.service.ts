import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, throwError } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.model';
import { environment } from '../../environments/environment';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { UserPayload } from '../models/user.model';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // THAY ĐỔI: Sử dụng UserPayload thay vì any
  private currentUserSubject = new BehaviorSubject<UserPayload | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private readonly TOKEN_KEY = 'auth_token';
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
    const token = this.getToken();
    if (token) {
      try {
        const decodedToken: UserPayload = jwtDecode(token);
        // Kiểm tra token còn hạn không trước khi set
        if (decodedToken.exp * 1000 > Date.now()) {
          this.currentUserSubject.next(decodedToken);
        } else {
          // Token hết hạn, xóa nó đi
          this.logout();
        }
      } catch (error) {
        console.error("Failed to decode token", error);
        this.logout();
      }
    }
  }

  public isUserInRole(requiredRole: string): boolean {
    const user = this.currentUserSubject.value;
    if (!user || !user.role) {
      return false;
    }
    // `user.role` giờ đã được định kiểu chặt chẽ
    return user.role === requiredRole;
  }

  getToken(): string | null {
    if (this.isBrowser) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  isAuthenticated(): boolean {
    const user = this.currentUserSubject.value;
    // Chỉ cần kiểm tra xem có user trong subject hay không
    return !!user;
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/api/auth/login`, request)
      .pipe(
        tap(response => {
          if (response.isSuccess && response.token) {
            if (this.isBrowser) {
              localStorage.setItem(this.TOKEN_KEY, response.token);
              try {
                const decodedToken: UserPayload = jwtDecode(response.token);
                this.currentUserSubject.next(decodedToken);
              } catch (error) {
                console.error("Failed to decode token on login", error);
              }
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