export interface AuthResponseDto {
  isSuccess: boolean;
  message: string;
  token?: string;
  tokenExpires?: Date;
  // Các trường khác nếu API trả về (ví dụ: thông tin cơ bản người dùng)
}
