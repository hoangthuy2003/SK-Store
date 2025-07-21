import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { UserAddressService, UserAddressDto, CreateUserAddressDto, UpdateUserAddressDto } from '../../services/user-address.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-address-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="container mx-auto px-4 py-8 max-w-4xl">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold text-gray-800">Quản lý địa chỉ</h1>
        <button 
          (click)="showAddForm()"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
          <i class="fas fa-plus mr-2"></i>Thêm địa chỉ mới
        </button>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading()" class="text-center py-8">
        <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <p class="mt-2 text-gray-600">Đang tải...</p>
      </div>

      <!-- Address List -->
      <div *ngIf="!loading() && addresses().length > 0" class="space-y-4">
        <div 
          *ngFor="let address of addresses()"
          class="bg-white border rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow">
          
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-2">
                <span *ngIf="address.isDefault" 
                      class="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full font-medium">
                  Mặc định
                </span>
                <span *ngIf="address.recipientName" class="font-medium text-gray-800">
                  {{ address.recipientName }}
                </span>
              </div>
              
              <p class="text-gray-700 mb-2">{{ address.addressLine }}</p>
              
              <p *ngIf="address.recipientPhoneNumber" class="text-sm text-gray-600">
                <i class="fas fa-phone mr-2"></i>{{ address.recipientPhoneNumber }}
              </p>
            </div>

            <div class="flex gap-2 ml-4">
              <button 
                (click)="editAddress(address)"
                class="text-blue-600 hover:text-blue-800 p-2 rounded transition-colors"
                title="Chỉnh sửa">
                <i class="fas fa-edit"></i>
              </button>
              
              <button 
                *ngIf="!address.isDefault"
                (click)="setDefault(address.addressId)"
                class="text-green-600 hover:text-green-800 p-2 rounded transition-colors"
                title="Đặt làm mặc định">
                <i class="fas fa-star"></i>
              </button>
              
              <button 
                (click)="deleteAddress(address.addressId)"
                class="text-red-600 hover:text-red-800 p-2 rounded transition-colors"
                title="Xóa">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading() && addresses().length === 0" 
           class="text-center py-12 bg-gray-50 rounded-lg">
        <i class="fas fa-map-marker-alt text-4xl text-gray-400 mb-4"></i>
        <h3 class="text-lg font-medium text-gray-700 mb-2">Chưa có địa chỉ nào</h3>
        <p class="text-gray-500 mb-4">Thêm địa chỉ đầu tiên để bắt đầu mua sắm</p>
        <button 
          (click)="showAddForm()"
          class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors">
          Thêm địa chỉ
        </button>
      </div>

      <!-- Add/Edit Form Modal -->
      <div *ngIf="showForm()" 
           class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-lg w-full max-w-md max-h-[90vh] overflow-y-auto">
          <div class="p-6">
            <div class="flex justify-between items-center mb-4">
              <h2 class="text-xl font-bold">
                {{ isEditing() ? 'Chỉnh sửa địa chỉ' : 'Thêm địa chỉ mới' }}
              </h2>
              <button 
                (click)="hideForm()"
                class="text-gray-400 hover:text-gray-600 text-xl">
                <i class="fas fa-times"></i>
              </button>
            </div>

            <form [formGroup]="addressForm" (ngSubmit)="onSubmit()">
              <div class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Địa chỉ <span class="text-red-500">*</span>
                  </label>
                  <textarea 
                    formControlName="addressLine"
                    rows="3"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Nhập địa chỉ chi tiết..."></textarea>
                  <div *ngIf="addressForm.get('addressLine')?.errors && addressForm.get('addressLine')?.touched" 
                       class="text-red-500 text-sm mt-1">
                    Địa chỉ không được để trống
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Tên người nhận
                  </label>
                  <input 
                    type="text"
                    formControlName="recipientName"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Nhập tên người nhận...">
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Số điện thoại
                  </label>
                  <input 
                    type="tel"
                    formControlName="recipientPhoneNumber"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Nhập số điện thoại...">
                </div>

                <div class="flex items-center">
                  <input 
                    type="checkbox"
                    id="isDefault"
                    formControlName="isDefault"
                    class="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500">
                  <label for="isDefault" class="ml-2 text-sm text-gray-700">
                    Đặt làm địa chỉ mặc định
                  </label>
                </div>
              </div>

              <div class="flex gap-3 mt-6">
                <button 
                  type="button"
                  (click)="hideForm()"
                  class="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50 transition-colors">
                  Hủy
                </button>
                <button 
                  type="submit"
                  [disabled]="!addressForm.valid || submitting()"
                  class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-400 transition-colors">
                  <span *ngIf="submitting()">
                    <i class="fas fa-spinner fa-spin mr-2"></i>Đang xử lý...
                  </span>
                  <span *ngIf="!submitting()">
                    {{ isEditing() ? 'Cập nhật' : 'Thêm địa chỉ' }}
                  </span>
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  `
})
export class AddressManagementComponent implements OnInit, OnDestroy {
  private userAddressService = inject(UserAddressService);
  private notificationService = inject(NotificationService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  addresses = signal<UserAddressDto[]>([]);
  loading = signal(false);
  showForm = signal(false);
  submitting = signal(false);
  isEditing = signal(false);
  editingId = signal<number | null>(null);

  addressForm: FormGroup;

  constructor() {
    this.addressForm = this.fb.group({
      addressLine: ['', [Validators.required, Validators.maxLength(500)]],
      recipientName: ['', [Validators.maxLength(100)]],
      recipientPhoneNumber: ['', [Validators.maxLength(20)]],
      isDefault: [false]
    });
  }

  ngOnInit() {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }
    
    this.loadAddresses();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAddresses() {
    this.loading.set(true);
    this.userAddressService.getUserAddresses()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (addresses) => {
          this.addresses.set(addresses);
          this.loading.set(false);
        },
        error: (error) => {
          console.error('Error loading addresses:', error);
          this.notificationService.showError('Có lỗi xảy ra khi tải danh sách địa chỉ');
          this.loading.set(false);
        }
      });
  }

  showAddForm() {
    this.isEditing.set(false);
    this.editingId.set(null);
    this.addressForm.reset({ isDefault: false });
    this.showForm.set(true);
  }

  editAddress(address: UserAddressDto) {
    this.isEditing.set(true);
    this.editingId.set(address.addressId);
    this.addressForm.patchValue({
      addressLine: address.addressLine,
      recipientName: address.recipientName || '',
      recipientPhoneNumber: address.recipientPhoneNumber || '',
      isDefault: address.isDefault
    });
    this.showForm.set(true);
  }

  hideForm() {
    this.showForm.set(false);
    this.addressForm.reset();
  }

  onSubmit() {
    if (!this.addressForm.valid) return;

    this.submitting.set(true);
    const formData = this.addressForm.value;

    const addressData = {
      addressLine: formData.addressLine.trim(),
      recipientName: formData.recipientName?.trim() || undefined,
      recipientPhoneNumber: formData.recipientPhoneNumber?.trim() || undefined,
      isDefault: formData.isDefault || false
    };

    if (this.isEditing()) {
      this.updateAddress(addressData);
    } else {
      this.addAddress(addressData);
    }
  }

  private addAddress(addressData: CreateUserAddressDto) {
    this.userAddressService.addAddress(addressData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Thêm địa chỉ thành công');
          this.hideForm();
          this.submitting.set(false);
          this.loadAddresses();
        },
        error: (error) => {
          console.error('Error adding address:', error);
          this.notificationService.showError('Có lỗi xảy ra khi thêm địa chỉ');
          this.submitting.set(false);
        }
      });
  }

  private updateAddress(addressData: UpdateUserAddressDto) {
    const addressId = this.editingId();
    if (!addressId) return;

    this.userAddressService.updateAddress(addressId, addressData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Cập nhật địa chỉ thành công');
          this.hideForm();
          this.submitting.set(false);
          this.loadAddresses();
        },
        error: (error) => {
          console.error('Error updating address:', error);
          this.notificationService.showError('Có lỗi xảy ra khi cập nhật địa chỉ');
          this.submitting.set(false);
        }
      });
  }

  setDefault(addressId: number) {
    this.userAddressService.setDefaultAddress(addressId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Đã đặt địa chỉ mặc định');
          this.loadAddresses();
        },
        error: (error) => {
          console.error('Error setting default address:', error);
          this.notificationService.showError('Có lỗi xảy ra khi đặt địa chỉ mặc định');
        }
      });
  }

  deleteAddress(addressId: number) {
    if (!confirm('Bạn có chắc chắn muốn xóa địa chỉ này?')) {
      return;
    }

    this.userAddressService.deleteAddress(addressId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Xóa địa chỉ thành công');
          this.loadAddresses();
        },
        error: (error) => {
          console.error('Error deleting address:', error);
          this.notificationService.showError('Có lỗi xảy ra khi xóa địa chỉ');
        }
      });
  }
}
