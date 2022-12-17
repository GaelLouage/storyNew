using Infrastructuur.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Classes
{
    public class FileService : IFileService
    {
        public async Task<string> UploadImage(IFormFile fileImage)
        {
            var fileName = Path.GetFileName(fileImage.FileName);
            var filePath = Path.Combine(@"wwwroot\Images\", fileName).Replace("\\", "/");
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileImage.CopyToAsync(fileStream);
            }
            return filePath.Replace("wwwroot", "");
        }
    }
}
