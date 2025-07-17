using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces;

namespace Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string subfolder = "products")
        {
            if (!IsValidImage(file))
            {
                throw new ArgumentException("File không h?p l?. Ch? ch?p nh?n file ?nh JPG, JPEG, PNG v?i kích th??c t?i ?a 5MB.");
            }

            // T?o tên file unique ?? tránh trùng l?p
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLower()}";
            
            // T?o ???ng d?n th? m?c l?u tr?
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", subfolder);
            
            // T?o th? m?c n?u ch?a t?n t?i
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // ???ng d?n ??y ?? c?a file
            var filePath = Path.Combine(uploadsFolder, fileName);

            // L?u file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Tr? v? URL t??ng ??i
            return $"/images/{subfolder}/{fileName}";
        }

        public async Task<List<string>> UploadMultipleImagesAsync(IFormFile[] files, string subfolder = "products")
        {
            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var imageUrl = await UploadImageAsync(file, subfolder);
                    imageUrls.Add(imageUrl);
                }
            }

            return imageUrls;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return false;

                // Chuy?n URL thành ???ng d?n v?t lý
                var fileName = Path.GetFileName(imageUrl);
                var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Ki?m tra kích th??c file
            if (file.Length > MaxFileSize)
                return false;

            // Ki?m tra ph?n m? r?ng
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                return false;

            // Ki?m tra MIME type
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
                return false;

            return true;
        }
    }
}