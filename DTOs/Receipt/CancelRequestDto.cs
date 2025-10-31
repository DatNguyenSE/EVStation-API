using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Receipt
{
    public class CancelRequestDto
    {
        [Required(ErrorMessage = "Lý do hủy là bắt buộc.")]
        public string Reason { get; set; } = string.Empty;
    }
}