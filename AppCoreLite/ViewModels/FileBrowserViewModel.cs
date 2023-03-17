using AppCoreLite.Enums;
using AppCoreLite.Models;

namespace AppCoreLite.ViewModels
{
    public class FileBrowserViewModel
    {
        public List<FileBrowserItemModel>? Contents { get; set; }
        public string? Title { get; set; }
        public string? FileContent { get; set; }
        public byte[]? FileBinaryContent { get; set; }
        public FileTypes FileType { get; set; }
        public string? FileContentType { get; set; }
        public string? HierarchicalDirectoryLinks { get; set; }
    }
}
