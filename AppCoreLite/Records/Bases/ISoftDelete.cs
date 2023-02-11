namespace AppCoreLite.Records.Bases
{
    public interface ISoftDelete
    {
        bool? IsDeleted { get; set; }
    }
}
