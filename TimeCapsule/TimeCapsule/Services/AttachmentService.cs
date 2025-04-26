//using TimeCapsule.Models.Dto;

//namespace TimeCapsule.Services
//{
//    public class AttachmentService
//    {
//        private readonly IWebHostEnvironment _environment;
//        private readonly string _uploadsFolder;

//        public AttachmentService(IWebHostEnvironment environment)
//        {
//            _environment = environment;
//            _uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

//            if (!Directory.Exists(_uploadsFolder))
//            {
//                Directory.CreateDirectory(_uploadsFolder);
//            }
//        }

//        public async Task<List<UploadedImageDto>> SaveImages(List<IFormFile> files)
//        {
//            var attachments = new List<UploadedImageDto>();

//            foreach (var file in files)
//            {
//                if (file.Length > 0 && file.ContentType.StartsWith("image/"))
//                {
//                    var attachment = await SaveFile(file);
//                    attachments.Add(attachment);
//                }
//            }

//            return attachments;
//        }

//        private async Task<UploadedImageDto> SaveFile(IFormFile file)
//        {
//            // Generuje unikalną nazwę pliku
//            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
//            string filePath = Path.Combine(_uploadsFolder, fileName);

//            // Zapisuje plik na dysku
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            var attachmentDto = new UploadedImageDto
//            {
//                FileName = Path.GetFileName(file.FileName),
//                FilePath = filePath,
//                FileUrl = $"/uploads/{fileName}",
//                ContentType = file.ContentType,
//                FileSize = file.Length,
//                UploadedAt = DateTime.Now
//            };

//            return attachmentDto;
//        }
//    }
//}