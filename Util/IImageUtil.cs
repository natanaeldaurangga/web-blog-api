using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LearnJwtAuth.Util
{
    public interface IImageUtil
    {
        public Task<string> UploadImageAsync(IFormFile file);
        public Task<bool> DeleteImageAsync(string fileName);
        public Task<byte[]> GetImageAsync(string fileName);
    }
}