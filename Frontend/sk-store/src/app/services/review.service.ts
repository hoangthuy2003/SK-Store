import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ReviewDto {
  reviewId: number;
  userName?: string;
  rating: number;
  comment?: string;
  reviewDate: Date;
}

export interface CreateReviewDto {
  rating: number;
  comment?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Lấy tất cả đánh giá của một sản phẩm
  getReviewsForProduct(productId: number): Observable<ReviewDto[]> {
    return this.http.get<ReviewDto[]>(`${this.apiUrl}/api/products/${productId}/reviews`);
  }

  // Thêm đánh giá mới cho sản phẩm (yêu cầu đăng nhập)
  addReview(productId: number, reviewData: CreateReviewDto): Observable<ReviewDto> {
    return this.http.post<ReviewDto>(`${this.apiUrl}/api/products/${productId}/reviews`, reviewData);
  }

  // Xóa đánh giá (admin only)
  deleteReview(reviewId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/api/reviews/${reviewId}`);
  }
}
