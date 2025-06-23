import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderDto } from '../../models/order.model';
import { Observable, of } from 'rxjs';
import { switchMap, catchError, map } from 'rxjs/operators';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';
import { UtilsService } from '../../services/utils.service';
@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, VndCurrencyPipe],
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css']
})
export class OrderDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private orderService = inject(OrderService);
 public utilsService = inject(UtilsService);
  order$!: Observable<OrderDto | null>;
  showSuccessMessage = false;

  ngOnInit(): void {
    // Lấy thông báo thành công từ state nếu có (sau khi checkout)
    const navigationState = this.router.getCurrentNavigation()?.extras.state;
    if (navigationState && navigationState['success']) {
      this.showSuccessMessage = true;
    }

    this.order$ = this.route.paramMap.pipe(
      switchMap(params => {
        const id = params.get('id');
        if (id) {
          return this.orderService.getOrderById(+id).pipe(
            catchError(err => {
              console.error('Failed to load order details', err);
              return of(null); // Trả về null nếu có lỗi
            })
          );
        }
        return of(null); // Trả về null nếu không có ID
      })
    );
  }

 

  
}