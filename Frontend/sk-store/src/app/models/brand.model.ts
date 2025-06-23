export interface BrandDto {
  brandId: number;
  brandName: string;
  description?: string;
}

export interface CreateBrandDto {
  brandName: string;
  description?: string;
}

export interface UpdateBrandDto {
  brandName?: string;
  description?: string;
}