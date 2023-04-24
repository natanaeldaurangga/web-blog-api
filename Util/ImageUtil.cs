using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LearnJwtAuth.Util
{
    public class ImageUtil : IImageUtil
    {
        private readonly IWebHostEnvironment _env;

        private readonly string _imagePath = "000files\\Images\\";

        private readonly IConfiguration _configuration;

        public ImageUtil(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return _configuration.GetSection("Image:Default").Value;
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_env.ContentRootPath, _imagePath + fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task<bool> DeleteImageAsync(string fileName)
        {
            var filePath = Path.Combine(_env.ContentRootPath, _imagePath + fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            File.Delete(filePath);
            return true;
        }

        public async Task<byte[]> GetImageAsync(string fileName)
        {
            var filePath = Path.Combine(_env.ContentRootPath, _imagePath + fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            return await File.ReadAllBytesAsync(filePath);
        }


    }
}