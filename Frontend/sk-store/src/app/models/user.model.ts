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