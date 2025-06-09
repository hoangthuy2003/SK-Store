import { Component, OnInit, signal, inject } from '@angular/core'; // Thêm inject
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service'; // <<< THÊM IMPORT
import { UserProfile } from '../../models/user.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  // Sửa lại cách inject service cho gọn hơn
  private authService = inject(AuthService);
  private cartService = inject(CartService); // <<< INJECT CART SERVICE

  currentUser = signal<UserProfile | null>(null);
  isMenuOpen = signal(false);
  isUserMenuOpen = signal(false);
  
  // Signal cartItemCount sẽ được cập nhật tự động từ service
  cartItemCount = signal(0);

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser.set(user);
    });

    // <<< KẾT NỐI VỚI CART SERVICE >>>
    this.cartService.cartItemCount$.subscribe(count => {
      this.cartItemCount.set(count);
    });
  }

  toggleMenu(): void {
    this.isMenuOpen.update(value => !value);
    if (this.isMenuOpen()) {
      this.isUserMenuOpen.set(false);
    }
  }

  toggleUserMenu(): void {
    this.isUserMenuOpen.update(value => !value);
  }

  closeMenu(): void {
    this.isMenuOpen.set(false);
  }

  logout(): void {
    this.authService.logout();
    this.isUserMenuOpen.set(false);
    this.isMenuOpen.set(false);
  }
}