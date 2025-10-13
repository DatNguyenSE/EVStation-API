using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingPost;

namespace API.DTOs.Station
{
    public class CreateStationDto : IValidatableObject
    {
        [Required(ErrorMessage = "Tên trạm là bắt buộc")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên trạm phải từ 3–100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "Vĩ độ phải nằm trong khoảng -90 đến 90")]
        public double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Kinh độ phải nằm trong khoảng -180 đến 180")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Giờ mở cửa là bắt buộc")]
        public string OpenTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giờ đóng cửa là bắt buộc")]
        public string CloseTime { get; set; } = string.Empty;

        public List<CreateChargingPostDto> Posts { get; set; } = new();

        public TimeSpan? GetOpenTime()
        {
            if (string.IsNullOrEmpty(OpenTime))
            {
                return null;
            }
            if (!TimeSpan.TryParse(OpenTime, out var time))
            {
                throw new ArgumentException("Giờ mở cửa không hợp lệ");
            }
            return time.TotalHours >= 24 ? TimeSpan.Zero : time;
        }

        public TimeSpan? GetCloseTime()
        {
            if (string.IsNullOrEmpty(CloseTime))
            {
                return null;
            }
            if (!TimeSpan.TryParse(CloseTime, out var time))
            {
                throw new ArgumentException("Giờ đóng cửa không hợp lệ");
            }
            return time.TotalHours >= 24 ? TimeSpan.Zero : time;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(OpenTime) && !TimeSpan.TryParse(OpenTime, out _))
            {
                yield return new ValidationResult("Giờ mở cửa không hợp lệ", new[] { nameof(OpenTime) });
            }
            if (!string.IsNullOrEmpty(CloseTime) && !TimeSpan.TryParse(CloseTime, out _))
            {
                yield return new ValidationResult("Giờ đóng cửa không hợp lệ", new[] { nameof(CloseTime) });
            }
            if (GetOpenTime().HasValue && GetCloseTime().HasValue && GetCloseTime() <= GetOpenTime() && GetCloseTime() != TimeSpan.Zero)
            {
                yield return new ValidationResult(
                    "Giờ đóng cửa phải sau giờ mở cửa",
                    new[] { nameof(CloseTime), nameof(OpenTime) }
                );
            }
        }
    }
}