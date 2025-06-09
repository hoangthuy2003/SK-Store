using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Review
{
    public class CreateReviewDto
    {
        // ProductId sẽ được lấy từ URL route, không cần truyền trong body

        [Required(ErrorMessage = "Vui lòng cung cấp điểm đánh giá.")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao.")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự.")]
        public string? Comment { get; set; }

        // UserId sẽ được lấy từ token của người dùng đang đăng nhập
    }
}
