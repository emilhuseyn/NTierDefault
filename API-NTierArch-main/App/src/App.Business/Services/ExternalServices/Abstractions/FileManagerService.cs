using App.Business.Services.ExternalServices.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Business.Services.ExternalServices.Abstractions
{
    public class FileManagerService : IFileManagerService
    {
        private readonly IWebHostEnvironment _environment;

        public FileManagerService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public bool BeAValidImage(IFormFile file)
        {
            return file != null && file.ContentType.Contains("image") && 1024 * 1024 * 5 >= file.Length;
        }

        

        public async Task<string> UploadLocalAsync(IFormFile file,string folderName,string _webHostEnvironment)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment, "Uploads", folderName);
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/Uploads/{folderName}/{uniqueFileName}"; // Return relative path
        }
    }
}
