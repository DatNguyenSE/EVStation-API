using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Receipt
{
    public class RefundRequestDto
    {
        [Required(ErrorMessage = "Mã hóa đơn là bắt buộc.")]
        public int ReceiptId { get; set; }

        [Required(ErrorMessage = "Số tiền hoàn là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền hoàn phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Lý do hoàn tiền là bắt buộc.")]
        [MaxLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự.")]
        public string Reason { get; set; } = string.Empty;
    }
}