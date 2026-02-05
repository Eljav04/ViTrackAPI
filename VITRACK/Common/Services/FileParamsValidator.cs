
using VITRACK.Common.Statics;

namespace VITRACK.Common.Services
{
    public class FileParamsValidator
    {
        public bool AllowImage { get; set; } = false;
        public bool AllowDocument { get; set; } = false;
        public bool AllowCompresed { get; set; } = false;
        public bool AllowNullable { get; set; } = false;
        public int MaxFileSize { get; set; } = 1; // Must be in megabites
        public string? ErrorMessage { get; private set; } = null;

        public void AllowAll()
        {
            this.AllowImage = true;
            this.AllowDocument = true;
            this.AllowCompresed = true;
        }

        public bool IsValidFile(IFormFile? file)
        {
            if ((file is null || file.Length == 0))
            {
                if (AllowNullable)
                    return true;
                ErrorMessage = "Fayl seçilməyib";
                return false;
            }

            string fileExtension = Path.GetExtension(file.FileName);

            if (file.Length > 1024 * 1024 * MaxFileSize)
            {
                this.ErrorMessage = $"Faylın ölçüsü {MaxFileSize}mb-dan kiçik olmalıdır";
                return false;
            }

            if (AllowImage && FileExtensions.IsValidImage(fileExtension))
            {
                return true;
            }
            if (AllowDocument && FileExtensions.IsValidDocument(fileExtension))
            {
                return true;
            }
            if (AllowCompresed && FileExtensions.IsValidCompressed(fileExtension))
            {
                return true;
            }

            this.ErrorMessage = "Faylın formatı düzgün deyil";
            return false;
        }

    }
}