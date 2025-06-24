import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';

// Services
import { OrderService } from '../../../services/order.service';
import { NotificationService } from '../../../services/notification.service';
import { UtilsService } from '../../../services/utils.service';

// Models
import { OrderDto, OrderFilterParameters } from '../../../models/order.model';

// Components & Pipes
import { PaginationComponent } from '../../../components/pagination/pagination.component';
import { VndCurrencyPipe } from '../../../pipes/vnd-currency.pipe';

@Component({
  selector: 'app-admin-order-list',
  standalone: true,
  imports: [CommonModule, PaginationComponent, VndCurrencyPipe, RouterLink],
  templateUrl: './order-list.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminOrderListComponent implements OnInit {
  private orderService = inject(OrderService);
  private notifier = inject(NotificationService);
  public utilsService = inject(UtilsService);
updatingPaymentStatusForOrderId = signal<number | null>(null); // Thêm signal mới
  // State signals
  orders = signal<OrderDto[]>([]);
  isLoading = signal(true);
  updatingStatusForOrderId = signal<number | null>(null); // Theo dõi đơn hàng đang được cập nhật

  // Pagination signals
  currentPage = signal(1);
  pageSize = 10;
  totalItems = signal(0);

  // <<< THÊM MẢNG TRẠNG THÁI ĐƠN HÀNG >>>
  readonly paymentStatuses: string[] = ['Unpaid', 'Paid', 'Failed', 'Refunded'];
  readonly orderStatuses: string[] = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading.set(true);
    const filters: OrderFilterParameters = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize,
    };
    this.orderService.getAdminOrders(filters).subscribe({
      next: (response: HttpResponse<OrderDto[]>) => {
        this.orders.set(response.body || []);
        const totalCount = response.headers.get('X-Total-Count');
        this.totalItems.set(totalCount ? +totalCount : 0);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.notifier.showError("Không thể tải danh sách đơn hàng.");
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadOrders();
  }

  // <<< THÊM PHƯƠNG THỨC CẬP NHẬT TRẠNG THÁI >>>
  onStatusChange(order: OrderDto, event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const newStatus = selectElement.value;

    if (!newStatus || newStatus === order.orderStatus) {
      return; // Không làm gì nếu không có thay đổi
    }

    this.updatingStatusForOrderId.set(order.orderId);

    this.orderService.updateOrderStatus(order.orderId, newStatus).subscribe({
      next: () => {
        this.notifier.showSuccess(`Đã cập nhật trạng thái đơn hàng #${order.orderId} thành "${newStatus}".`);
        // Cập nhật lại trạng thái của đơn hàng trong danh sách mà không cần gọi lại API
        this.orders.update(currentOrders => 
          currentOrders.map(o => o.orderId === order.orderId ? { ...o, orderStatus: newStatus } : o)
        );
        this.updatingStatusForOrderId.set(null);
      },
      error: (err) => {
        this.notifier.showError(err.error?.message || 'Cập nhật trạng thái thất bại.');
        // Reset lại giá trị của dropdown về trạng thái cũ
        selectElement.value = order.orderStatus;
        this.updatingStatusForOrderId.set(null);
      }
    });
  }
  onPaymentStatusChange(order: OrderDto, event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const newStatus = selectElement.value;

    if (!newStatus || newStatus === order.paymentStatus) {
      return;
    }

    this.updatingPaymentStatusForOrderId.set(order.orderId);

    this.orderService.updateOrderPaymentStatus(order.orderId, newStatus).subscribe({
      next: () => {
        this.notifier.showSuccess(`Đã cập nhật TT thanh toán của đơn hàng #${order.orderId}.`);
        this.orders.update(currentOrders => 
          currentOrders.map(o => o.orderId === order.orderId ? { ...o, paymentStatus: newStatus } : o)
        );
        this.updatingPaymentStatusForOrderId.set(null);
      },
      error: (err) => {
        this.notifier.showError(err.error?.message || 'Cập nhật thất bại.');
        selectElement.value = order.paymentStatus;
        this.updatingPaymentStatusForOrderId.set(null);
      }
    });
  }
}