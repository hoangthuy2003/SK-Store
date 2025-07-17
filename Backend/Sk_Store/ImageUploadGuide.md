# Product Image Upload Feature

## T?ng quan
T�nh n?ng n�y cho ph�p upload ?nh s?n ph?m t? file local (PNG, JPG) v?i k�ch th??c t?i ?a 5MB m?i file.

## C?u tr�c th? m?c
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
- `Authorization: Bearer {token}` (y�u c?u quy?n Admin)
- `Content-Type: multipart/form-data`

**Form Data:**
- `ProductName` (string, required): T�n s?n ph?m
- `Description` (string, optional): M� t? s?n ph?m
- `Price` (decimal, required): Gi� s?n ph?m
- `StockQuantity` (int, required): S? l??ng t?n kho
- `CategoryId` (int, required): ID danh m?c
- `BrandId` (int, required): ID th??ng hi?u
- `PrimaryImageIndex` (int, optional, default=0): Ch? s? ?nh ch�nh
- `ImageFiles` (file[], optional): Danh s�ch file ?nh

**Example Response:**
```json
{
  "productId": 1,
  "productName": "B�t bi Thi�n Long",
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
- `Authorization: Bearer {token}` (y�u c?u quy?n Admin)
- `Content-Type: multipart/form-data`

**Form Data:**
- `ProductName` (string, optional): T�n s?n ph?m m?i
- `Description` (string, optional): M� t? m?i
- `Price` (decimal, optional): Gi� m?i
- `StockQuantity` (int, optional): S? l??ng m?i
- `CategoryId` (int, optional): ID danh m?c m?i
- `BrandId` (int, optional): ID th??ng hi?u m?i
- `ReplaceAllImages` (bool, default=false): C� thay th? t?t c? ?nh c? kh�ng
- `PrimaryImageIndex` (int, optional): Ch? s? ?nh ch�nh trong danh s�ch ?nh m?i
- `ImageFiles` (file[], optional): Danh s�ch file ?nh m?i

## R�ng bu?c file
- **??nh d?ng cho ph�p:** .jpg, .jpeg, .png
- **K�ch th??c t?i ?a:** 5MB m?i file
- **MIME types h? tr?:** image/jpeg, image/jpg, image/png

## C�ch s? d?ng

### 1. Test b?ng HTML file
1. M? `http://localhost:5000/test-upload.html` trong tr�nh duy?t
2. ?i?n th�ng tin s?n ph?m
3. Ch?n ?nh t? m�y t�nh
4. Nh?p JWT token (l?y t? login API)
5. Click "T?o s?n ph?m"

### 2. Test b?ng Postman
1. T?o request POST t?i `http://localhost:5000/api/products/upload`
2. Ch?n Body > form-data
3. Th�m c�c field theo form data tr�n
4. V?i `ImageFiles`, ch?n type l� File v� upload ?nh
5. Th�m header Authorization v?i Bearer token

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

## Xem ?nh ?� upload
Sau khi upload th�nh c�ng, ?nh c� th? ???c truy c?p qua URL:
`http://localhost:5000/images/products/{filename}`

V� d?: `http://localhost:5000/images/products/550e8400-e29b-41d4-a716-446655440000.jpg`

## L?u � k? thu?t

### Services ?� th�m
- `IFileUploadService`: Interface cho upload file
- `FileUploadService`: Implementation x? l� upload, validation, x�a file

### DTOs m?i
- `CreateProductWithFilesDto`: DTO cho t?o s?n ph?m v?i file
- `UpdateProductWithFilesDto`: DTO cho c?p nh?t s?n ph?m v?i file

### Endpoints m?i
- `POST /api/products/upload`: T?o s?n ph?m v?i file upload
- `PUT /api/products/{id}/upload`: C?p nh?t s?n ph?m v?i file upload

### C?u h�nh
- ?� th�m `app.UseStaticFiles()` trong Program.cs
- ?� ??ng k� `IFileUploadService` trong DI container
- T?o th? m?c `wwwroot/images/products` ?? l?u ?nh

## Troubleshooting

### L?i 413 Payload Too Large
N?u g?p l?i n�y, c?n c?u h�nh t?ng gi?i h?n file size trong Program.cs:
```csharp
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});
```

### L?i kh�ng t�m th?y ?nh
- Ki?m tra th? m?c `wwwroot/images/products` ?� ???c t?o
- Ki?m tra quy?n ghi file c?a ?ng d?ng
- ??m b?o `app.UseStaticFiles()` ???c g?i trong Program.cs

### L?i JWT token
- ??m b?o token ???c l?y t? API login
- Token ph?i c� role "Admin" ?? t?o/s?a s?n ph?m
- Ki?m tra token ch?a h?t h?n