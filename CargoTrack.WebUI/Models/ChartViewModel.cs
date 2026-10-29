namespace CargoTrack.WebUI.Models
{
    public class ChartViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string? Label { get; set; }
        public bool Horizontal { get; set; }
        public List<string> Labels { get; set; } = new();
        public List<int> Values { get; set; } = new();
        public List<int> Keys { get; set; } = new();
    }
}
