using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Vnpay;
using API.Entities;

namespace API.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string txnRef);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}