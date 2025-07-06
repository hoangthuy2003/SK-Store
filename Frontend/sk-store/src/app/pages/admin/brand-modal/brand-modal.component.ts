import { Component, OnInit, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { BrandDto, CreateBrandDto, UpdateBrandDto } from '../../../models/brand.model';
import { BrandService } from '../../../services/brand.service';
import { NotificationService } from '../../../services/notification.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-brand-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './brand-modal.component.html',
  styleUrls: ['./brand-modal.component.css']
})
export class BrandModalComponent implements OnInit {
  @Input() brand: BrandDto | null = null;
  @Output() close = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private brandService = inject(BrandService);
  private notifier = inject(NotificationService);

  brandForm: FormGroup;
  isEditMode = false;
  isSaving = signal(false);

  constructor() {
    this.brandForm = this.fb.group({
      brandName: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)]
    });
  }

  ngOnInit(): void {
    this.isEditMode = !!this.brand;
    if (this.isEditMode) {
      this.brandForm.patchValue(this.brand!);
    }
  }

  onSubmit(): void {
    if (this.brandForm.invalid) return;
    this.isSaving.set(true);

    const formData = this.brandForm.value;
    let operation: Observable<BrandDto | void>;

    if (this.isEditMode) {
      operation = this.brandService.updateBrand(this.brand!.brandId, formData);
    } else {
      operation = this.brandService.createBrand(formData);
    }

    operation.subscribe({
      next: () => {
        this.notifier.showSuccess(`Đã ${this.isEditMode ? 'cập nhật' : 'tạo'} thương hiệu thành công!`);
        this.close.emit(true); // Gửi tín hiệu cần reload
      },
      error: (err: Error) => {
        this.notifier.showError(err.message);
        this.isSaving.set(false);
      }
    });
  }

  closeModal(): void {
    this.close.emit(false);
  }
}