import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, shareReplay, tap, map } from 'rxjs/operators'; // Thêm map
import { environment } from '../../environments/environment';
import { CategoryDto, CreateCategoryDto, UpdateCategoryDto } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/categories`;
  private allCategories$: Observable<CategoryDto[]> | null = null; // Cache cho tất cả categories

  // Phương thức lấy dữ liệu CÓ PHÂN TRANG (cho trang list)
  getPagedCategories(page: number, pageSize: number): Observable<HttpResponse<CategoryDto[]>> {
    const params = new HttpParams()
        .set('pageNumber', page.toString())
        .set('pageSize', pageSize.toString());
    return this.http.get<CategoryDto[]>(this.apiUrl, { params, observe: 'response' });
  }

  // <<< THÊM PHƯƠNG THỨC MỚI ĐỂ LẤY TẤT CẢ >>>
  // Phương thức này sẽ lấy tất cả danh mục, không phân trang, và cache lại
  getAllCategories(): Observable<CategoryDto[]> {
    if (!this.allCategories$) {
      // Gọi API không có tham số phân trang
      this.allCategories$ = this.http.get<CategoryDto[]>(this.apiUrl).pipe(
        shareReplay(1), // Cache lại kết quả
        catchError(this.handleError)
      );
    }
    return this.allCategories$;
  }

  // ... (các phương thức create, update, delete giữ nguyên nhưng cần sửa clearCache)

  createCategory(category: CreateCategoryDto): Observable<CategoryDto> {
    return this.http.post<CategoryDto>(this.apiUrl, category).pipe(tap(() => this.clearCache()));
  }

  updateCategory(id: number, category: UpdateCategoryDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, category).pipe(tap(() => this.clearCache()));
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(tap(() => this.clearCache()));
  }

  private clearCache() {
    // Xóa cả hai cache khi có thay đổi
    this.allCategories$ = null;
  }

  private handleError(error: any): Observable<never> {
    // ...
    const errorMessage = error.error?.message || 'Lỗi xử lý danh mục.';
    return throwError(() => new Error(errorMessage));
  }
  getCategoryById(id: number): Observable<CategoryDto> {
    return this.http.get<CategoryDto>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }
}