import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, shareReplay } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CategoryDto } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/categories`;

  // Cache lại kết quả để không phải gọi API mỗi lần vào trang shop
  private categories$: Observable<CategoryDto[]> | null = null;

  getCategories(): Observable<CategoryDto[]> {
    if (!this.categories$) {
      this.categories$ = this.http.get<CategoryDto[]>(this.apiUrl).pipe(
        shareReplay(1), // Cache và replay kết quả cho các subscriber sau
        catchError(this.handleError)
      );
    }
    return this.categories$;
  }

  private handleError(error: any): Observable<never> {
    console.error('An error occurred in CategoryService:', error);
    // Có thể throw error để component tự xử lý
    throw new Error('Failed to load categories. Please try again later.');
  }
}