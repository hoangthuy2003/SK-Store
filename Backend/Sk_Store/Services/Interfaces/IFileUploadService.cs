using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Upload m?t file ?nh và tr? v? URL c?a file ?ã upload
        /// </summary>
        /// <param name="file">File ?nh c?n upload</param>
        /// <param name="subfolder">Th? m?c con ?? l?u file (ví d?: "products")</param>
        /// <returns>URL c?a file ?ã upload</returns>
        Task<string> UploadImageAsync(IFormFile file, string subfolder = "products");

        /// <summary>
        /// Upload nhi?u file ?nh cùng lúc
        /// </summary>
        /// <param name="files">Danh sách file ?nh c?n upload</param>
        /// <param name="subfolder">Th? m?c con ?? l?u file</param>
        /// <returns>Danh sách URL c?a các file ?ã upload</returns>
        Task<List<string>> UploadMultipleImagesAsync(IFormFile[] files, string subfolder = "products");

        /// <summary>
        /// Xóa file ?nh d?a trên URL
        /// </summary>
        /// <param name="imageUrl">URL c?a file c?n xóa</param>
        /// <returns>True n?u xóa thành công</returns>
        Task<bool> DeleteImageAsync(string imageUrl);

        /// <summary>
        /// Ki?m tra xem file có ph?i là ?nh h?p l? không
        /// </summary>
        /// <param name="file">File c?n ki?m tra</param>
        /// <returns>True n?u file là ?nh h?p l?</returns>
        bool IsValidImage(IFormFile file);
    }
}