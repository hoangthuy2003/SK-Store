export interface CategoryDto {
  categoryId: number;
  categoryName: string;
  description?: string;
}

// --- THÊM CÁC INTERFACE MỚI ---
export interface CreateCategoryDto {
  categoryName: string;
  description?: string;
}

export interface UpdateCategoryDto {
  categoryName?: string;
  description?: string;
}