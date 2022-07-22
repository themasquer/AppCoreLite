using AppCoreLite.Records.Bases;

namespace AppCoreLite.Models.Bases
{
    public abstract class ReportBase : ISoftDelete
    {
        public bool? IsDeleted { get; set; }
    }
}
