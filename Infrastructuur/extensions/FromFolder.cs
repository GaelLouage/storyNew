using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.extensions
{
    public static class FromFolder
    {
        public static string UploadImage(this IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(@"wwwroot\Images", fileName).Replace("\\", "/");
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                 file.CopyTo(fileStream);
            }
            return filePath.Replace("wwwroot", "").Replace("/Images","Images");
        }
    }
}
