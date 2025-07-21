using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class UserAddressDto
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string AddressLine { get; set; } = null!;
        public string? RecipientName { get; set; }
        public string? RecipientPhoneNumber { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CreateUserAddressDto
    {
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string AddressLine { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự")]
        public string? RecipientName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? RecipientPhoneNumber { get; set; }

        public bool IsDefault { get; set; } = false;
    }

    public class UpdateUserAddressDto
    {
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string AddressLine { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự")]
        public string? RecipientName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? RecipientPhoneNumber { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
