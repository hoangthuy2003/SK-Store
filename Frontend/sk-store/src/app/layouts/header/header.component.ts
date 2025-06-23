import { Component, OnInit, signal, inject } from '@angular/core'; // Thêm inject
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service'; // <<< THÊM IMPORT
import { UserProfile } from '../../models/user.model';
import { UserPayload } from '../../models/user.model';
@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  private authService = inject(AuthService);
  private cartService = inject(CartService);

  // THAY ĐỔI: Sử dụng UserPayload
  currentUser = signal<UserPayload | null>(null);
  isMenuOpen = signal(false);
  isUserMenuOpen = signal(false);
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