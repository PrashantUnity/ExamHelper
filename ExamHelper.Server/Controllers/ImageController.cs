using ExamHelper.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace ExamHelper.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImageController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var directoryPath = Path.Combine(_environment.WebRootPath, "uploads");

                // Create the directory if it does not exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { FileName = fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        public IActionResult GetAllImages()
        {
            //var directoryPath = Path.Combine(_environment.ContentRootPath, "uploads"); 
            var directoryPath = "C:\\Users\\Prashant\\Downloads\\Grepper\\ServerForImages\\wwwroot\\uploads";

            if (!Directory.Exists(directoryPath))
            {
                return NotFound("Image directory not found");
            }
            var imagePaths = Directory.GetFiles(directoryPath, "*.png")
                                      .Select(Path.GetFileName);
            var images = new List<ImageModel>();

            foreach (var imagePath in imagePaths)
            {
                var imageUrl = Path.Combine(directoryPath, Path.GetFileName(imagePath)); // Assuming the images are served from a directory named "uploads"
                images.Add(new ImageModel
                {
                    FileName = Path.GetFileName(imagePath),
                    ImageData = System.IO.File.ReadAllBytes(imageUrl),
                    Url = imageUrl,
                    ImageText = GetImageText(imageUrl)
                });
            }

            return Ok(images);
        }

        private static string GetImageText(string path)
        {
            string text = "";
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using var img = Pix.LoadFromFile(path);
                using var page = engine.Process(img);
                text = page.GetText();
            }
            return text;
        }
    }
}
