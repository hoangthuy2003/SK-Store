import { CartItemDto } from "./cart.model";

// DTO để gửi lên server khi tạo đơn hàng
export interface CreateOrderRequestDto {
  shippingAddress: string;
  recipientName: string;
  recipientPhoneNumber: string;
  paymentMethod: string; // Ví dụ: 'COD'
  notes?: string;
}

// DTO cho chi tiết một sản phẩm trong đơn hàng
export interface OrderItemDto {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  productImageUrl?: string;
}

// DTO cho chi tiết một đơn hàng hoàn chỉnh
export interface OrderDto {
  orderId: number;
  userId: number;
  orderDate: string;
  orderStatus: string;
  totalAmount: number;
  shippingAddress: string;
  recipientName: string;
  recipientPhoneNumber: string;
  paymentMethod: string;
  paymentStatus: string;
  deliveryDate?: string;
  shippingFee: number;
  notes?: string;
  userFullName?: string;
  userEmail?: string;
  orderItems: OrderItemDto[];
}
export interface UpdateOrderPaymentStatusDto {
  newPaymentStatus: string;
}
// DTO cho tham số lọc đơn hàng của Admin
export interface OrderFilterParameters {
  orderStatus?: string | null;
  paymentStatus?: string | null;
  searchTerm?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
  pageNumber: number;
  pageSize: number;
}

// <<< THÊM EXPORT VÀO INTERFACE NÀY >>>
// DTO để cập nhật trạng thái đơn hàng
export interface UpdateOrderStatusDto {
  newStatus: string;
}