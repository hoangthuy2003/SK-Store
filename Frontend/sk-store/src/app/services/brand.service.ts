import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, shareReplay, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { BrandDto, CreateBrandDto, UpdateBrandDto } from '../models/brand.model';

@Injectable({
  providedIn: 'root'
})
export class BrandService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/brands`;

  // Cache cho việc lấy TẤT CẢ thương hiệu (dùng cho dropdown, filter)
  private allBrands$: Observable<BrandDto[]> | null = null;

  /**
   * Lấy danh sách thương hiệu CÓ PHÂN TRANG.
   * Dùng cho trang quản lý danh sách thương hiệu.
   * @param page Số trang hiện tại.
   * @param pageSize Số lượng item mỗi trang.
   * @returns Một Observable chứa HttpResponse đầy đủ.
   */
  getPagedBrands(page: number, pageSize: number): Observable<HttpResponse<BrandDto[]>> {
    const params = new HttpParams()
        .set('pageNumber', page.toString())
        .set('pageSize', pageSize.toString());
    // Không cache kết quả này vì nó thay đổi theo trang
    return this.http.get<BrandDto[]>(this.apiUrl, { params, observe: 'response' }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Lấy TẤT CẢ các thương hiệu, không phân trang.
   * Dùng cho các dropdown chọn thương hiệu trong form sản phẩm.
   * Kết quả sẽ được cache lại để tăng hiệu suất.
   * @returns Một Observable chứa mảng BrandDto.
   */
  getAllBrands(): Observable<BrandDto[]> {
    if (!this.allBrands$) {
      this.allBrands$ = this.http.get<BrandDto[]>(this.apiUrl).pipe(
        shareReplay(1), // Cache và chia sẻ kết quả cho các subscriber sau
        catchError(this.handleError)
      );
    }
    return this.allBrands$;
  }

  getBrandById(id: number): Observable<BrandDto> {
    return this.http.get<BrandDto>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  createBrand(brand: CreateBrandDto): Observable<BrandDto> {
    return this.http.post<BrandDto>(this.apiUrl, brand).pipe(
      tap(() => this.clearCache()),
      catchError(this.handleError)
    );
  }

  updateBrand(id: number, brand: UpdateBrandDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, brand).pipe(
      tap(() => this.clearCache()),
      catchError(this.handleError)
    );
  }

  deleteBrand(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.clearCache()),
      catchError(this.handleError)
    );
  }

  /**
   * Xóa cache của danh sách tất cả thương hiệu khi có sự thay đổi dữ liệu.
   */
  private clearCache() {
    this.allBrands$ = null;
  }

  private handleError(error: any): Observable<never> {
    console.error('An error occurred in BrandService:', error);
    const errorMessage = error.error?.message || 'Đã có lỗi xảy ra với thương hiệu. Vui lòng thử lại.';
    return throwError(() => new Error(errorMessage));
  }
}