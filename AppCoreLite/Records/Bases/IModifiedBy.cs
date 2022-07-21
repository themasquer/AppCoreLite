namespace AppCoreLite.Records.Bases
{
    public interface IModifiedBy
    {
        DateTime? CreateDate { get; set; }
        string? CreatedBy { get; set; }
        DateTime? UpdateDate { get; set; }
        string? UpdatedBy { get; set; }
    }
}
