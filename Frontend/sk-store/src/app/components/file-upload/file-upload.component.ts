import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface FileUploadResult {
  files: File[];
  errors: string[];
}

@Component({
  selector: 'app-file-upload',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="file-upload-container">
      <div class="upload-area" 
           [class.drag-over]="isDragOver"
           (dragover)="onDragOver($event)"
           (dragleave)="onDragLeave($event)"
           (drop)="onDrop($event)"
           (click)="fileInput.click()">
        
        <input #fileInput
               type="file"
               multiple
               accept="image/*,.jpg,.jpeg,.png"
               (change)="onFileSelect($event)"
               class="hidden">
               
        <div class="upload-content">
          <i class="fas fa-cloud-upload-alt text-4xl text-gray-400 mb-4"></i>
          <p class="text-lg font-medium text-gray-700">
            {{ isDragOver ? 'Thả ảnh vào đây' : 'Kéo thả ảnh hoặc click để chọn' }}
          </p>
          <p class="text-sm text-gray-500 mt-2">
            Hỗ trợ: JPG, JPEG, PNG • Tối đa 5MB/file
          </p>
        </div>
      </div>

      <!-- Preview ảnh đã chọn -->
      @if (selectedFiles.length > 0) {
        <div class="selected-files mt-4">
          <h4 class="text-sm font-medium text-gray-700 mb-2">
            Ảnh đã chọn ({{ selectedFiles.length }})
          </h4>
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            @for (file of selectedFiles; track file.name; let i = $index) {
              <div class="relative group">
                <img [src]="getFilePreview(file)" 
                     [alt]="file.name"
                     class="w-full h-24 object-cover rounded-lg border">
                
                <!-- Primary image indicator -->
                @if (i === primaryImageIndex) {
                  <div class="absolute top-1 left-1 bg-blue-500 text-white text-xs px-2 py-1 rounded">
                    Ảnh chính
                  </div>
                }
                
                <!-- Actions -->
                <div class="absolute inset-0 bg-black bg-opacity-50 opacity-0 group-hover:opacity-100 
                           transition-opacity rounded-lg flex items-center justify-center space-x-2">
                  @if (i !== primaryImageIndex) {
                    <button (click)="setPrimaryImage(i)"
                            class="bg-blue-500 hover:bg-blue-600 text-white p-1 rounded text-xs">
                      <i class="fas fa-star"></i>
                    </button>
                  }
                  <button (click)="removeFile(i)"
                          class="bg-red-500 hover:bg-red-600 text-white p-1 rounded text-xs">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
                
                <!-- File name -->
                <p class="text-xs text-gray-600 mt-1 truncate" [title]="file.name">
                  {{ file.name }}
                </p>
              </div>
            }
          </div>
        </div>
      }

      <!-- Error messages -->
      @if (errors.length > 0) {
        <div class="mt-4">
          @for (error of errors; track error) {
            <div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-2">
              {{ error }}
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .upload-area {
      border: 2px dashed #d1d5db;
      border-radius: 8px;
      padding: 3rem 2rem;
      text-align: center;
      cursor: pointer;
      transition: all 0.3s ease;
    }
    
    .upload-area:hover,
    .upload-area.drag-over {
      border-color: #3b82f6;
      background-color: #eff6ff;
    }
    
    .upload-content {
      pointer-events: none;
    }
    
    .hidden {
      display: none;
    }
  `]
})
export class FileUploadComponent {
  @Input() maxFiles: number = 10;
  @Input() maxFileSize: number = 5 * 1024 * 1024; // 5MB
  @Output() filesChange = new EventEmitter<FileUploadResult>();
  @Output() primaryImageChange = new EventEmitter<number>();

  selectedFiles: File[] = [];
  errors: string[] = [];
  isDragOver = false;
  primaryImageIndex = 0;

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
    
    const files = Array.from(event.dataTransfer?.files || []) as File[];
    this.handleFiles(files);
  }

  onFileSelect(event: any) {
    const files = Array.from(event.target.files || []) as File[];
    this.handleFiles(files);
  }

  private handleFiles(files: File[]) {
    this.errors = [];
    const validFiles: File[] = [];

    files.forEach(file => {
      if (this.validateFile(file)) {
        validFiles.push(file);
      }
    });

    // Kiểm tra giới hạn số lượng file
    const totalFiles = this.selectedFiles.length + validFiles.length;
    if (totalFiles > this.maxFiles) {
      this.errors.push(`Chỉ được chọn tối đa ${this.maxFiles} ảnh`);
      return;
    }

    // Thêm file mới vào danh sách
    this.selectedFiles = [...this.selectedFiles, ...validFiles];
    this.emitChanges();
  }

  private validateFile(file: File): boolean {
    // Kiểm tra kích thước
    if (file.size > this.maxFileSize) {
      this.errors.push(`File ${file.name} vượt quá kích thước cho phép (5MB)`);
      return false;
    }

    // Kiểm tra định dạng
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];
    if (!allowedTypes.includes(file.type.toLowerCase())) {
      this.errors.push(`File ${file.name} không đúng định dạng. Chỉ chấp nhận JPG, JPEG, PNG`);
      return false;
    }

    return true;
  }

  removeFile(index: number) {
    this.selectedFiles.splice(index, 1);
    
    // Điều chỉnh primary image index nếu cần
    if (this.primaryImageIndex >= this.selectedFiles.length) {
      this.primaryImageIndex = Math.max(0, this.selectedFiles.length - 1);
    }
    
    this.emitChanges();
  }

  setPrimaryImage(index: number) {
    this.primaryImageIndex = index;
    this.primaryImageChange.emit(index);
    this.emitChanges();
  }

  getFilePreview(file: File): string {
    return URL.createObjectURL(file);
  }

  private emitChanges() {
    this.filesChange.emit({
      files: this.selectedFiles,
      errors: this.errors
    });
  }

  // Method để reset component
  reset() {
    this.selectedFiles = [];
    this.errors = [];
    this.primaryImageIndex = 0;
    this.emitChanges();
  }
}
