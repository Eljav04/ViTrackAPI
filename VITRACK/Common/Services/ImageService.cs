
using VITRACK.Common.Helpers;

namespace BIEML.Core.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string?> SaveImageAsync(IFormFile image, string folderName)
        {
            if (image == null || image.Length == 0)
                return null;

            string curdir = Directory.GetCurrentDirectory();
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"uploads/{folderName}");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{Guid.NewGuid().ToString("N").Substring(0, 15)}_{TimeHelper.GetBakuTime()}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/uploads/{folderName}/{uniqueFileName}";
        }

    }
}