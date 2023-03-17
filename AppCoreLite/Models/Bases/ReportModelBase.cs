using AppCoreLite.Records.Bases;

namespace AppCoreLite.Models.Bases
{
    public abstract class ReportModelBase : ISoftDelete
    {
        public bool? IsDeleted { get; set; }
    }
}
