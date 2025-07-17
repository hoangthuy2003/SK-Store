using BusinessObjects;

namespace Repositories.Interfaces
{
    /// <summary>
    /// Interface cho repository qu?n lý ?nh s?n ph?m
    /// </summary>
    public interface IProductImageRepository
    {
        /// <summary>
        /// L?y ?nh theo ID
        /// </summary>
        /// <param name="imageId">ID c?a ?nh</param>
        /// <returns>ProductImage object ho?c null n?u không tìm th?y</returns>
        Task<ProductImage?> GetByIdAsync(int imageId);

        /// <summary>
        /// L?y t?t c? ?nh c?a m?t s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>Danh sách ?nh c?a s?n ph?m</returns>
        Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId);

        /// <summary>
        /// L?y ?nh chính c?a s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>?nh chính ho?c null n?u không có</returns>
        Task<ProductImage?> GetPrimaryImageByProductIdAsync(int productId);

        /// <summary>
        /// L?y t?t c? ?nh ph? c?a s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>Danh sách ?nh ph?</returns>
        Task<IEnumerable<ProductImage>> GetSecondaryImagesByProductIdAsync(int productId);

        /// <summary>
        /// Thêm ?nh m?i
        /// </summary>
        /// <param name="productImage">?nh c?n thêm</param>
        /// <returns>?nh ?ã ???c thêm</returns>
        Task<ProductImage> AddAsync(ProductImage productImage);

        /// <summary>
        /// Thêm nhi?u ?nh cùng lúc
        /// </summary>
        /// <param name="productImages">Danh sách ?nh c?n thêm</param>
        /// <returns>Danh sách ?nh ?ã ???c thêm</returns>
        Task<IEnumerable<ProductImage>> AddRangeAsync(IEnumerable<ProductImage> productImages);

        /// <summary>
        /// C?p nh?t thông tin ?nh
        /// </summary>
        /// <param name="productImage">?nh c?n c?p nh?t</param>
        /// <returns>?nh ?ã ???c c?p nh?t</returns>
        Task<ProductImage> UpdateAsync(ProductImage productImage);

        /// <summary>
        /// Xóa ?nh theo ID
        /// </summary>
        /// <param name="imageId">ID c?a ?nh c?n xóa</param>
        /// <returns>True n?u xóa thành công, False n?u không tìm th?y</returns>
        Task<bool> DeleteAsync(int imageId);

        /// <summary>
        /// Xóa ?nh theo object
        /// </summary>
        /// <param name="productImage">?nh c?n xóa</param>
        /// <returns>True n?u xóa thành công</returns>
        Task<bool> DeleteAsync(ProductImage productImage);

        /// <summary>
        /// Xóa t?t c? ?nh c?a m?t s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>S? l??ng ?nh ?ã xóa</returns>
        Task<int> DeleteAllByProductIdAsync(int productId);

        /// <summary>
        /// ??t ?nh làm ?nh chính (và b? primary c?a các ?nh khác)
        /// </summary>
        /// <param name="imageId">ID c?a ?nh s? thành ?nh chính</param>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>True n?u thành công</returns>
        Task<bool> SetPrimaryImageAsync(int imageId, int productId);

        /// <summary>
        /// Ki?m tra ?nh có thu?c v? s?n ph?m không
        /// </summary>
        /// <param name="imageId">ID c?a ?nh</param>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>True n?u ?nh thu?c v? s?n ph?m</returns>
        Task<bool> ImageBelongsToProductAsync(int imageId, int productId);

        /// <summary>
        /// ??m s? l??ng ?nh c?a s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>S? l??ng ?nh</returns>
        Task<int> CountImagesByProductIdAsync(int productId);

        /// <summary>
        /// Ki?m tra ?nh có t?n t?i không
        /// </summary>
        /// <param name="imageId">ID c?a ?nh</param>
        /// <returns>True n?u t?n t?i</returns>
        Task<bool> ExistsAsync(int imageId);
    }
}