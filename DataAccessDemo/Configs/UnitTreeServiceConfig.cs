using AppCoreLite.Configs;
using AppCoreLite.Enums;

namespace DataAccessDemo.Configs
{
    public class UnitTreeServiceConfig : TreeNodeServiceConfig
    {
        public override void Set(Languages language)
        {
            NodeDetailFound = language == Languages.Turkish ? "Pozisyon bulundu." : "Position found.";
            NodeDetailTextRequired = language == Languages.Turkish ? "Pozisyon ünvanı zorunludur." : "Position title is required.";
            NodeNameRequired = language == Languages.Turkish ? "Birim adı zorunludur." : "Unit name is required.";
            NodeFound = language == Languages.Turkish ? "Birim bulundu." : "Unit found.";
            NodeNotFound = language == Languages.Turkish ? "Birim bulunamadı." : "Unit not found.";
            NodeDetailNotFound = language == Languages.Turkish ? "Pozisyon bulunamadı." : "Position not found.";
            NoUpdateForRootNodeActive = language == Languages.Turkish ? "Kök birim aktif özelliği güncellenemez." : "Root unit active property cannot be updated.";
            NoDeleteForRootNode = language == Languages.Turkish ? "Kök birim silinemez." : "Root unit cannot be deleted.";
            NoDeleteForNodeDetailHasNodes = language == Languages.Turkish ? "Pozisyon silinemez çünkü pozisyonun ilişkili birimleri bulunmaktadır." : "Position cannot be deleted because position has relational units.";
            NodeDetailDeleted = language == Languages.Turkish ? "Pozisyon başarıyla silindi." : "Position deleted successfully.";
        }
    }
}
