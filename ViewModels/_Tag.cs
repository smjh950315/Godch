namespace Godch.ViewModels
{
    public class sTag : ViewBase
    {
        public int Id { get; set; }
        public string? TagName { get; set; }
    }
    public class TagSelection
    {
        public List<dynamic> AvailableTags { get; set; }
        public List<dynamic> SelectedTags { get; set; }
        public TagSelection()
        {
            AvailableTags = new List<dynamic>();
            SelectedTags = new List<dynamic>();
        }
    }
}
