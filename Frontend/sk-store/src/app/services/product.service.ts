import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
// Sửa lại import để bao gồm UpdateProductDto
import { ProductDto, ProductFilterParameters, ProductDetailDto, CreateProductDto, UpdateProductDto } from '../models/product.model';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/api/products`;

    getProductsWithCount(filters: ProductFilterParameters): Observable<HttpResponse<ProductDto[]>> {
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

        // <<< THÊM LOGIC XỬ LÝ CHO ISACTIVE Ở ĐÂY >>>
        // Chỉ thêm tham số 'isActive' vào URL nếu nó không phải là null hoặc undefined.
        // Điều này cho phép admin gửi `null` để lấy tất cả,
        // và trang shop không cần gửi gì cả (backend sẽ dùng giá trị mặc định là true).
        if (filters.isActive !== null && filters.isActive !== undefined) {
            params = params.append('isActive', filters.isActive.toString());
        }
        // <<< KẾT THÚC PHẦN THÊM >>>

        return this.http.get<ProductDto[]>(this.apiUrl, { params, observe: 'response' }).pipe(
            catchError(this.handleError)
        );
    }

    getFeaturedProducts(limit: number = 4): Observable<ProductDto[]> {
        const filters: ProductFilterParameters = {
            pageNumber: 1,
            pageSize: limit,
            // <<< THÊM DÒNG NÀY ĐỂ ĐẢM BẢO TRANG CHỦ CHỈ LẤY SẢN PHẨM ACTIVE >>>
            isActive: true
        };
        
        return this.getProductsWithCount(filters).pipe(
            map(response => response.body || [])
        );
    }

    getProductById(id: number): Observable<ProductDetailDto> {
        return this.http.get<ProductDetailDto>(`${this.apiUrl}/${id}`).pipe(
            catchError(this.handleError)
        );
    }

    createProduct(productData: CreateProductDto): Observable<ProductDetailDto> {
        return this.http.post<ProductDetailDto>(this.apiUrl, productData).pipe(
            catchError(this.handleError)
        );
    }

    updateProduct(id: number, productData: UpdateProductDto): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}`, productData).pipe(
            catchError(this.handleError)
        );
    }

    deleteProduct(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
            catchError(this.handleError)
        );
    }

    private handleError(error: any): Observable<never> {
        console.error('An error occurred in ProductService:', error);
        // Sửa lại để lấy message từ error.error.message nếu có
        const errorMessage = error.error?.message || 'Đã có lỗi xảy ra với sản phẩm. Vui lòng thử lại.';
        return throwError(() => new Error(errorMessage));
    }
}