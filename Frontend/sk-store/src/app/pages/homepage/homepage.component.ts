import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { ProductDto } from '../../models/product.model';
import { VndCurrencyPipe } from '../../pipes/vnd-currency.pipe';
@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css'],
  standalone: true,
  imports: [
    CommonModule, // Thay thế NgIf, NgFor, NgClass
    RouterLink,
    VndCurrencyPipe 
  ]
})
export class HomepageComponent implements OnInit {
  private productService = inject(ProductService);

  // State signals
  featuredProducts = signal<ProductDto[]>([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  constructor() {}

  ngOnInit(): void {
    this.loadFeaturedProducts();
  }

  loadFeaturedProducts(): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.productService.getFeaturedProducts(4).subscribe({
      next: (data) => {
        this.featuredProducts.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Không thể tải sản phẩm nổi bật. Vui lòng thử lại sau.');
        console.error(err);
        this.isLoading.set(false);
      }
    });
  }

  
}