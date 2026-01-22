
namespace VITRACK.Common.Statics
{
    internal static class FileExtensions
    {
        public static readonly List<string> ImageExtensionsList = new()
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".heic", ".heics",
            ".JPG", ".JPEG", ".PNG", ".BMP", ".WEBP", ".HEIC", ".HEICS"
        };

        public static readonly List<string> DocumentExtensionsList = new()
        {
            ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".pdf", ".txt", ".odt", ".ods", ".odp",
            ".DOC", ".DOCX", ".PPT", ".PPTX", ".XLS", ".XLSX", ".PDF", ".TXT", ".ODT", ".ODS", ".ODP"
        };

        public static readonly List<string> CompressedExtensionsList = new()
        {
            ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz",
            ".ZIP", ".RAR", ".7Z", ".TAR", ".GZ", ".BZ2", ".XZ"
        };

        public static bool IsValidImage(string checkExtension)
        {
            if (ImageExtensionsList.Contains(checkExtension))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidDocument(string checkExtension)
        {
            if (DocumentExtensionsList.Contains(checkExtension))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidCompressed(string checkExtension)
        {
            if (CompressedExtensionsList.Contains(checkExtension))
            {
                return true;
            }
            return false;
        }
    }
}