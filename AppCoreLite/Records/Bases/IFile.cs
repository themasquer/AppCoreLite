namespace AppCoreLite.Records.Bases
{
    public interface IFile
    {
        public byte[]? FileData { get; set; }
        public string? FileContent { get; set; }
        public string? FilePath { get; set; }
    }
}
