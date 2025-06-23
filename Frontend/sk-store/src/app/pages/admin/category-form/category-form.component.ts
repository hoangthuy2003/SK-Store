import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryDto } from '../../../models/category.model'; 
// Services
import { CategoryService } from '../../../services/category.service';
import { NotificationService } from '../../../services/notification.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-admin-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-form.component.html',
  styleUrls: ['../admin-shared.css']
})
export class AdminCategoryFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);
  private notifier = inject(NotificationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private location = inject(Location);

  categoryForm: FormGroup;
  isEditMode = false;
  currentId: number | null = null;

  isLoading = signal(true);
  isSaving = signal(false);

  constructor() {
    this.categoryForm = this.fb.group({
      categoryName: ['', Validators.required],
      description: ['']
    });
  }

 ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.currentId = +id;
      this.categoryService.getCategoryById(this.currentId).subscribe({
        next: (data: CategoryDto) => {
          this.categoryForm.patchValue(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.notifier.showError("Không tìm thấy danh mục.");
          this.router.navigate(['/admin/categories']);
        }
      });
    } else {
      this.isLoading.set(false);
    }
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) return;
    this.isSaving.set(true);

    const formData = this.categoryForm.value;
    
    let operation: Observable<CategoryDto | void>;

    if (this.isEditMode) {
      operation = this.categoryService.updateCategory(this.currentId!, formData);
    } else {
      operation = this.categoryService.createCategory(formData);
    }

    operation.subscribe({
      next: () => {
        this.notifier.showSuccess(`Đã ${this.isEditMode ? 'cập nhật' : 'tạo'} danh mục thành công!`);
        this.router.navigate(['/admin/categories']);
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