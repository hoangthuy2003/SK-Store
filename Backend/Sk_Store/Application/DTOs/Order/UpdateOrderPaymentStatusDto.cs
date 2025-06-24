using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Order
{
    public class UpdateOrderPaymentStatusDto
    {
        [Required]
        public string NewPaymentStatus { get; set; }
    }
}