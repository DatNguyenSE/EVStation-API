using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace API.Services
{
    public interface IQRCodeService
    {
        byte[] GenerateQRCode(string content);
    }

    // tạo QR code cho trụ
    public class QRCodeService : IQRCodeService
    {
        public byte[] GenerateQRCode(string content)
        {
            // chịu trách nhiệm tạo dữ liệu QR (matrix các ô trắng đen) từ chuỗi text bạn truyền vào
            using var qrGenerator = new QRCodeGenerator();

            // Tạo dữ liệu QR code thô (kiểu QRCodeData) từ nội dung text
            // QRCodeGenerator.ECCLevel.Q là mức chống lỗi (kiểu có thể phục hồi sau khi tạo QR code thoi)
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            // Tạo đối tượng QRCode thật sự từ dữ liệu vừa sinh
            using var qrCode = new QRCode(qrCodeData);

            // Sinh ra ảnh bitmap từ QR data, mỗi “ô” của QR có kích thước 20 pixel.
            using var bitmap = qrCode.GetGraphic(20);


            // Tạo một luồng bộ nhớ (MemoryStream) tạm thời để chứa ảnh
            using var ms = new MemoryStream();
            // Lưu ảnh bitmap vừa tạo vào luồng ms theo định dạng PNG
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}