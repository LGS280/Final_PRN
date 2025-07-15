using DiamondAssessmentSystem.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Helpers
{
    public static class FileHelper
    {
        // Các định dạng được phép upload
        public static readonly string[] AllowedExtensions = new[]
        {
            // Ảnh
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp",

            // Tài liệu
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt",

            // Video
            ".mp4", ".avi", ".mov", ".webm", ".mkv"
        };

        // Xác định loại message dựa trên đuôi file
        public static MessageType GetMessageType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (ImageExtensions.Contains(ext)) return MessageType.Image;
            if (VideoExtensions.Contains(ext)) return MessageType.Video;
            return MessageType.File;
        }

        // Kiểm tra file có được phép không
        public static bool IsAllowed(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedExtensions.Contains(ext);
        }

        // Lấy tên file duy nhất lưu server
        public static string GenerateUniqueFileName(string extension)
        {
            return $"{Guid.NewGuid()}{extension}";
        }

        // Phân loại extensions theo loại
        private static readonly HashSet<string> ImageExtensions = new HashSet<string>
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
        };

        private static readonly HashSet<string> VideoExtensions = new HashSet<string>
        {
            ".mp4", ".avi", ".mov", ".webm", ".mkv"
        };
    }
}
