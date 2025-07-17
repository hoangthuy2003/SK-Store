using BusinessObjects;

namespace Repositories.Interfaces
{
    /// <summary>
    /// Interface cho repository qu?n l� ?nh s?n ph?m
    /// </summary>
    public interface IProductImageRepository
    {
        /// <summary>
        /// L?y ?nh theo ID
        /// </summary>
        /// <param name="imageId">ID c?a ?nh</param>
        /// <returns>ProductImage object ho?c null n?u kh�ng t�m th?y</returns>
        Task<ProductImage?> GetByIdAsync(int imageId);

        /// <summary>
        /// L?y t?t c? ?nh c?a m?t s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>Danh s�ch ?nh c?a s?n ph?m</returns>
        Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(int productId);

        /// <summary>
        /// L?y ?nh ch�nh c?a s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>?nh ch�nh ho?c null n?u kh�ng c�</returns>
        Task<ProductImage?> GetPrimaryImageByProductIdAsync(int productId);

        /// <summary>
        /// L?y t?t c? ?nh ph? c?a s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>Danh s�ch ?nh ph?</returns>
        Task<IEnumerable<ProductImage>> GetSecondaryImagesByProductIdAsync(int productId);

        /// <summary>
        /// Th�m ?nh m?i
        /// </summary>
        /// <param name="productImage">?nh c?n th�m</param>
        /// <returns>?nh ?� ???c th�m</returns>
        Task<ProductImage> AddAsync(ProductImage productImage);

        /// <summary>
        /// Th�m nhi?u ?nh c�ng l�c
        /// </summary>
        /// <param name="productImages">Danh s�ch ?nh c?n th�m</param>
        /// <returns>Danh s�ch ?nh ?� ???c th�m</returns>
        Task<IEnumerable<ProductImage>> AddRangeAsync(IEnumerable<ProductImage> productImages);

        /// <summary>
        /// C?p nh?t th�ng tin ?nh
        /// </summary>
        /// <param name="productImage">?nh c?n c?p nh?t</param>
        /// <returns>?nh ?� ???c c?p nh?t</returns>
        Task<ProductImage> UpdateAsync(ProductImage productImage);

        /// <summary>
        /// X�a ?nh theo ID
        /// </summary>
        /// <param name="imageId">ID c?a ?nh c?n x�a</param>
        /// <returns>True n?u x�a th�nh c�ng, False n?u kh�ng t�m th?y</returns>
        Task<bool> DeleteAsync(int imageId);

        /// <summary>
        /// X�a ?nh theo object
        /// </summary>
        /// <param name="productImage">?nh c?n x�a</param>
        /// <returns>True n?u x�a th�nh c�ng</returns>
        Task<bool> DeleteAsync(ProductImage productImage);

        /// <summary>
        /// X�a t?t c? ?nh c?a m?t s?n ph?m
        /// </summary>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>S? l??ng ?nh ?� x�a</returns>
        Task<int> DeleteAllByProductIdAsync(int productId);

        /// <summary>
        /// ??t ?nh l�m ?nh ch�nh (v� b? primary c?a c�c ?nh kh�c)
        /// </summary>
        /// <param name="imageId">ID c?a ?nh s? th�nh ?nh ch�nh</param>
        /// <param name="productId">ID c?a s?n ph?m</param>
        /// <returns>True n?u th�nh c�ng</returns>
        Task<bool> SetPrimaryImageAsync(int imageId, int productId);

        /// <summary>
        /// Ki?m tra ?nh c� thu?c v? s?n ph?m kh�ng
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
        /// Ki?m tra ?nh c� t?n t?i kh�ng
        /// </summary>
        /// <param name="imageId">ID c?a ?nh</param>
        /// <returns>True n?u t?n t?i</returns>
        Task<bool> ExistsAsync(int imageId);
    }
}