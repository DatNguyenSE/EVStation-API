using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Receipt;

namespace API.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailConfirmationAsync(string toEmail, string userId, string token);
        Task SendChargingReceiptAsync(string toEmail, ReceiptDto dto);
        Task SendAccountBannedEmailAsync(string toEmail, string username, int maxViolations, int banDays, DateTimeOffset banUntil);
        Task SendAccountUnbannedEmailAsync(string toEmail, string username);
    }
}