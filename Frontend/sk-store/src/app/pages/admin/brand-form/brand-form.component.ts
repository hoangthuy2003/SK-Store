import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

// Services
import { BrandService } from '../../../services/brand.service';
import { NotificationService } from '../../../services/notification.service';
import { Observable } from 'rxjs';
import { BrandDto } from '../../../models/brand.model';

@Component({
  selector: 'app-admin-brand-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './brand-form.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminBrandFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private brandService = inject(BrandService);
  private notifier = inject(NotificationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private location = inject(Location);

  brandForm: FormGroup;
  isEditMode = false;
  currentId: number | null = null;

  isLoading = signal(true);
  isSaving = signal(false);

  constructor() {
    this.brandForm = this.fb.group({
      brandName: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.currentId = +id;
      this.brandService.getBrandById(this.currentId).subscribe({
        next: (data) => {
          this.brandForm.patchValue(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.notifier.showError("Không tìm thấy thương hiệu.");
          this.router.navigate(['/admin/brands']);
        }
      });
    } else {
      this.isLoading.set(false);
    }
  }

  onSubmit(): void {
    if (this.brandForm.invalid) return;
    this.isSaving.set(true);

    const formData = this.brandForm.value;

    // <<< KHAI BÁO RÕ RÀNG KIỂU DỮ LIỆU CHO operation >>>
    let operation: Observable<BrandDto | void>;

    if (this.isEditMode) {
      operation = this.brandService.updateBrand(this.currentId!, formData);
    } else {
      operation = this.brandService.createBrand(formData);
    }

    operation.subscribe({
      next: () => {
        this.notifier.showSuccess(`Đã ${this.isEditMode ? 'cập nhật' : 'tạo'} thương hiệu thành công!`);
        this.router.navigate(['/admin/brands']);
      },
      error: (err: Error) => { // <<< THÊM KIỂU DỮ LIỆU CHO err >>>
        this.notifier.showError(err.message);
        this.isSaving.set(false);
      }
    });
  }

  goBack(): void {
    this.location.back();
  }
}