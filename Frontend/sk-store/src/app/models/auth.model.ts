export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    gender: string;
    dateOfBirth: Date;
}

export interface AuthResponse {
    isSuccess: boolean;
    message: string;
    token?: string;
    tokenExpires?: Date;
}

export interface AuthValidation {
    [key: string]: string | undefined;
    email?: string;
    password?: string;
    firstName?: string;
    lastName?: string;
    phoneNumber?: string;
    gender?: string;
    dateOfBirth?: string;
    confirmPassword?: string;
}

export interface ForgotPasswordRequest {
    email: string;
}

export interface ResetPasswordRequest {
    email: string;
    otp: string;
    newPassword: string;
}

export interface VerifyEmailRequest {
    otp: string;
}

export class AuthValidation {
    static email(email: string): { [key: string]: boolean } | null {
        if (!email) {
            return { required: true };
        }
        const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return emailRegex.test(email) ? null : { email: true };
    }

    static password(password: string): { [key: string]: boolean } | null {
        if (!password) {
            return { required: true };
        }
        if (password.length < 6) {
            return { minlength: true };
        }
        return null;
    }

    static phoneNumber(phoneNumber: string): { [key: string]: boolean } | null {
        if (!phoneNumber) {
            return { required: true };
        }
        const phoneRegex = /^0\d{9}$/;
        return phoneRegex.test(phoneNumber) ? null : { phoneNumber: true };
    }

    static name(name: string): { [key: string]: boolean } | null {
        if (!name) {
            return { required: true };
        }
        if (name.length < 2 || name.length > 100) {
            return { length: true };
        }
        const nameRegex = /^[a-zA-ZÀ-ỹ\s]+$/;
        return nameRegex.test(name) ? null : { name: true };
    }

    static gender(gender: string): { [key: string]: boolean } | null {
        if (!gender) {
            return { required: true };
        }
        return null;
    }

    static dateOfBirth(dateOfBirth: Date): { [key: string]: boolean } | null {
        if (!dateOfBirth) {
            return { required: true };
        }
        const today = new Date();
        const age = today.getFullYear() - dateOfBirth.getFullYear();
        if (age < 18 || age > 100) {
            return { age: true };
        }
        return null;
    }

    static getErrorMessage(controlName: string, error: any): string {
        switch (controlName) {
            case 'email':
                if (error.required) return 'Email không được để trống';
                if (error.email) return 'Email không đúng định dạng';
                break;
            case 'password':
                if (error.required) return 'Mật khẩu không được để trống';
                if (error.minlength) return 'Mật khẩu phải có ít nhất 6 ký tự';
                break;
            case 'phoneNumber':
                if (error.required) return 'Số điện thoại không được để trống';
                if (error.phoneNumber) return 'Số điện thoại phải có đúng 10 ký tự và bắt đầu bằng số 0';
                break;
            case 'firstName':
            case 'lastName':
                if (error.required) return 'Họ tên không được để trống';
                if (error.length) return 'Họ tên phải từ 2 đến 100 ký tự';
                if (error.name) return 'Họ tên chỉ được chứa chữ cái và dấu cách';
                break;
            case 'gender':
                if (error.required) return 'Giới tính không được để trống';
                break;
            case 'dateOfBirth':
                if (error.required) return 'Ngày sinh không được để trống';
                if (error.age) return 'Tuổi phải từ 18 đến 100';
                break;
        }
        return 'Giá trị không hợp lệ';
    }
} 