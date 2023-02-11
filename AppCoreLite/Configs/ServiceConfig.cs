using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class ServiceConfig : IConfig
    {
        public string ChangesNotSaved { get; set; }
        public string AddedSuccessfuly { get; set; }
        public string UpdatedSuccessfuly { get; set; }
        public string DeletedSuccessfuly { get; set; }
        public string RecordFound { get; set; }
        public string RecordsFound { get; set; }
        public string RecordNotFound { get; set; }
        public string OperationFailed { get; set; }
        public string All { get; set; }
        public List<string> RecordsPerPageCounts { get; set; }
        public string PageOrderFilterSessionKey { get; set; }
        public string InvalidFileExtensionOrFileLength { get; set; }
        public string RelatedRecordsFound { get; set; }
        public string RelatedRecordsDeletedSuccessfully { get; set; }

        public ServiceConfig()
        {
            ChangesNotSaved = "Değişiklikler kaydedilmedi!";
            AddedSuccessfuly = "Kayıt başarıyla eklendi.";
            UpdatedSuccessfuly = "Kayıt başarıyla güncellendi.";
            DeletedSuccessfuly = "Kayıt başarıyla silindi.";
            RecordFound = "Kayıt bulundu.";
            RecordsFound = "Kayıt bulundu.";
            RecordNotFound = "Kayıt bulunamadı.";
            OperationFailed = "İşlem gerçekleştirilemedi!";
            All = "Tümü";
            RecordsPerPageCounts = new List<string>() { "5", "10", "25", "50", "100", All };
            PageOrderFilterSessionKey = "PageOrderFilter";
            InvalidFileExtensionOrFileLength = "Geçersiz dosya uzantısı veya boyutu!";
            RelatedRecordsFound = "İlişkili kayıtlar bulundu.";
            RelatedRecordsDeletedSuccessfully = "İlişkili kayıtlar başarıyla silindi.";
        }

        public void Set(Languages language)
        {
            ChangesNotSaved = language == Languages.Turkish ? "Değişiklikler kaydedilmedi!" : "Changes not saved!";
            AddedSuccessfuly = language == Languages.Turkish ? "Kayıt başarıyla eklendi." : "Record added successfuly.";
            UpdatedSuccessfuly = language == Languages.Turkish ? "Kayıt başarıyla güncellendi." : "Record updated successfuly.";
            DeletedSuccessfuly = language == Languages.Turkish ? "Kayıt başarıyla silindi." : "Record deleted successfuly.";
            RecordFound = language == Languages.Turkish ? "Kayıt bulundu." : "Record found.";
            RecordsFound = language == Languages.Turkish ? "Kayıt bulundu." : "Records found.";
            RecordNotFound = language == Languages.Turkish ? "Kayıt bulunamadı." : "Record not found.";
            OperationFailed = language == Languages.Turkish ? "İşlem gerçekleştirilemedi!" : "Operation failed!";
            All = language == Languages.Turkish ? "Tümü" : "All";
            RecordsPerPageCounts[RecordsPerPageCounts.Count - 1] = All;
            InvalidFileExtensionOrFileLength = language == Languages.Turkish ? "Geçersiz dosya uzantısı veya boyutu!" : "Invalid file extension or length!";
            RelatedRecordsFound = language == Languages.Turkish ? "İlişkili kayıtlar bulundu." : "Related records found.";
            RelatedRecordsDeletedSuccessfully = language == Languages.Turkish ? "İlişkili kayıtlar başarıyla silindi." : "Related records deleted successfully.";
		}
    }
}
