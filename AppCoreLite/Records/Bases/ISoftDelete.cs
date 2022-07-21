namespace AppCoreLite.Records.Bases
{
    public interface ISoftDelete
    {
        public bool? IsDeleted { get; set; }
    }
}
