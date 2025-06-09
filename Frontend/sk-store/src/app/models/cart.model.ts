export interface CartItemDto {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  productImageUrl?: string;
  stockQuantity: number;
}

export interface CartDto {
  cartId: number;
  userId: number;
  items: CartItemDto[];
  grandTotal: number;
  totalItems: number;
  creationDate: string;
  lastUpdatedDate?: string;
}

// DTO để gửi lên server khi thêm sản phẩm
export interface AddItemToCartDto {
  productId: number;
  quantity: number;
}

// DTO để gửi lên server khi cập nhật số lượng
export interface UpdateCartItemQuantityDto {
  quantity: number;
}