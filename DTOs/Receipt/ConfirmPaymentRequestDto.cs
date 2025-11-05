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
        public PaymentMethod PaymentMethod { get; set; }
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        EWallet,
        BankTransfer
    }

}