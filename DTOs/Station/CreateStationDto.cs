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
        public double Latitude { get; set; }     //  (có thể để optional nếu sẽ geocode)
        [Range(-180, 180, ErrorMessage = "Kinh độ phải nằm trong khoảng -180 đến 180")]
        public double Longitude { get; set; }  

        [Required(ErrorMessage = "Giờ mở cửa là bắt buộc")]  
        public TimeSpan OpenTime { get; set; }   
        [Required(ErrorMessage = "Giờ đóng cửa là bắt buộc")]
        public TimeSpan CloseTime { get; set; }
        public List<CreateChargingPostDto> Posts { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CloseTime <= OpenTime)
        {
            yield return new ValidationResult(
                "Giờ đóng cửa phải sau giờ mở cửa",
                new[] { nameof(CloseTime), nameof(OpenTime) }
            );
        }
        }
    }
}