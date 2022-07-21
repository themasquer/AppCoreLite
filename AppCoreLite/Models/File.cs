using AppCoreLite.Records.Bases;

namespace AppCoreLite.Models
{
    public class File : IFile
    {
        public byte[]? FileData { get; set; }
        public string? FileContent { get; set; }
        public string? FilePath { get; set; }
    }
}
