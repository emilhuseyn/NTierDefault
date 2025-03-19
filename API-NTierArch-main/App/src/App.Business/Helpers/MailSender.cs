using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using App.Core.Entities;
using QRCoder; 

namespace App.Business.Services.ExternalServices.Abstractions
{
    public static class MailService
    {
        public static void SendQrCode(string recipientEmail, string qrCodeFilePath, Graduate graduate)
        {
            string qrFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", qrCodeFilePath.TrimStart('/'));
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "template.png");

            if (!File.Exists(qrFullPath))
            {
                throw new FileNotFoundException("QR code image not found", qrFullPath);
            }

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Template image not found", templatePath);
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress("mezun@digiteam.az"),
                Subject = "Your QR Code",
                IsBodyHtml = true
            };
            mailMessage.To.Add(recipientEmail);

            string emailHtml = GenerateQRCodeHtml($"{graduate.Name} {graduate.Surname}");

            var templateImage = new LinkedResource(templatePath, "image/png") { ContentId = "templateImage" };
            var qrImage = new LinkedResource(qrFullPath, "image/png") { ContentId = "qrcode" };

            var altView = AlternateView.CreateAlternateViewFromString(emailHtml, null, "text/html");
            altView.LinkedResources.Add(templateImage);
            altView.LinkedResources.Add(qrImage);
            mailMessage.AlternateViews.Add(altView);

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential("mezun@digiteam.az", "eltr ygrt mijp okzj");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
        }
        public static string GenerateQRCodeHtml(string graduateName)
        {
            return $@"<!DOCTYPE html>
<html lang='az'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Məzun 2025 Dəvətnaməsi</title>
    <style>
        body {{
            font-family: 'AzL Palatino Linotype', 'Times New Roman', serif;
            color: #134865;
            text-align: center;
            margin: 0;
            padding: 0;
            width: 100%;
        }}

        .email-container {{
            width: 500px;
            height: 700px;
            margin: 0 auto;
            background-image: url('cid:templateImage'); 
            background-size: contain;
            background-repeat: no-repeat;
            background-position: center;
            position: relative;
        }}

        table {{
            width: 100%;
            height: 100%;
        }}

        td {{
            text-align: center;
            vertical-align: middle;
            color: #134865;
        }}

        .graduate-name {{
            font-size: 18px;
            font-weight: bold;
            padding-top: 320px; /* Reduced from 320px */
            height: 20px
        }}

        .message {{
            font-size: 14px;
            padding-top: 15px; /* Reduced from 20px */
        }}

        .signature {{
            font-size: 16px;
            padding-top: 10px; /* Reduced from 15px */
        }}

        .note {{
            font-size: 12px;
            padding-top: 5px; /* Reduced from 10px */
        }}
        .qr-code {{
        
        }}
        .qr-code img {{
            width: 100px;
            height: 100px;
            margin-left: 195px;
            display: block;
            margin-bottom: 75px; /* Reduced from 40px */
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <table cellspacing='0' cellpadding='0'>
            <tr>
                <td class='graduate-name'>{graduateName}</td>
            </tr>
            <tr>
                <td class='message'>
                    Səni təbrik edirik və uğurlarının davamını diləyirik.<br> 
                    Məzuniyyət günündə səni aramızda görməkdən şad olarıq!
                </td>
            </tr>
            <tr>
                <td class='signature'>Hörmətlə, Bakı Dövlət Universiteti</td>
            </tr>
            <tr>
                <td class='note'>
                    Qeyd: QR Kodu məkana yaxınlaşdığınız zaman göstərməyi unutmayın.<br> 
                    Məkan, tarix və saat öncədən bildiriləcəkdir.
                </td>
            </tr>
            <tr>
                <td class='qr-code'>
                    <img src='cid:qrcode' alt='QR Kod'>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>";
        }

    }
}
