import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root' // Cung cấp service ở cấp độ root để có thể inject ở bất cứ đâu
})
export class UtilsService {

  constructor() { }

  /**
   * Trả về một chuỗi class của Tailwind CSS dựa trên trạng thái đơn hàng.
   * @param status Trạng thái đơn hàng (ví dụ: 'Pending', 'Delivered').
   * @returns Chuỗi class CSS.
   */
  getOrderStatusClass(status: string): string {
    if (!status) {
      return 'bg-gray-100 text-gray-800'; // Trạng thái mặc định nếu status là null/undefined
    }

    switch (status.toLowerCase()) {
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'processing':
        return 'bg-blue-100 text-blue-800';
      case 'shipped':
        return 'bg-indigo-100 text-indigo-800';
      case 'delivered':
        return 'bg-green-100 text-green-800';
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  // Bạn có thể thêm các hàm tiện ích khác vào đây trong tương lai
  // Ví dụ:
  // getPaymentStatusClass(status: string): string { ... }
}