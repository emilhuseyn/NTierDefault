using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Business.Services.ExternalServices.Interfaces
{
    public interface IFileManagerService
    {
        bool BeAValidImage(IFormFile file);
        Task<string> UploadLocalAsync(IFormFile file, string folderName, string _webHostEnvironment);
    }
}
