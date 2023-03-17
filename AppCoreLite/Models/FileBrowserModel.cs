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
        public string? Link { get; set; }
        public byte Level { get; set; }
    }
}
