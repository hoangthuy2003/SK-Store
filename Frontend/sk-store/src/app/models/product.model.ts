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
  isActive?: boolean | null;
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

export interface CreateProductDto {
  productName: string;
  description?: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  brandId: number;
  isActive: boolean;
  // Tạm thời chưa xử lý Images và Attributes trong form này cho đơn giản
}
export interface UpdateProductDto { // <<< THÊM EXPORT VÀO ĐÂY
  productName?: string;
  description?: string;
  price?: number;
  stockQuantity?: number;
  categoryId?: number;
  brandId?: number;
  isActive?: boolean;
}

// DTOs cho upload file
export interface CreateProductWithFilesDto {
  productName: string;
  description?: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  brandId: number;
  isActive: boolean;
  primaryImageIndex?: number;
  // ImageFiles sẽ được thêm vào FormData, không cần define ở đây
}

export interface UpdateProductWithFilesDto {
  productName?: string;
  description?: string;
  price?: number;
  stockQuantity?: number;
  categoryId?: number;
  brandId?: number;
  isActive?: boolean;
  primaryImageIndex?: number;
  replaceAllImages?: boolean;
  imagesToDelete?: number[]; // Thêm property này
  // ImageFiles sẽ được thêm vào FormData
}

export interface ProductFileUploadResponse {
  productId: number;
  productName: string;
  price: number;
  productImages: ProductImageDto[];
}

export interface CreateProductAttributeDto {
  attributeName: string;
  attributeValue: string;
}

export interface UpdateProductAttributeDto {
  attributeId?: number;
  attributeName: string;
  attributeValue: string;
}

