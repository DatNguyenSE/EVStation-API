using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Receipt
{
    public class ConfirmPaymentRequestDto
    {
        [Required]
        public string PaymentMethod { get; set; } = null!;// "Cash", "Credit Card",...
    }
}