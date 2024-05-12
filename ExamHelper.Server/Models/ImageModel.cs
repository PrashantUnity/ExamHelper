namespace ExamHelper.Server.Models
{
    public class ImageModel
    {
        public string FileName { get; set; }
        public string Url { get; set; }

        public byte[] ImageData { get; set; }

        public string ImageText { get; set; } = string.Empty;
    }
}