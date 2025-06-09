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
  orderItems: OrderItemDto[];
  userFullName?: string;
  userEmail?: string;
}