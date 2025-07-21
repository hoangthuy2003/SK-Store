using Application.DTOs.User;
using BusinessObjects;
using Repositories.UnitOfWork;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserAddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserAddressDto>> GetUserAddressesAsync(int userId)
        {
            var addresses = await _unitOfWork.UserAddresses.GetAddressesByUserIdAsync(userId);
            return addresses.Select(a => new UserAddressDto
            {
                AddressId = a.AddressId,
                UserId = a.UserId,
                AddressLine = a.AddressLine,
                RecipientName = a.RecipientName,
                RecipientPhoneNumber = a.RecipientPhoneNumber,
                IsDefault = a.IsDefault
            }).ToList();
        }

        public async Task<UserAddressDto?> GetUserAddressByIdAsync(int addressId, int userId)
        {
            var address = await _unitOfWork.UserAddresses.GetByIdAsync(addressId);
            if (address == null || address.UserId != userId)
            {
                return null;
            }

            return new UserAddressDto
            {
                AddressId = address.AddressId,
                UserId = address.UserId,
                AddressLine = address.AddressLine,
                RecipientName = address.RecipientName,
                RecipientPhoneNumber = address.RecipientPhoneNumber,
                IsDefault = address.IsDefault
            };
        }

        public async Task<UserAddressDto?> GetDefaultAddressAsync(int userId)
        {
            var address = await _unitOfWork.UserAddresses.GetDefaultAddressAsync(userId);
            if (address == null)
            {
                return null;
            }

            return new UserAddressDto
            {
                AddressId = address.AddressId,
                UserId = address.UserId,
                AddressLine = address.AddressLine,
                RecipientName = address.RecipientName,
                RecipientPhoneNumber = address.RecipientPhoneNumber,
                IsDefault = address.IsDefault
            };
        }

        public async Task<(UserAddressDto? Address, string? ErrorMessage)> AddUserAddressAsync(int userId, CreateUserAddressDto createDto)
        {
            try
            {
                // Kiểm tra user có tồn tại không
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return (null, "Người dùng không tồn tại.");
                }

                // Nếu đây là địa chỉ mặc định hoặc là địa chỉ đầu tiên, đặt làm mặc định
                var existingAddresses = await _unitOfWork.UserAddresses.GetAddressesByUserIdAsync(userId);
                bool shouldBeDefault = createDto.IsDefault || !existingAddresses.Any();

                // Nếu đặt làm mặc định, xóa địa chỉ mặc định hiện tại
                if (shouldBeDefault)
                {
                    await _unitOfWork.UserAddresses.ClearDefaultAddressesAsync(userId);
                }

                var newAddress = new UserAddress
                {
                    UserId = userId,
                    AddressLine = createDto.AddressLine.Trim(),
                    RecipientName = createDto.RecipientName?.Trim(),
                    RecipientPhoneNumber = createDto.RecipientPhoneNumber?.Trim(),
                    IsDefault = shouldBeDefault
                };

                await _unitOfWork.UserAddresses.AddAsync(newAddress);
                await _unitOfWork.CompleteAsync();

                return (new UserAddressDto
                {
                    AddressId = newAddress.AddressId,
                    UserId = newAddress.UserId,
                    AddressLine = newAddress.AddressLine,
                    RecipientName = newAddress.RecipientName,
                    RecipientPhoneNumber = newAddress.RecipientPhoneNumber,
                    IsDefault = newAddress.IsDefault
                }, null);
            }
            catch (Exception ex)
            {
                return (null, $"Có lỗi xảy ra khi thêm địa chỉ: {ex.Message}");
            }
        }

        public async Task<(UserAddressDto? Address, string? ErrorMessage)> UpdateUserAddressAsync(int addressId, int userId, UpdateUserAddressDto updateDto)
        {
            try
            {
                var address = await _unitOfWork.UserAddresses.GetByIdAsync(addressId);
                if (address == null || address.UserId != userId)
                {
                    return (null, "Không tìm thấy địa chỉ hoặc bạn không có quyền chỉnh sửa.");
                }

                // Nếu đặt làm mặc định, xóa địa chỉ mặc định hiện tại
                if (updateDto.IsDefault && !address.IsDefault)
                {
                    await _unitOfWork.UserAddresses.ClearDefaultAddressesAsync(userId);
                }

                // Cập nhật thông tin địa chỉ
                address.AddressLine = updateDto.AddressLine.Trim();
                address.RecipientName = updateDto.RecipientName?.Trim();
                address.RecipientPhoneNumber = updateDto.RecipientPhoneNumber?.Trim();
                address.IsDefault = updateDto.IsDefault;

                await _unitOfWork.UserAddresses.UpdateAsync(address);
                await _unitOfWork.CompleteAsync();

                return (new UserAddressDto
                {
                    AddressId = address.AddressId,
                    UserId = address.UserId,
                    AddressLine = address.AddressLine,
                    RecipientName = address.RecipientName,
                    RecipientPhoneNumber = address.RecipientPhoneNumber,
                    IsDefault = address.IsDefault
                }, null);
            }
            catch (Exception ex)
            {
                return (null, $"Có lỗi xảy ra khi cập nhật địa chỉ: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteUserAddressAsync(int addressId, int userId)
        {
            try
            {
                var address = await _unitOfWork.UserAddresses.GetByIdAsync(addressId);
                if (address == null || address.UserId != userId)
                {
                    return (false, "Không tìm thấy địa chỉ hoặc bạn không có quyền xóa.");
                }

                // Kiểm tra xem có phải địa chỉ mặc định không
                bool wasDefault = address.IsDefault;

                await _unitOfWork.UserAddresses.DeleteAsync(address);
                await _unitOfWork.CompleteAsync();

                // Nếu xóa địa chỉ mặc định, đặt địa chỉ đầu tiên còn lại làm mặc định
                if (wasDefault)
                {
                    var remainingAddresses = await _unitOfWork.UserAddresses.GetAddressesByUserIdAsync(userId);
                    var firstAddress = remainingAddresses.FirstOrDefault();
                    if (firstAddress != null)
                    {
                        await _unitOfWork.UserAddresses.SetDefaultAddressAsync(userId, firstAddress.AddressId);
                        await _unitOfWork.CompleteAsync();
                    }
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra khi xóa địa chỉ: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> SetDefaultAddressAsync(int addressId, int userId)
        {
            try
            {
                var isOwner = await _unitOfWork.UserAddresses.IsAddressOwnerAsync(userId, addressId);
                if (!isOwner)
                {
                    return (false, "Không tìm thấy địa chỉ hoặc bạn không có quyền thay đổi.");
                }

                await _unitOfWork.UserAddresses.SetDefaultAddressAsync(userId, addressId);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra khi đặt địa chỉ mặc định: {ex.Message}");
            }
        }
    }
}
