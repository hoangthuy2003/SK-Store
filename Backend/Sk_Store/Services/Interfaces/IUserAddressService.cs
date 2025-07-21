using Application.DTOs.User;

namespace Services.Interfaces
{
    public interface IUserAddressService
    {
        Task<IEnumerable<UserAddressDto>> GetUserAddressesAsync(int userId);
        Task<UserAddressDto?> GetUserAddressByIdAsync(int addressId, int userId);
        Task<UserAddressDto?> GetDefaultAddressAsync(int userId);
        Task<(UserAddressDto? Address, string? ErrorMessage)> AddUserAddressAsync(int userId, CreateUserAddressDto createDto);
        Task<(UserAddressDto? Address, string? ErrorMessage)> UpdateUserAddressAsync(int addressId, int userId, UpdateUserAddressDto updateDto);
        Task<(bool Success, string? ErrorMessage)> DeleteUserAddressAsync(int addressId, int userId);
        Task<(bool Success, string? ErrorMessage)> SetDefaultAddressAsync(int addressId, int userId);
    }
}
