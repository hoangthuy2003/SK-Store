import { Component, OnInit, signal, inject, OnDestroy } from '@angular/core'; // Thêm OnDestroy
import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms'; // <<< THÊM IMPORT
import { Subject, Subscription } from 'rxjs'; // <<< THÊM IMPORT
import { debounceTime, distinctUntilChanged } from 'rxjs/operators'; // <<< THÊM IMPORT

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
  imports: [CommonModule, PaginationComponent, VndCurrencyPipe, RouterLink, ReactiveFormsModule], // <<< THÊM ReactiveFormsModule
  templateUrl: './order-list.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminOrderListComponent implements OnInit, OnDestroy { // <<< Implement OnDestroy
  private orderService = inject(OrderService);
  private notifier = inject(NotificationService);
  public utilsService = inject(UtilsService);
  private fb = inject(FormBuilder); // <<< INJECT FormBuilder

  // State signals
  orders = signal<OrderDto[]>([]);
  isLoading = signal(true);
  updatingStatusForOrderId = signal<number | null>(null);
  updatingPaymentStatusForOrderId = signal<number | null>(null);

  // Pagination signals
  currentPage = signal(1);
  pageSize = 10;
  totalItems = signal(0);

  // <<< THÊM LOGIC CHO BỘ LỌC >>>
  filterForm: FormGroup;
  private filterSubscription: Subscription | undefined;

  readonly paymentStatuses: string[] = ['Unpaid', 'Paid', 'Failed', 'Refunded'];
  readonly orderStatuses: string[] = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];

  constructor() {
    this.filterForm = this.fb.group({
      searchTerm: [''],
      orderStatus: [''],
      paymentStatus: [''],
      fromDate: [''], 
      toDate: ['']    
    });
  }

  ngOnInit(): void {
    this.loadOrders();
    // Lắng nghe sự thay đổi của form lọc
    this.filterSubscription = this.filterForm.valueChanges.pipe(
      debounceTime(400), // Chờ 400ms sau khi người dùng nhập xong
      distinctUntilChanged() // Chỉ gọi API nếu giá trị thay đổi
    ).subscribe(() => {
      this.currentPage.set(1); // Reset về trang 1 khi lọc
      this.loadOrders();
    });
  }

  ngOnDestroy(): void {
    // Hủy subscription để tránh memory leak
    this.filterSubscription?.unsubscribe();
  }

  loadOrders(): void {
    this.isLoading.set(true);
    // Lấy giá trị từ form lọc
    const formValue = this.filterForm.value;
    const filters: OrderFilterParameters = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize,
      searchTerm: formValue.searchTerm || null,
      orderStatus: formValue.orderStatus || null,
      paymentStatus: formValue.paymentStatus || null,
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

  resetFilters(): void {
    this.filterForm.reset({
      searchTerm: '',
      orderStatus: '',
      paymentStatus: '',
      fromDate: '', // Thêm reset cho control này
      toDate: ''    // Thêm reset cho control này
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadOrders();
  }

  // Các hàm onStatusChange và onPaymentStatusChange giữ nguyên, không thay đổi
  onStatusChange(order: OrderDto, event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const newStatus = selectElement.value;

    if (!newStatus || newStatus === order.orderStatus) return;
    this.updatingStatusForOrderId.set(order.orderId);

    this.orderService.updateOrderStatus(order.orderId, newStatus).subscribe({
      next: () => {
        this.notifier.showSuccess(`Đã cập nhật trạng thái đơn hàng #${order.orderId}.`);
        this.orders.update(currentOrders => 
          currentOrders.map(o => o.orderId === order.orderId ? { ...o, orderStatus: newStatus } : o)
        );
        this.updatingStatusForOrderId.set(null);
      },
      error: (err) => {
        this.notifier.showError(err.error?.message || 'Cập nhật trạng thái thất bại.');
        selectElement.value = order.orderStatus;
        this.updatingStatusForOrderId.set(null);
      }
    });
  }

  onPaymentStatusChange(order: OrderDto, event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const newStatus = selectElement.value;

    if (!newStatus || newStatus === order.paymentStatus) return;
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