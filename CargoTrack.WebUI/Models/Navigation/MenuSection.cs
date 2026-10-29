namespace CargoTrack.WebUI.Models.Navigation
{
    public class MenuSection
    {
        public string Title { get; set; }
        public List<MenuItem> Items { get; set; } = new();
    }

    public class MenuItem
    {
        public MenuItem(string text, string icon, string url)
        {
            Text = text;
            Icon = icon;
            Url = url;
        }

        public string Text { get; }
        public string Icon { get; }
        public string Url { get; }
        public bool IsActive { get; set; }
    }
}
