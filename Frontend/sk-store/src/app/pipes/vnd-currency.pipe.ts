import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'vndCurrency',
  standalone: true // Đây là một Standalone Pipe, đúng chuẩn Angular 17+
})
export class VndCurrencyPipe implements PipeTransform {

  transform(value: number | null | undefined): string {
    // Xử lý trường hợp giá trị là null hoặc undefined để tránh lỗi
    if (value === null || value === undefined) {
      return '';
    }

    // Sử dụng Intl.NumberFormat để định dạng tiền tệ
    // Đây chính là logic từ hàm formatPrice() của bạn
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(value);
  }
}