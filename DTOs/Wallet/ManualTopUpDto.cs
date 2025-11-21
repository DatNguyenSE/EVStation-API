using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Wallet
{
    public class ManualTopUpDto
    {
        [Required(ErrorMessage = "Vui lòng nhập Username của người dùng.")]
        public string DriverUserName { get; set; } = null!;
        
        [Required(ErrorMessage = "Vui lòng nhập số tiền cần nạp.")]
        [Range(10000, double.MaxValue, ErrorMessage = "Số tiền nạp tối thiểu là 10,000đ.")]
        public decimal Amount { get; set; }
    }
}