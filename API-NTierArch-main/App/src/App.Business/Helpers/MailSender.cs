using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using App.Core.Entities;
using DinkToPdf;
using Microsoft.Identity.Client;
using QRCoder; 

namespace App.Business.Services.ExternalServices.Abstractions
{
    public static class MailService
    {
        public static SynchronizedConverter _pdfConverter = new SynchronizedConverter(new PdfTools());

        public static void SendQrCode(string recipientEmail, string qrCodeFilePath, Graduate graduate)
        {
            var context = new CustomAssemblyLoadContext();
            var dllPath = Path.Combine(Directory.GetCurrentDirectory(), "lib", "wkhtmltox", "libwkhtmltox.dll");
            try
            {
                context.LoadUnmanagedLibrary(dllPath);
                Console.WriteLine("DLL Loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading DLL: {ex.Message}");
            }


 
            string qrFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", qrCodeFilePath.TrimStart('/'));
            if (!File.Exists(qrFullPath))
                throw new FileNotFoundException("QR code image not found", qrFullPath);

            string html = GenerateQRCodeHtml($"{graduate.Name} {graduate.Surname}")
                .Replace("cid:qrcode", $"file:///{qrFullPath.Replace("\\", "/")}");

            var pdfBytes = ConvertHtmlToPdf(html);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("mezun@digiteam.az"),
                Subject = "Məzun Dəvətnaməsi (PDF)",
                Body = "Zəhmət olmasa PDF əlavəsini yoxlayın.",
                IsBodyHtml = false
            };

            mailMessage.To.Add(recipientEmail);
            mailMessage.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "devetname.pdf"));

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential("mezun@digiteam.az", "eltr ygrt mijp okzj");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
        }

    
        private static byte[] ConvertHtmlToPdf(string html)
        {
            try
            {
                Console.WriteLine("Starting PDF conversion...");

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = new GlobalSettings
                    {
                        PaperSize = new PechkinPaperSize("102mm", "143mm"),  // Add units (mm)
                        Orientation = Orientation.Portrait,
                        Margins = new MarginSettings { Top = 0, Bottom = 0, Left = 0, Right = 0 }
                    },
                    Objects = {
                new ObjectSettings
                {
                    HtmlContent = html,
                    WebSettings = {
                        DefaultEncoding = "utf-8",
                        LoadImages = true,
                        EnableJavascript = false  // Disable JavaScript to prevent hanging
                    }
                }
            }
                };

                Console.WriteLine("Configured document, attempting conversion...");

                // Create a task with timeout
                var task = Task.Run(() => _pdfConverter.Convert(doc));
                if (!task.Wait(TimeSpan.FromSeconds(15)))  
                {
                    Console.WriteLine("Islemedi");
                }

                Console.WriteLine("PDF conversion completed successfully");
                return task.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during PDF conversion: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }



        public static string GenerateQRCodeHtml(string graduateName)
        {
            return $@"<!DOCTYPE html>
<html lang=""az"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""color-scheme"" content=""light only"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Roboto+Flex:opsz,wght@8..144,100..1000&display=swap"" rel=""stylesheet"">
    <title>Məzun 2025 Dəvətnaməsi</title>
    <style>


        .email-container {{
            width: 500px;text-align: center;
            height: 700px;
            margin: 0 auto;
            background-image: url('https://i.ibb.co/TBx6cXFk/invitation-card-3x-2.png');
            background-size: contain;
            background-repeat: no-repeat;
            background-position: center top;
            position: relative;
        }}
    
        table {{
            width: 100%;
            height: 100%;
            position: relative;
            z-index: 1;
        }}

        .qr-code img {{
            width: 100px;
            height: 100px;
            display: block;
            margin: 0 auto 40px auto;
            border-radius: 10px;
            border: 2px solid #f7ddaf;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <table cellspacing=""0"" cellpadding=""0"">
            <tr>
                <td style=""color:#134865; font-size: 18px; font-weight: bold; padding-top: 300px;"">{graduateName}</td>
            </tr>
            <tr>
                <td style=""color:#134865; font-size: 14px; padding-top: 10px;"">
                    Səni təbrik edirik və uğurlarının davamını diləyirik.<br>
                    Məzuniyyət günündə səni aramızda görməkdən şad olarıq!
                </td>
            </tr>
            <tr>
                <td style=""color:#134865; font-size: 16px; padding-top: 5px;"">
                    Hörmətlə, Bakı Dövlət Universiteti
                </td>
            </tr>
            <tr>
                <td style=""color:#134865; font-size: 12px; padding-top: 5px;letter-spacing:0.5px;"">
                    Qeyd: QR Kodu məkana yaxınlaşdığınız zaman göstərməyi unutmayın.<br>
                    Məkan, tarix və saat öncədən bildiriləcəkdir.
                </td>
            </tr>
            <tr>
                <td class=""qr-code"">
                    <img src=""cid:qrcode"" alt=""QR Kod"">
                </td>
            </tr>
        </table>
    </div>
</body>
<style>
          body {{
        font-family: ""Roboto Flex"", sans-serif;
        font-optical-sizing: auto;
        font-weight: 400;
        font-style: normal;
        font-variation-settings:
            ""slnt"" 0,
            ""wdth"" 100,
            ""GRAD"" 0,
            ""XOPQ"" 96,
            ""XTRA"" 468,
            ""YOPQ"" 79,
            ""YTAS"" 750,
            ""YTDE"" -203,
            ""YTFI"" 738,
            ""YTLC"" 514,
            ""YTUC"" 712;
    text-align: center;
            letter-spacing: 0.2px;

    margin: 0;
    padding: 0;
    width: 100%;
}}
</style>
</html>
";
        }

    }
}
