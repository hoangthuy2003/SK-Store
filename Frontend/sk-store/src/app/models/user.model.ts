// DTO cho danh sách người dùng trong trang Admin
export interface UserDto {
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phoneNumber: string;
  gender: string;
  dateOfBirth: string;
  isActive: boolean;
  registrationDate: string;
  lastLoginDate?: string;
  isVerified: boolean;
  roleId: number;
  roleName: string;
}

// DTO để cập nhật người dùng từ trang Admin
export interface UserForAdminUpdateDto {
  isActive: boolean;
  roleId: number;
}

// DTO cho tham số lọc người dùng
export interface UserFilterParameters {
  searchTerm?: string | null;
  roleId?: number | null;
  isActive?: boolean | null;
  pageNumber: number;
  pageSize: number;
  sortBy?: string; // sortBy và sortDirection là optional
  sortDirection?: 'asc' | 'desc';
}

// DTO cho Role (để hiển thị trong dropdown)
export interface RoleDto {
  roleId: number;
  roleName: string;
}

// DTO cho thông tin hồ sơ người dùng (trang Profile)
export interface UserProfile {
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phoneNumber: string;
  gender: string;
  dateOfBirth: string; // Dùng string để dễ dàng bind với form
  registrationDate: string;
  lastLoginDate?: string;
  isVerified: boolean;
}

// DTO để cập nhật hồ sơ người dùng
export interface UpdateUserProfileDto {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  gender: string;
  dateOfBirth: string;
}

// DTO để thay đổi mật khẩu
export interface ChangePasswordDto {
  oldPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

// Payload được giải mã từ JWT token
export interface UserPayload {
  sub: string; // User ID
  email: string;
  firstname: string;
  lastname: string;
  role: 'Admin' | 'User';
  nbf: number;
  exp: number;
  iat: number;
  iss: string;
  aud: string;
}