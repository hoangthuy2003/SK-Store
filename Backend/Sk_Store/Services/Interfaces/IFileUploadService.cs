using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Upload m?t file ?nh v� tr? v? URL c?a file ?� upload
        /// </summary>
        /// <param name="file">File ?nh c?n upload</param>
        /// <param name="subfolder">Th? m?c con ?? l?u file (v� d?: "products")</param>
        /// <returns>URL c?a file ?� upload</returns>
        Task<string> UploadImageAsync(IFormFile file, string subfolder = "products");

        /// <summary>
        /// Upload nhi?u file ?nh c�ng l�c
        /// </summary>
        /// <param name="files">Danh s�ch file ?nh c?n upload</param>
        /// <param name="subfolder">Th? m?c con ?? l?u file</param>
        /// <returns>Danh s�ch URL c?a c�c file ?� upload</returns>
        Task<List<string>> UploadMultipleImagesAsync(IFormFile[] files, string subfolder = "products");

        /// <summary>
        /// X�a file ?nh d?a tr�n URL
        /// </summary>
        /// <param name="imageUrl">URL c?a file c?n x�a</param>
        /// <returns>True n?u x�a th�nh c�ng</returns>
        Task<bool> DeleteImageAsync(string imageUrl);

        /// <summary>
        /// Ki?m tra xem file c� ph?i l� ?nh h?p l? kh�ng
        /// </summary>
        /// <param name="file">File c?n ki?m tra</param>
        /// <returns>True n?u file l� ?nh h?p l?</returns>
        bool IsValidImage(IFormFile file);
    }
}