using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using API.DTOs.Receipt;
using API.Entities;
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

        public async Task SendChargingReceiptAsync(string toEmail, Receipt receipt)
        {
            var sessionList = receipt.ChargingSessions?.ToList() ?? new List<ChargingSession>();
            var sessionIds = sessionList.Select(s => s.Id).ToList();

            var subject = $"Biên lai thanh toán phiên sạc #{string.Join(", ", sessionIds)} - EVolt Charging Receipt";

            string sessionRows = "";
            foreach (var s in sessionList)
            {
                sessionRows += $@"
                    <tr>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>{s.VehiclePlate}</td>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>{s.ChargingPost.Code}</td>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>{s.StartTime:HH:mm dd/MM/yyyy}</td>
                        <td style='padding:10px; border-bottom:1px solid #eee;'>{(s.EndTime.HasValue ? s.EndTime.Value.ToString("HH:mm dd/MM/yyyy") : "Đang sạc")}</td>
                        <td style='padding:10px; border-bottom:1px solid #eee; text-align:right;'>{s.EnergyConsumed:F2} kWh</td>
                        <td style='padding:10px; border-bottom:1px solid #eee; text-align:right;'>{s.TotalCost:N0} VNĐ</td>
                    </tr>";
            }

            var body = $@"
                <html>
                <body style='font-family: system-ui, -apple-system, Roboto, sans-serif; background-color: #f8fafc; margin:0; padding:0;'>
                    <div style='max-width:720px; margin:30px auto; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 16px rgba(0,0,0,0.08);'>

                        <!-- HEADER -->
                        <div style='background:linear-gradient(90deg, #1565c0, #0d47a1); color:#fff; padding:30px; text-align:center;'>
                            <h2 style='margin:0; font-size:22px; letter-spacing:0.3px;'>Biên lai thanh toán phiên sạc</h2>
                            <p style='margin:6px 0 0; font-size:14px; opacity:0.9;'>Mã hóa đơn: #{receipt.Id}</p>
                        </div>

                        <!-- BODY -->
                        <div style='padding:30px;'>
                            <p style='font-size:15px; color:#333;'>Xin chào <strong>{receipt.AppUser?.FullName ?? "Quý khách"}</strong>,</p>
                            <p style='font-size:15px; color:#555; line-height:1.6;'>
                                Cảm ơn bạn đã sử dụng dịch vụ tại 
                                <strong>{sessionList.FirstOrDefault()?.ChargingPost?.StationName ?? "EVolt Station"}</strong>.
                                Dưới đây là chi tiết hóa đơn thanh toán của bạn:
                            </p>

                            <h3 style='color:#0d47a1; margin-top:30px; font-size:17px;'>Thông tin hóa đơn</h3>
                            <table style='width:100%; border-collapse:collapse; font-size:14px; color:#444; margin-top:10px;'>
                                <tr><td style='padding:6px;'>Mã hóa đơn</td><td style='text-align:right;'>{receipt.Id}</td></tr>
                                <tr><td style='padding:6px;'>Ngày tạo</td><td style='text-align:right;'>{receipt.CreateAt:HH:mm dd/MM/yyyy}</td></tr>
                                <tr><td style='padding:6px;'>Gói cước</td><td style='text-align:right;'>{receipt.Package?.Package.Name ?? receipt.PricingName}</td></tr>
                                <tr><td style='padding:6px;'>Đơn giá</td><td style='text-align:right;'>{receipt.PricePerKwhSnapshot:N0} VNĐ/kWh</td></tr>
                                <tr><td style='padding:6px;'>Phương thức thanh toán</td><td style='text-align:right;'>{receipt.PaymentMethod ?? "Không xác định"}</td></tr>
                                <tr><td style='padding:6px;'>Trạng thái</td><td style='text-align:right;'>{receipt.Status}</td></tr>
                            </table>

                            {(receipt.ConfirmedByStaff != null ? $@"
                                <p style='margin-top:10px; color:#555; font-size:14px;'>
                                    Xác nhận bởi nhân viên: <strong>{receipt.ConfirmedByStaff.FullName}</strong> lúc {receipt.ConfirmedAt:HH:mm dd/MM/yyyy}
                                </p>" : "")}

                            <h3 style='color:#0d47a1; margin-top:35px; font-size:17px;'>Danh sách phiên sạc</h3>
                            <table style='width:100%; border-collapse:collapse; font-size:14px; margin-top:10px;'>
                                <thead style='background:#f1f3f6;'>
                                    <tr>
                                        <th style='padding:10px; text-align:left;'>Xe</th>
                                        <th style='padding:10px; text-align:left;'>Trụ sạc</th>
                                        <th style='padding:10px;'>Bắt đầu</th>
                                        <th style='padding:10px;'>Kết thúc</th>
                                        <th style='padding:10px;'>Điện năng</th>
                                        <th style='padding:10px; text-align:right;'>Tổng (VNĐ)</th>
                                    </tr>
                                </thead>
                                <tbody>{sessionRows}</tbody>
                            </table>

                            {(receipt.IdleStartTime.HasValue && receipt.IdleEndTime.HasValue ? $@"
                                <p style='margin-top:20px; color:#555; font-size:14px;'>
                                    Thời gian chờ: {receipt.IdleStartTime:HH:mm} - {receipt.IdleEndTime:HH:mm}
                                    ({(receipt.IdleEndTime - receipt.IdleStartTime)?.TotalMinutes:F0} phút)
                                </p>" : "")}

                            <h3 style='color:#0d47a1; margin-top:35px; font-size:17px;'>Chi tiết thanh toán</h3>
                            <table style='width:100%; border-collapse:collapse; font-size:14px; color:#444; margin-top:10px;'>
                                <tr><td style='padding:8px;'>Phí năng lượng</td><td style='text-align:right;'>{receipt.EnergyCost:N0}</td></tr>
                                <tr><td style='padding:8px;'>Phí chờ</td><td style='text-align:right;'>{receipt.IdleFee:N0}</td></tr>
                                <tr><td style='padding:8px;'>Phí quá giờ</td><td style='text-align:right;'>{receipt.OverstayFee:N0}</td></tr>
                                <tr><td style='padding:8px;'>Giảm giá</td><td style='text-align:right; color:#2e7d32;'>-{receipt.DiscountAmount:N0}</td></tr>
                                <tr style='border-top:2px solid #ddd;'>
                                    <td style='padding:12px 8px; font-weight:600;'>Tổng cộng</td>
                                    <td style='text-align:right; padding:12px 8px; color:#d32f2f; font-weight:600;'>{receipt.TotalCost:N0}</td>
                                </tr>
                            </table>

                            <div style='margin-top:40px; text-align:center;'>
                                <p style='font-size:14px; color:#555;'>Cảm ơn bạn đã tin tưởng sử dụng dịch vụ của <strong>EVolt</strong>.</p>
                                <p style='font-size:13px; color:#777;'>
                                    Nếu bạn có thắc mắc, vui lòng liên hệ: 
                                    <a href='mailto:support@evolt.vn' style='color:#1565c0; text-decoration:none;'>support@evolt.vn</a>
                                </p>
                            </div>
                        </div>

                        <!-- FOOTER -->
                        <div style='background:#f5f5f5; text-align:center; padding:20px; font-size:12px; color:#999;'>
                            © 2025 EVolt System. All rights reserved.<br>
                            <a href='https://evolt.vn' style='color:#999; text-decoration:none;'>www.evolt.vn</a>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAccountBannedEmailAsync(string toEmail, string username, int maxViolations, int banDays, DateTimeOffset banUntil)
        {
            var subject = "THÔNG BÁO: Tạm khóa tài khoản EVolt";
            var banUntilStr = banUntil.ToString("HH:mm dd/MM/yyyy");

            // Thay đổi logic để hiển thị chi tiết vi phạm khác nhau cho Ban thủ công và Ban tự động
            string violationDetailsHtml;
            if (maxViolations > 0)
            {
                subject += " do vi phạm quy tắc đặt chỗ";
                violationDetailsHtml = $@"
            <p style='color: #900; margin-top: 0;'><strong>Chi tiết vi phạm:</strong></p>
            <ul style='color: #333; padding-left: 20px;'>
                <li><strong>Lý do:</strong> Vi phạm quy tắc không đến (No-Show) hoặc hủy đặt chỗ không đúng hạn.</li>
                <li><strong>Số lần vi phạm:</strong> Đạt/vượt ngưỡng {maxViolations} lần.</li>
                <li><strong>Thời hạn cấm:</strong> Tài khoản sẽ bị khóa trong <strong>{banDays} ngày</strong>.</li>
                <li><strong>Mở khóa vào:</strong> <strong>{banUntilStr}</strong></li>
            </ul>";
            }
            else
            {
                subject += " do quyết định từ Ban Quản lý";
                violationDetailsHtml = $@"
            <p style='color: #900; margin-top: 0;'><strong>Chi tiết:</strong></p>
            <ul style='color: #333; padding-left: 20px;'>
                <li><strong>Lý do:</strong> Tài khoản bị tạm khóa theo quyết định của Ban Quản lý.</li>
                <li><strong>Thời hạn cấm:</strong> Tài khoản sẽ bị khóa trong <strong>{banDays} ngày</strong>.</li>
                <li><strong>Mở khóa vào:</strong> <strong>{banUntilStr}</strong></li>
            </ul>";
            }

            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); padding: 30px; border-left: 5px solid #d32f2f;'>
                    <h2 style='color: #d32f2f; text-align: center; margin-bottom: 20px;'>⚠️ Thông báo Tạm khóa Tài khoản ⚠️</h2>
                    <p style='color: #333; font-size: 16px;'>Chào <strong>{username}</strong>,</p>
                    
                    <p style='color: #555;'>Chúng tôi thông báo tài khoản của bạn đã bị tạm khóa.</p>
                    
                    <div style='background-color: #fef0f0; border: 1px solid #f0a0a0; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        {violationDetailsHtml}
                    </div>

                    <p style='color: #555;'>Trong thời gian tài khoản bị khóa, bạn sẽ không thể thực hiện các chức năng liên quan đến việc đặt chỗ và sử dụng dịch vụ sạc điện.</p>
                    
                    <p style='color: #555; margin-top: 20px;'>Vui lòng tuân thủ nghiêm ngặt các quy định của hệ thống.</p>
                    
                    <hr style='margin: 30px 0; border: 0; border-top: 1px solid #eee;'>
                    <p style='text-align: center; color: #777; font-size: 12px;'>Mọi thắc mắc, vui lòng liên hệ Bộ phận Hỗ trợ: <a href='mailto:evoltstation@gmail.com'>evoltstation@gmail.com</a></p>
                    <p style='text-align: center; color: #aaa; font-size: 10px;'>© 2025 EVolt System.</p>
                </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}