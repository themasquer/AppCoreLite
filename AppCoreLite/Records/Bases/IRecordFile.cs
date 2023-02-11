namespace AppCoreLite.Records.Bases
{
    public interface IRecordFile
    {
        byte[]? FileData { get; set; }
        string? FileContent { get; set; }
        string? FilePath { get; set; }
    }
}
