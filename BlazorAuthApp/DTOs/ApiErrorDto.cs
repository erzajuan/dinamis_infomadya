namespace BlazorAuthApp.DTOs
{
    public class ApiErrorDto
    {
        public string Title { get; set; } = "";
        public int Status { get; set; }
        public string Detail { get; set; } = "";
        public string Instance { get; set; } = "";
    }
}