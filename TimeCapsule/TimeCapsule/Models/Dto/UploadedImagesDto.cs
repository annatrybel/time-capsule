using System;

namespace TimeCapsule.Models.Dto
{
    public class UploadedImageDto
    {
        public string FileName { get; set; } = string.Empty;
        public string Base64Content { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}