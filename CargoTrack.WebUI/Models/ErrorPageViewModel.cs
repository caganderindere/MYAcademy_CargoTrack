namespace CargoTrack.WebUI.Models
{
    public class ErrorPageViewModel
    {
        public int StatusCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public string? RequestId { get; set; }
    }
}
