using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class ServiceConfig : IConfig
    {
        public virtual string ChangesNotSaved { get; set; }
        public virtual string AddedSuccessfuly { get; set; }
        public virtual string UpdatedSuccessfuly { get; set; }
        public virtual string DeletedSuccessfuly { get; set; }
        public virtual string RecordFound { get; set; }
        public virtual string RecordsFound { get; set; }
        public virtual string RecordNotFound { get; set; }
        public virtual string OperationFailed { get; set; }
        public virtual string All { get; set; }
        public virtual List<string> RecordsPerPageCounts { get; set; }
        public virtual string PageOrderFilterSessionKey { get; set; }
        public virtual string InvalidFileExtensionOrFileLength { get; set; }
        public virtual string RelatedRecordsFound { get; set; }

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
            RelatedRecordsFound = "Related records found.";
        }
    }
}
