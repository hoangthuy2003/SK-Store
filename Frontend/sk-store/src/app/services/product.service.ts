import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { ProductDto, ProductFilterParameters, ProductDetailDto } from '../models/product.model';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/api/products`;

    /**
     * Lấy danh sách sản phẩm cùng với thông tin phân trang từ headers.
     * @param filters Các tham số để lọc và phân trang sản phẩm.
     * @returns Một Observable chứa HttpResponse đầy đủ (bao gồm body và headers).
     */
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
        // Thêm các tham số sắp xếp nếu cần
        // if (filters.sortBy) {
        //     params = params.append('sortBy', filters.sortBy);
        // }
        // if (filters.sortDirection) {
        //     params = params.append('sortDirection', filters.sortDirection);
        // }

        // Sử dụng { observe: 'response' } để lấy toàn bộ HttpResponse
        return this.http.get<ProductDto[]>(this.apiUrl, { params, observe: 'response' }).pipe(
            catchError(this.handleError)
        );
    }

    /**
     * Lấy danh sách các sản phẩm nổi bật (chỉ lấy phần body).
     * @param limit Số lượng sản phẩm nổi bật cần lấy.
     * @returns Một Observable chứa mảng các ProductDto.
     */
    getFeaturedProducts(limit: number = 4): Observable<ProductDto[]> {
        const filters: ProductFilterParameters = {
            pageNumber: 1,
            pageSize: limit,
            // Bạn có thể thêm các tham số sắp xếp ở đây nếu backend hỗ trợ
            // ví dụ: sắp xếp theo sản phẩm mới nhất hoặc đánh giá cao nhất
        };
        
        // Gọi hàm getProductsWithCount nhưng chỉ lấy phần body của response
        return this.getProductsWithCount(filters).pipe(
            map(response => response.body || []) // Trích xuất body, trả về mảng rỗng nếu body là null
        );
    }

    /**
     * Lấy thông tin chi tiết của một sản phẩm theo ID.
     * @param id ID của sản phẩm.
     * @returns Một Observable chứa ProductDetailDto.
     */
    getProductById(id: number): Observable<ProductDetailDto> {
        return this.http.get<ProductDetailDto>(`${this.apiUrl}/${id}`).pipe(
            catchError(this.handleError)
        );
    }

    /**
     * Hàm xử lý lỗi tập trung cho ProductService.
     * @param error Lỗi trả về từ HttpClient.
     * @returns Một Observable ném ra lỗi đã được xử lý.
     */
    private handleError(error: any): Observable<never> {
        // Ghi log lỗi ra console để debug
        console.error('An error occurred in ProductService:', error);
        
        // Tạo một thông điệp lỗi thân thiện hơn để component có thể sử dụng
        const userFriendlyError = new Error('Không thể tải dữ liệu sản phẩm. Vui lòng thử lại sau.');
        
        // Ném ra lỗi mới để các subscriber có thể bắt được
        return throwError(() => userFriendlyError);
    }
}