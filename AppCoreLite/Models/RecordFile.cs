using AppCoreLite.Records.Bases;

namespace AppCoreLite.Models
{
    public class RecordFile : IRecordFile
    {
        public byte[]? FileData { get; set; }
        public string? FileContent { get; set; }
        public string? FilePath { get; set; }
    }

    public class RecordFileToDownload
    {
        public Stream? Stream { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
    }
}
