import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  
  /**
   * Chuyển đổi URL ảnh tương đối thành URL đầy đủ
   * @param imageUrl URL ảnh từ backend (có thể là relative hoặc absolute)
   * @returns URL đầy đủ để hiển thị ảnh
   */
  getFullImageUrl(imageUrl: string): string {
    if (!imageUrl) {
      return this.getPlaceholderUrl();
    }

    // Nếu URL đã là absolute (bắt đầu với http:// hoặc https://)
    if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
      return imageUrl;
    }

    // Nếu URL là relative, thêm staticUrl vào đầu
    if (imageUrl.startsWith('/')) {
      return `${environment.staticUrl}${imageUrl}`;
    }

    // Nếu URL không bắt đầu với /, thêm / vào
    return `${environment.staticUrl}/${imageUrl}`;
  }

  /**
   * Trả về URL ảnh placeholder mặc định
   */
  getPlaceholderUrl(): string {
    return 'https://via.placeholder.com/300x200?text=No+Image';
  }

  /**
   * Kiểm tra xem ảnh có tồn tại không
   * @param imageUrl URL ảnh cần kiểm tra
   * @returns Promise<boolean>
   */
  async checkImageExists(imageUrl: string): Promise<boolean> {
    try {
      const response = await fetch(this.getFullImageUrl(imageUrl), { method: 'HEAD' });
      return response.ok;
    } catch {
      return false;
    }
  }
}
