using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
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
