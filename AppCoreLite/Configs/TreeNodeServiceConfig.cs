using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class TreeNodeServiceConfig : IConfig
    {
        public string? NodeDetailFound { get; set; }
        public string? NodeDetailTextRequired { get; set; }
        public string? NodeNameRequired { get; set; }
        public string? NodeFound { get; set; }
        public string? NodeNotFound { get; set; }
        public string? NodeDetailNotFound { get; set; }
        public string? NoUpdateForRootNodeActive { get; set; }
        public string? NoDeleteForRootNode { get; set; }
        public string? NoDeleteForNodeDetailHasNodes { get; set; }
        public string? NodeDetailDeleted { get; set; }

        public TreeNodeServiceConfig()
        {
            NodeDetailFound = "Nod detayı bulundu.";
            NodeDetailTextRequired = "Nod yazısı zorunludur.";
            NodeNameRequired = "Nod adı zorunludur.";
            NodeFound = "Nod bulundu.";
            NodeNotFound = "Nod bulunamadı.";
            NodeDetailNotFound = "Nod detayı bulunamadı.";
            NoUpdateForRootNodeActive = "Kök nod aktif özelliği güncellenemez.";
            NoDeleteForRootNode = "Kök nod silinemez.";
            NoDeleteForNodeDetailHasNodes = "Nod detayı silinemez çünkü nod detayının ilişkili nodları bulunmaktadır.";
            NodeDetailDeleted = "Nod detayı başarıyla silindi.";
        }

        public virtual void Set(Languages language)
        {
            NodeDetailFound = language == Languages.Turkish ? "Nod detayı bulundu." : "Node detail found.";
            NodeDetailTextRequired = language == Languages.Turkish ? "Nod detayı yazısı zorunludur." : "Node detail text is required.";
            NodeNameRequired = language == Languages.Turkish ? "Nod adı zorunludur." : "Node name is required.";
            NodeFound = language == Languages.Turkish ? "Nod bulundu." : "Node found.";
            NodeNotFound = language == Languages.Turkish ? "Nod bulunamadı." : "Node not found.";
            NodeDetailNotFound = language == Languages.Turkish ? "Nod detayı bulunamadı." : "Node detail not found.";
            NoUpdateForRootNodeActive = language == Languages.Turkish ? "Kök nod aktif özelliği güncellenemez." : "Root node active property cannot be updated.";
            NoDeleteForRootNode = language == Languages.Turkish ? "Kök nod silinemez." : "Root node cannot be deleted.";
            NoDeleteForNodeDetailHasNodes = language == Languages.Turkish ? "Nod detayı silinemez çünkü nod detayının ilişkili nodları bulunmaktadır." : "Node detail cannot be deleted because node detail has relational nodes.";
            NodeDetailDeleted = language == Languages.Turkish ? "Nod detayı başarıyla silindi." : "Node detail deleted successfully.";
        }
    }
}
