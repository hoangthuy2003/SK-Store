import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderDto } from '../../models/order.model';
import { Observable } from 'rxjs';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';
import { UtilsService } from '../../services/utils.service';
@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterLink, VndCurrencyPipe],
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.css']
})
export class OrderListComponent implements OnInit {
  private orderService = inject(OrderService);
  
  // Sử dụng observable để lấy danh sách đơn hàng
  orders$!: Observable<OrderDto[]>;

  ngOnInit(): void {
    this.orders$ = this.orderService.getMyOrders();
  }

   public utilsService = inject(UtilsService);

  
}