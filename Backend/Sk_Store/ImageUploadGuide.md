# Product Image Upload Feature

## T?ng quan
Tính n?ng này cho phép upload ?nh s?n ph?m t? file local (PNG, JPG) v?i kích th??c t?i ?a 5MB m?i file.

## C?u trúc th? m?c
```
Sk_Store/
??? wwwroot/
?   ??? images/
?   ?   ??? products/        # Th? m?c ch?a ?nh s?n ph?m
?   ??? test-upload.html     # File test upload
??? ...
```

## API Endpoints

### 1. T?o s?n ph?m v?i file upload
**POST** `/api/products/upload`

**Headers:**
- `Authorization: Bearer {token}` (yêu c?u quy?n Admin)
- `Content-Type: multipart/form-data`

**Form Data:**
- `ProductName` (string, required): Tên s?n ph?m
- `Description` (string, optional): Mô t? s?n ph?m
- `Price` (decimal, required): Giá s?n ph?m
- `StockQuantity` (int, required): S? l??ng t?n kho
- `CategoryId` (int, required): ID danh m?c
- `BrandId` (int, required): ID th??ng hi?u
- `PrimaryImageIndex` (int, optional, default=0): Ch? s? ?nh chính
- `ImageFiles` (file[], optional): Danh sách file ?nh

**Example Response:**
```json
{
  "productId": 1,
  "productName": "Bút bi Thiên Long",
  "price": 5000,
  "productImages": [
    {
      "imageId": 1,
      "imageUrl": "/images/products/550e8400-e29b-41d4-a716-446655440000.jpg",
      "isPrimary": true
    }
  ]
}
```

### 2. C?p nh?t s?n ph?m v?i file upload
**PUT** `/api/products/{id}/upload`

**Headers:**
- `Authorization: Bearer {token}` (yêu c?u quy?n Admin)
- `Content-Type: multipart/form-data`

**Form Data:**
- `ProductName` (string, optional): Tên s?n ph?m m?i
- `Description` (string, optional): Mô t? m?i
- `Price` (decimal, optional): Giá m?i
- `StockQuantity` (int, optional): S? l??ng m?i
- `CategoryId` (int, optional): ID danh m?c m?i
- `BrandId` (int, optional): ID th??ng hi?u m?i
- `ReplaceAllImages` (bool, default=false): Có thay th? t?t c? ?nh c? không
- `PrimaryImageIndex` (int, optional): Ch? s? ?nh chính trong danh sách ?nh m?i
- `ImageFiles` (file[], optional): Danh sách file ?nh m?i

## Ràng bu?c file
- **??nh d?ng cho phép:** .jpg, .jpeg, .png
- **Kích th??c t?i ?a:** 5MB m?i file
- **MIME types h? tr?:** image/jpeg, image/jpg, image/png

## Cách s? d?ng

### 1. Test b?ng HTML file
1. M? `http://localhost:5000/test-upload.html` trong trình duy?t
2. ?i?n thông tin s?n ph?m
3. Ch?n ?nh t? máy tính
4. Nh?p JWT token (l?y t? login API)
5. Click "T?o s?n ph?m"

### 2. Test b?ng Postman
1. T?o request POST t?i `http://localhost:5000/api/products/upload`
2. Ch?n Body > form-data
3. Thêm các field theo form data trên
4. V?i `ImageFiles`, ch?n type là File và upload ?nh
5. Thêm header Authorization v?i Bearer token

### 3. Test b?ng cURL
```bash
curl -X POST "http://localhost:5000/api/products/upload" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "ProductName=Test Product" \
  -F "Price=10000" \
  -F "StockQuantity=100" \
  -F "CategoryId=1" \
  -F "BrandId=1" \
  -F "ImageFiles=@path/to/image1.jpg" \
  -F "ImageFiles=@path/to/image2.png" \
  -F "PrimaryImageIndex=0"
```

## Xem ?nh ?ã upload
Sau khi upload thành công, ?nh có th? ???c truy c?p qua URL:
`http://localhost:5000/images/products/{filename}`

Ví d?: `http://localhost:5000/images/products/550e8400-e29b-41d4-a716-446655440000.jpg`

## L?u ý k? thu?t

### Services ?ã thêm
- `IFileUploadService`: Interface cho upload file
- `FileUploadService`: Implementation x? lý upload, validation, xóa file

### DTOs m?i
- `CreateProductWithFilesDto`: DTO cho t?o s?n ph?m v?i file
- `UpdateProductWithFilesDto`: DTO cho c?p nh?t s?n ph?m v?i file

### Endpoints m?i
- `POST /api/products/upload`: T?o s?n ph?m v?i file upload
- `PUT /api/products/{id}/upload`: C?p nh?t s?n ph?m v?i file upload

### C?u hình
- ?ã thêm `app.UseStaticFiles()` trong Program.cs
- ?ã ??ng ký `IFileUploadService` trong DI container
- T?o th? m?c `wwwroot/images/products` ?? l?u ?nh

## Troubleshooting

### L?i 413 Payload Too Large
N?u g?p l?i này, c?n c?u hình t?ng gi?i h?n file size trong Program.cs:
```csharp
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});
```

### L?i không tìm th?y ?nh
- Ki?m tra th? m?c `wwwroot/images/products` ?ã ???c t?o
- Ki?m tra quy?n ghi file c?a ?ng d?ng
- ??m b?o `app.UseStaticFiles()` ???c g?i trong Program.cs

### L?i JWT token
- ??m b?o token ???c l?y t? API login
- Token ph?i có role "Admin" ?? t?o/s?a s?n ph?m
- Ki?m tra token ch?a h?t h?n