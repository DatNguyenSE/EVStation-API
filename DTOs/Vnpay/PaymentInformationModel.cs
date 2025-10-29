using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Vnpay
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; } = "other";
        [Required(ErrorMessage = "Vui lòng nhập số tiền muốn nạp.")]
        [Range(10000, double.MaxValue, ErrorMessage = "Số tiền nạp tối thiểu là 10.000 VNĐ.")]
        public decimal? Amount { get; set; }
        public string OrderDescription { get; set; } = "Nap tien vao vi";
        public string? Name { get; set; }
        public string? TxnRef { get; set; } 
    }
}