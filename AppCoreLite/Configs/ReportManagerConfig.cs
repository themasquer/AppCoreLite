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

    public class ExportManagerConfig : IConfig
    {
        public string FileNameWithoutExtension { get; set; }
        public string ExcelWorksheetName { get; set; }
        public bool IsExcelLicenseCommercial { get; set; }

        public ExportManagerConfig()
        {
            FileNameWithoutExtension = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Replace("/", "").Replace(" ", "_").Replace(":", "") + "_Rapor";
            ExcelWorksheetName = "Sayfa1";
            IsExcelLicenseCommercial = false;
        }

        public void Set(Languages language)
        {
            FileNameWithoutExtension = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").Replace("/", "").Replace(" ", "_").Replace(":", "");
            if (language == Languages.Turkish)
                FileNameWithoutExtension += "_Rapor";
            else
                FileNameWithoutExtension += "_Report";
            ExcelWorksheetName = language == Languages.Turkish ? "Sayfa1" : "Sheet1";
        }

        public void Set(bool isExcelLicenseCommercial)
        {
            IsExcelLicenseCommercial = isExcelLicenseCommercial;
        }
    }
}
