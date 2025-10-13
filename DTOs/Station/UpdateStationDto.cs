using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;

namespace API.DTOs.Station
{
    public class UpdateStationDto : IValidatableObject
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên trạm phải từ 3–100 ký tự")]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public StationStatus? Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OpenTime.HasValue && CloseTime.HasValue)
            {
                if (CloseTime <= OpenTime)
                {
                    yield return new ValidationResult( // yield return là nó tạo list để return => tiện hơn khi trả IEnumerable
                        "Giờ đóng cửa phải sau giờ mở cửa",
                        new[] { nameof(CloseTime), nameof(OpenTime) } // trả cho FE biết lỗi thuộc về property nào
                    );
                }
            }
        }
    }
}