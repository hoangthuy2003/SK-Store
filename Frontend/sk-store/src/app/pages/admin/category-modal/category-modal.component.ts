import { Component, OnInit, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CategoryDto, CreateCategoryDto, UpdateCategoryDto } from '../../../models/category.model';
import { CategoryService } from '../../../services/category.service';
import { NotificationService } from '../../../services/notification.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-category-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-modal.component.html',
  styleUrls: ['./category-modal.component.css']
})
export class CategoryModalComponent implements OnInit {
  // Nhận dữ liệu category để edit, hoặc null để create
  @Input() category: CategoryDto | null = null;
  // Gửi sự kiện đóng modal, kèm theo tín hiệu có cần reload list hay không
  @Output() close = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);
  private notifier = inject(NotificationService);

  categoryForm: FormGroup;
  isEditMode = false;
  isSaving = signal(false);

  constructor() {
    this.categoryForm = this.fb.group({
      categoryName: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)]
    });
  }

  ngOnInit(): void {
    this.isEditMode = !!this.category;
    if (this.isEditMode) {
      this.categoryForm.patchValue(this.category!);
    }
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) return;
    this.isSaving.set(true);

    const formData = this.categoryForm.value;
    let operation: Observable<CategoryDto | void>;

    if (this.isEditMode) {
      operation = this.categoryService.updateCategory(this.category!.categoryId, formData);
    } else {
      operation = this.categoryService.createCategory(formData);
    }

    operation.subscribe({
      next: () => {
        this.notifier.showSuccess(`Đã ${this.isEditMode ? 'cập nhật' : 'tạo'} danh mục thành công!`);
        this.close.emit(true); // Gửi tín hiệu cần reload
      },
      error: (err: Error) => {
        this.notifier.showError(err.message);
        this.isSaving.set(false);
      }
    });
  }

  // Hàm để đóng modal khi click ra ngoài hoặc nút hủy
  closeModal(): void {
    this.close.emit(false); // Không cần reload
  }
}