using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class ReportManagerConfig : IConfig
    {
        public string RecordFound { get; set; }
        public string RecordsFound { get; set; }
        public string RecordNotFound { get; set; }
        public string All { get; set; }
        public List<string> RecordsPerPageCounts { get; set; }
        
        public ReportManagerConfig()
        {
            RecordFound = "Kayıt bulundu.";
            RecordsFound = "Kayıt bulundu.";
            RecordNotFound = "Kayıt bulunamadı.";
            All = "Tümü";
            RecordsPerPageCounts = new List<string>() { "5", "10", "25", "50", "100", All };
        }

        public void Set(Languages language)
        {
            RecordFound = language == Languages.Turkish ? "Kayıt bulundu." : "Record found.";
            RecordsFound = language == Languages.Turkish ? "Kayıt bulundu." : "Records found.";
            RecordNotFound = language == Languages.Turkish ? "Kayıt bulunamadı." : "Record not found.";
            All = language == Languages.Turkish ? "Tümü" : "All";
            RecordsPerPageCounts[RecordsPerPageCounts.Count - 1] = All;
        }
    }
}
