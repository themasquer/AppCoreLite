namespace AppCoreLite.Models
{
    public class FileBrowserItemModel
    {
        public string? Name { get; set; }
        public string? Folders { get; set; }
        public bool IsFile { get; set; }
    }

    public class HierarchicalDirectoryModel
    {
        public string? Path { get; set; }
        public byte Level { get; set; }
        public string? Name { get; set; }
    }

    public class HierarchicalDirectoryHtmlModel : HierarchicalDirectoryModel
    {
        public bool IsLiTagCurrent { get; set; }
        public bool IsUlTagHidden { get; set; }
        public string? Link { get; set; }
    }
}
