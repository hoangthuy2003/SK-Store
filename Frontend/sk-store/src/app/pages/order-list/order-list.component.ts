import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderDto } from '../../models/order.model';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';
import { UtilsService } from '../../services/utils.service';
// <<< THÊM CÁC IMPORT CẦN THIẾT >>>
import { HttpResponse } from '@angular/common/http';
import { PaginationComponent } from '../../components/pagination/pagination.component';

@Component({
  selector: 'app-order-list',
  standalone: true,
  // <<< THÊM PaginationComponent VÀO IMPORTS >>>
  imports: [CommonModule, RouterLink, VndCurrencyPipe, PaginationComponent],
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.css']
})
export class OrderListComponent implements OnInit {
  private orderService = inject(OrderService);
  public utilsService = inject(UtilsService);

  // <<< SỬA LẠI TOÀN BỘ LOGIC STATE VÀ PHÂN TRANG >>>
  orders = signal<OrderDto[]>([]);
  isLoading = signal(true);
  
  currentPage = signal(1);
  pageSize = 10;
  totalItems = signal(0);

  ngOnInit(): void {
    this.loadMyOrders();
  }

  loadMyOrders(): void {
    this.isLoading.set(true);
    // Gọi service với tham số phân trang
    this.orderService.getMyOrders(this.currentPage(), this.pageSize).subscribe({
      next: (response: HttpResponse<OrderDto[]>) => {
        this.orders.set(response.body || []);
        const totalCount = response.headers.get('X-Total-Count');
        this.totalItems.set(totalCount ? +totalCount : 0);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error("Failed to load user orders", err);
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadMyOrders();
  }
}