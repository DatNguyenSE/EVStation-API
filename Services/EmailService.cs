using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using API.DTOs.Receipt;
using API.Entities.Email;
using API.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"[EmailService] SendEmailAsync error: {ex}");
                throw new InvalidOperationException($"Failed to send email: {ex.Message}");
            }
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string userId, string token)
        {
            var frontendUrl = "http://localhost:4200";
            var encodeToken = Uri.EscapeDataString(token);
            var confirmationLink = $"{frontendUrl}/confirm-email?userId={userId}&token={encodeToken}";

            var subject = "Xác nhận địa chỉ Email của bạn";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #333;'>Xác nhận Email</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản!</p>
                        <p>Vui lòng nhấp vào nút bên dưới để xác nhận địa chỉ email của bạn:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmationLink}' 
                            style='background-color: #4CAF50; 
                                    color: white; 
                                    padding: 12px 30px; 
                                    text-decoration: none; 
                                    border-radius: 4px;
                                    display: inline-block;'>
                                Xác nhận Email
                            </a>
                        </div>
                        <p style='color: #666; font-size: 14px;'>
                            Nếu bạn không tạo tài khoản này, vui lòng bỏ qua email này.
                        </p>
                        <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                            Link sẽ hết hạn sau 24 giờ.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
        
        public async Task SendChargingReceiptAsync(string toEmail, ReceiptDto dto)
        {
            var subject = $"Hóa đơn phiên sạc #{string.Join(", ", dto.SessionIds ?? new List<int>())} - EVolt Charging Receipt";

            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 0; margin: 0;'>
                <div style='max-width: 650px; margin: 20px auto; background: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.05); padding: 25px;'>
                    <h2 style='color: #333; text-align: center;'>Biên lai thanh toán phiên sạc</h2>
                    <p style='text-align: center; color: #555;'>Cảm ơn bạn <strong>{dto.DriverName ?? "Khách hàng"}</strong> đã sử dụng dịch vụ tại <strong>{dto.StationName ?? "EVolt Station"}</strong>.</p>

                    <hr style='margin: 20px 0;'>

                    <h3 style='color: #444;'>Thông tin phiên sạc</h3>
                    <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                        <tr><td style='padding: 6px;'>🚗 <strong>Biển số xe:</strong></td><td style='text-align:right;'>{dto.VehiclePlate ?? "N/A"}</td></tr>
                        <tr><td style='padding: 6px;'>📍 <strong>Trụ sạc:</strong></td><td style='text-align:right;'>{dto.PostCode ?? "N/A"}</td></tr>
                        <tr><td style='padding: 6px;'>⚡ <strong>Gói cước:</strong></td><td style='text-align:right;'>{dto.PackageName ?? dto.PricingName}</td></tr>
                        <tr><td style='padding: 6px;'>🔋 <strong>Điện năng tiêu thụ:</strong></td><td style='text-align:right;'>{dto.EnergyConsumed:F2} kWh</td></tr>
                        <tr><td style='padding: 6px;'>💰 <strong>Đơn giá (VNĐ/kWh):</strong></td><td style='text-align:right;'>{dto.PricePerKwhSnapshot:N0}</td></tr>
                        <tr><td style='padding: 6px;'>🕒 <strong>Thời gian tạo hóa đơn:</strong></td><td style='text-align:right;'>{dto.CreateAt:HH:mm dd/MM/yyyy}</td></tr>
                        <tr><td style='padding: 6px;'>📅 <strong>Trạng thái:</strong></td><td style='text-align:right;'>{dto.Status}</td></tr>
                    </table>

                    <h3 style='color: #444;'>Chi tiết thanh toán</h3>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <th style='text-align:left; border-bottom:1px solid #ddd; padding: 8px;'>Khoản mục</th>
                            <th style='text-align:right; border-bottom:1px solid #ddd; padding: 8px;'>Số tiền (VNĐ)</th>
                        </tr>
                        <tr>
                            <td style='padding: 8px;'>Phí năng lượng</td>
                            <td style='text-align:right; padding: 8px;'>{dto.EnergyCost:N0}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px;'>Phí chờ</td>
                            <td style='text-align:right; padding: 8px;'>{dto.IdleFee:N0}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px;'>Phí quá giờ</td>
                            <td style='text-align:right; padding: 8px;'>{dto.OverstayFee:N0}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px;'>Giảm giá</td>
                            <td style='text-align:right; padding: 8px; color: #2e7d32;'>-{dto.DiscountAmount:N0}</td>
                        </tr>
                        <tr>
                            <td style='border-top:1px solid #ddd; padding: 8px;'><strong>Tổng cộng</strong></td>
                            <td style='border-top:1px solid #ddd; text-align:right; padding: 8px; font-weight:bold; color:#d32f2f;'>{dto.TotalCost:N0}</td>
                        </tr>
                    </table>

                    {(dto.IdleStartTime.HasValue && dto.IdleEndTime.HasValue ? $@"
                        <p style='margin-top: 20px; color: #555;'>
                            ⏸ Thời gian chờ: {dto.IdleStartTime:HH:mm} - {dto.IdleEndTime:HH:mm} ({(dto.IdleEndTime - dto.IdleStartTime)?.TotalMinutes:F0} phút)
                        </p>" : "")}

                    <hr style='margin: 30px 0;'>
                    <p style='text-align: center; color: #555;'>Nếu bạn có thắc mắc, vui lòng liên hệ đội ngũ hỗ trợ của chúng tôi qua email <a href='mailto:support@evolt.vn'>support@evolt.vn</a>.</p>
                    <p style='text-align: center; color: #aaa; font-size: 12px;'>© 2025 EVolt System. All rights reserved.</p>
                </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAccountBannedEmailAsync(string toEmail, string username, int maxViolations, int banDays, DateTimeOffset banUntil)
        {
            var subject = "THÔNG BÁO: Tạm khóa tài khoản EVolt do vi phạm quy tắc đặt chỗ";
            var banUntilStr = banUntil.AddHours(7).ToString("HH:mm dd/MM/yyyy"); // Giả định múi giờ hiển thị là +7

            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); padding: 30px; border-left: 5px solid #d32f2f;'>
                        <h2 style='color: #d32f2f; text-align: center; margin-bottom: 20px;'>⚠️ Thông báo Tạm khóa Tài khoản ⚠️</h2>
                        <p style='color: #333; font-size: 16px;'>Chào <strong>{username}</strong>,</p>
                        
                        <p style='color: #555;'>Chúng tôi thông báo tài khoản của bạn đã bị tạm khóa do vi phạm Quy tắc Đặt chỗ quá số lần cho phép.</p>
                        
                        <div style='background-color: #fef0f0; border: 1px solid #f0a0a0; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p style='color: #900; margin-top: 0;'><strong>Chi tiết vi phạm:</strong></p>
                            <ul style='color: #333; padding-left: 20px;'>
                                <li><strong>Lý do:</strong> Vi phạm quy tắc không đến (No-Show).</li>
                                <li><strong>Số lần vi phạm:</strong> Đạt/vượt ngưỡng {maxViolations} lần.</li>
                                <li><strong>Thời hạn cấm:</strong> Tài khoản sẽ bị khóa trong <strong>{banDays} ngày</strong>.</li>
                                <li><strong>Mở khóa vào:</strong> <strong>{banUntilStr}</strong></li>
                            </ul>
                        </div>

                        <p style='color: #555;'>Trong thời gian tài khoản bị khóa, bạn sẽ không thể thực hiện các chức năng liên quan đến việc đặt chỗ và sử dụng dịch vụ sạc điện.</p>
                        
                        <p style='color: #555; margin-top: 20px;'>Vui lòng tuân thủ nghiêm ngặt Quy tắc Đặt chỗ để tránh các hình phạt tiếp theo.</p>
                        
                        <hr style='margin: 30px 0; border: 0; border-top: 1px solid #eee;'>
                        <p style='text-align: center; color: #777; font-size: 12px;'>Mọi thắc mắc, vui lòng liên hệ Bộ phận Hỗ trợ: <a href='mailto:support@evolt.vn'>evoltstation@gmail.com</a></p>
                        <p style='text-align: center; color: #aaa; font-size: 10px;'>© 2025 EVolt System.</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}