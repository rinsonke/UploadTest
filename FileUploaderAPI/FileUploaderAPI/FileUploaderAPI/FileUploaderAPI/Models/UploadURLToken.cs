namespace FileUploaderAPI.Models
{
    public class UploadURLToken
    {
        public int Id { get; set; } = 0;
        public string Token { get; set; } = "NULLTOKEN";
        public bool  IsExpired { get; set; }
    }
}
