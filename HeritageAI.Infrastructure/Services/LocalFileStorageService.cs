using HeritageAI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace HeritageAI.Application.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _webRootPath = env.WebRootPath;
        }

        public async Task<string> SaveProductImageAsync(Stream fileStream, string fileName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var folderPath = Path.Combine(_webRootPath, "uploads", "products");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, uniqueFileName);

            using (var fileOutput = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileOutput);
            }

            return $"/uploads/products/{uniqueFileName}";
        }

        public void DeleteFile(string relativeUrl)
        {
            var fullPath = Path.Combine(_webRootPath, relativeUrl.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
