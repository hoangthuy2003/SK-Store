// Định nghĩa cho danh sách sản phẩm (trang shop)
export interface ProductDto {
  productId: number;
  productName: string;
  price: number;
  primaryImageUrl?: string;
  categoryName: string;
  brandName: string;
  averageRating: number;
  reviewCount: number;
  // --- THÊM 2 DÒNG DƯỚI ĐÂY ---
  stockQuantity: number;
  isActive: boolean;
}

// Định nghĩa cho tham số lọc sản phẩm
export interface ProductFilterParameters {
  categoryId?: number | null;
  brandId?: number | null;
  searchTerm?: string | null;
  pageNumber: number;
  pageSize: number;
  // Thêm các tham số sắp xếp nếu cần
  // sortBy?: string;
  // sortDirection?: 'asc' | 'desc';
}


export interface ProductImageDto {
  imageId: number;
  imageUrl: string;
  isPrimary: boolean;
}

export interface ProductAttributeDto {
  attributeId: number;
  attributeName: string;
  attributeValue: string;
}

export interface ReviewDto {
  reviewId: number;
  userName?: string;
  rating: number;
  comment?: string;
  reviewDate: string; // Dùng string để dễ dàng xử lý
}

export interface ProductDetailDto {
  productId: number;
  productName: string;
  description?: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  categoryName: string;
  brandId: number;
  brandName: string;
  isActive: boolean;
  creationDate: string;
  lastUpdatedDate?: string;
  productImages: ProductImageDto[];
  productAttributes: ProductAttributeDto[];
  reviews: ReviewDto[];
  averageRating: number;
  reviewCount: number;
}