import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { ProductDto, ProductFilterParameters, ProductDetailDto } from '../models/product.model';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/api/products`;

    getProducts(filters: ProductFilterParameters): Observable<ProductDto[]> {
        let params = new HttpParams()
            .set('pageNumber', filters.pageNumber.toString())
            .set('pageSize', filters.pageSize.toString());

        if (filters.categoryId) {
            params = params.append('categoryId', filters.categoryId.toString());
        }
        if (filters.brandId) {
            params = params.append('brandId', filters.brandId.toString());
        }
        if (filters.searchTerm) {
            params = params.append('searchTerm', filters.searchTerm);
        }

        return this.http.get<ProductDto[]>(this.apiUrl, { params }).pipe(
            catchError(this.handleError)
        );
    }
    getFeaturedProducts(limit: number = 4): Observable<ProductDto[]> {
        // Giả sử "sản phẩm nổi bật" là các sản phẩm mới nhất hoặc có đánh giá cao nhất.
        // Chúng ta sẽ lấy trang đầu tiên với `limit` sản phẩm.
        const filters: ProductFilterParameters = {
            pageNumber: 1,
            pageSize: limit,
            // Bạn có thể thêm các tham số sắp xếp ở đây nếu backend hỗ trợ
            // sortBy: 'rating',
            // sortDirection: 'desc'
        };
        return this.getProducts(filters);
    }

    getProductById(id: number): Observable<ProductDetailDto> {
        return this.http.get<ProductDetailDto>(`${this.apiUrl}/${id}`).pipe(
            catchError(this.handleError)
        );
    }
    private handleError(error: any): Observable<never> {
        console.error('An error occurred in ProductService:', error);
        throw new Error('Failed to load products. Please try again later.');
    }
}