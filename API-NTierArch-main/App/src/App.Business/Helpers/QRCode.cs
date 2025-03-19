using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using QRCoder;

namespace App.Business.Helpers
{
    public static class QRCode
    {
        private static string GenerateQRCode(string qrData, int graduateId)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);

                string uploadsFolder = Path.Combine("_webHostEnvironment.WebRootPath", "Uploads", "QRs");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = $"{graduateId}.png";
                string filePath = Path.Combine(uploadsFolder, fileName);
                File.WriteAllBytes(filePath, qrCodeBytes);

                return $"/Uploads/QRs/{fileName}"; // Return relative path for frontend usage
            }
        }
    }
}
