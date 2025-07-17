import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
// Sửa lại import để bao gồm UpdateProductDto
import { ProductDto, ProductFilterParameters, ProductDetailDto, CreateProductDto, UpdateProductDto, CreateProductWithFilesDto, UpdateProductWithFilesDto, ProductFileUploadResponse } from '../models/product.model';

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

    // Methods mới cho upload file
    createProductWithFiles(productData: CreateProductWithFilesDto, imageFiles?: File[]): Observable<ProductDetailDto> {
        const formData = new FormData();
        
        // Thêm dữ liệu sản phẩm vào FormData
        formData.append('ProductName', productData.productName);
        if (productData.description) {
            formData.append('Description', productData.description);
        }
        formData.append('Price', productData.price.toString());
        formData.append('StockQuantity', productData.stockQuantity.toString());
        formData.append('CategoryId', productData.categoryId.toString());
        formData.append('BrandId', productData.brandId.toString());
        formData.append('IsActive', productData.isActive.toString());
        
        if (productData.primaryImageIndex !== undefined) {
            formData.append('PrimaryImageIndex', productData.primaryImageIndex.toString());
        }
        
        // Thêm files vào FormData
        if (imageFiles && imageFiles.length > 0) {
            imageFiles.forEach(file => {
                formData.append('ImageFiles', file);
            });
        }
        
        return this.http.post<ProductDetailDto>(`${this.apiUrl}/upload`, formData).pipe(
            catchError(this.handleError)
        );
    }

    updateProductWithFiles(id: number, productData: UpdateProductWithFilesDto, imageFiles?: File[]): Observable<ProductDetailDto> {
        const formData = new FormData();
        
        // Thêm dữ liệu sản phẩm vào FormData (tất cả fields theo backend)
        formData.append('ProductName', productData.productName || '');
        formData.append('Description', productData.description || '');
        formData.append('Price', productData.price?.toString() || '0');
        formData.append('StockQuantity', productData.stockQuantity?.toString() || '0');
        formData.append('CategoryId', productData.categoryId?.toString() || '0');
        formData.append('BrandId', productData.brandId?.toString() || '0');
        formData.append('IsActive', productData.isActive?.toString() || 'true');
        formData.append('ReplaceAllImages', productData.replaceAllImages?.toString() || 'false');
        formData.append('PrimaryImageIndex', productData.primaryImageIndex?.toString() || '0');
        
        // Thêm danh sách ID ảnh cần xóa
        if (productData.imagesToDelete && productData.imagesToDelete.length > 0) {
            productData.imagesToDelete.forEach((imageId, index) => {
                formData.append(`ImagesToDelete[${index}]`, imageId.toString());
            });
        }
        
        // Thêm files vào FormData
        if (imageFiles && imageFiles.length > 0) {
            imageFiles.forEach(file => {
                formData.append('ImageFiles', file);
            });
        }
        
        return this.http.put<ProductDetailDto>(`${this.apiUrl}/${id}/upload`, formData).pipe(
            catchError(this.handleError)
        );
    }

    // Method để xóa ảnh riêng lẻ
    deleteProductImage(productId: number, imageId: number): Observable<void> {
        const url = `${this.apiUrl}/${productId}/images/${imageId}`;
        console.log('DELETE request to:', url);
        return this.http.delete<void>(url).pipe(
            catchError(this.handleError)
        );
    }

    // Method để validate image files
    validateImageFiles(files: File[]): { isValid: boolean; errors: string[] } {
        const errors: string[] = [];
        const maxFileSize = 5 * 1024 * 1024; // 5MB
        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];
        const allowedExtensions = ['.jpg', '.jpeg', '.png'];

        files.forEach((file, index) => {
            // Kiểm tra kích thước file
            if (file.size > maxFileSize) {
                errors.push(`File ${file.name} vượt quá kích thước cho phép (5MB)`);
            }

            // Kiểm tra MIME type
            if (!allowedTypes.includes(file.type.toLowerCase())) {
                errors.push(`File ${file.name} không đúng định dạng. Chỉ chấp nhận JPG, JPEG, PNG`);
            }

            // Kiểm tra extension
            const extension = file.name.toLowerCase().substring(file.name.lastIndexOf('.'));
            if (!allowedExtensions.includes(extension)) {
                errors.push(`File ${file.name} có phần mở rộng không hợp lệ`);
            }
        });

        return {
            isValid: errors.length === 0,
            errors: errors
        };
    }

    private handleError(error: any): Observable<never> {
        console.error('An error occurred in ProductService:', error);
        
        // Xử lý lỗi SSL/TLS cụ thể
        if (error.error?.code === 'DEPTH_ZERO_SELF_SIGNED_CERT' || 
            error.status === 0 && error.statusText === 'Unknown Error') {
            console.error('SSL Certificate error detected. Please check your backend SSL configuration.');
            return throwError(() => new Error('Lỗi kết nối SSL. Vui lòng kiểm tra cấu hình backend hoặc liên hệ quản trị viên.'));
        }
        
        // Sửa lại để lấy message từ error.error.message nếu có
        const errorMessage = error.error?.message || 'Đã có lỗi xảy ra với sản phẩm. Vui lòng thử lại.';
        return throwError(() => new Error(errorMessage));
    }
}