import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent {
  // Inject service và làm nó public để template có thể truy cập trực tiếp signal
  public notificationService = inject(NotificationService);

  // Hàm để lấy class CSS dựa trên loại thông báo
  getNotificationClass(type: 'success' | 'error' | 'info' | 'warning'): string {
    switch (type) {
      case 'success':
        return 'bg-green-500';
      case 'error':
        return 'bg-red-500';
      case 'info':
        return 'bg-blue-500';
      case 'warning':
        return 'bg-yellow-500';
    }
  }
  
  // Hàm để lấy icon dựa trên loại thông báo
  getNotificationIcon(type: 'success' | 'error' | 'info' | 'warning'): string {
    switch (type) {
      case 'success':
        return 'fa-check-circle';
      case 'error':
        return 'fa-times-circle';
      case 'info':
        return 'fa-info-circle';
      case 'warning':
        return 'fa-exclamation-triangle';
    }
  }
}