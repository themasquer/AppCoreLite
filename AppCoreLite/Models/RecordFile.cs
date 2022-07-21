using AppCoreLite.Records.Bases;

namespace AppCoreLite.Models
{
    public class RecordFile : IRecordFile
    {
        public byte[]? FileData { get; set; }
        public string? FileContent { get; set; }
        public string? FilePath { get; set; }
    }
}
