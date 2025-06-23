import { Injectable, signal, WritableSignal } from '@angular/core';

// Định nghĩa kiểu dữ liệu cho một thông báo
export interface Notification {
  id: number;
  message: string;
  type: 'success' | 'error' | 'info' | 'warning';
  duration?: number; // Thời gian hiển thị (ms)
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  // Signal chứa danh sách các thông báo đang hiển thị
  notifications: WritableSignal<Notification[]> = signal([]);

  private nextId = 0;

  constructor() { }

  /**
   * Hiển thị một thông báo thành công.
   * @param message Nội dung thông báo.
   * @param duration Thời gian hiển thị (ms). Mặc định là 3000ms.
   */
  showSuccess(message: string, duration: number = 3000): void {
    this.addNotification({ message, type: 'success', duration });
  }

  /**
   * Hiển thị một thông báo lỗi.
   * @param message Nội dung thông báo.
   * @param duration Thời gian hiển thị (ms). Mặc định là 5000ms.
   */
  showError(message: string, duration: number = 5000): void {
    this.addNotification({ message, type: 'error', duration });
  }
  
  /**
   * Hiển thị một thông báo thông tin.
   * @param message Nội dung thông báo.
   * @param duration Thời gian hiển thị (ms). Mặc định là 3000ms.
   */
  showInfo(message: string, duration: number = 3000): void {
    this.addNotification({ message, type: 'info', duration });
  }

  private addNotification(notification: Omit<Notification, 'id'>): void {
    const id = this.nextId++;
    const newNotification: Notification = { id, ...notification };

    // Thêm thông báo mới vào đầu danh sách
    this.notifications.update(currentNotifications => [newNotification, ...currentNotifications]);

    // Tự động xóa thông báo sau một khoảng thời gian
    if (notification.duration) {
      setTimeout(() => this.removeNotification(id), notification.duration);
    }
  }

  /**
   * Xóa một thông báo khỏi danh sách.
   * @param id ID của thông báo cần xóa.
   */
  removeNotification(id: number): void {
    this.notifications.update(currentNotifications =>
      currentNotifications.filter(n => n.id !== id)
    );
  }
}