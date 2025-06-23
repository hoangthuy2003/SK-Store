export interface User {
    userId: number;
    email: string;
    firstName: string;
    lastName: string;
    fullName: string;
    phoneNumber: string;
    gender: string;
    dateOfBirth: Date;
    isActive: boolean;
    registrationDate: Date;
    lastLoginDate?: Date;
    isVerified: boolean;
    roleId: number;
    roleName: string;
}
export interface UpdateUserProfileDto {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  gender: string;
  dateOfBirth: string; // Sử dụng string để dễ dàng bind với form input type="date"
}

export interface ChangePasswordDto {
  oldPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}
export interface UserPayload {
  sub: string; // User ID
  email: string;
  firstname: string;
  lastname: string;
  role: 'Admin' | 'User'; // Sử dụng union type để có kiểu dữ liệu chặt chẽ
  nbf: number; // Not Before
  exp: number; // Expiration Time
  iat: number; // Issued At
  iss: string; // Issuer
  aud: string; // Audience
}
export interface UserProfile {
    userId: number;
    email: string;
    firstName: string;
    lastName: string;
    fullName: string;
    phoneNumber: string;
    gender: string;
    dateOfBirth: Date;
    registrationDate: Date;
    lastLoginDate?: Date;
    isVerified: boolean;
    role: 'User' | 'Admin';
    isEmailVerified: boolean;
    createdAt: Date;
    updatedAt: Date;
}

export interface UpdateUserProfile {
    firstName: string;
    lastName: string;
    phoneNumber: string;
    gender: string;
    dateOfBirth: Date;
}

export interface ChangePassword {
    oldPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

export interface UserFilterParameters {
    searchTerm?: string;
    roleId?: number;
    isActive?: boolean;
    pageNumber: number;
    pageSize: number;
    sortBy: string;
    sortDirection: string;
}

export interface LoginRequest {
    username: string;
    password: string;
}

export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
}

export interface AuthResponse {
    token: string;
    user: User;
} 