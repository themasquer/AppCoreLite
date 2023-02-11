using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class AccountServiceConfig : IConfig
    {
        public string? UserNotFound { get; set; }
        public string? UserFound { get; set; }
        public string? UserRegistered { get; set; }
        public string? RoleNotFound { get; set; }

        public AccountServiceConfig()
        {
            UserNotFound = "Kullanıcı bulunamadı!";
            UserFound = "Kullanıcı bulundu.";
            UserRegistered = "Kullanıcı kaydedildi.";
            RoleNotFound = "Rol bulunamadı!";
        }

        public void Set(Languages language)
        {
            UserNotFound = language == Languages.Turkish ? "Kullanıcı bulunamadı!" : "User not found!";
            UserFound = language == Languages.Turkish ? "Kullanıcı bulundu." : "User found.";
            UserRegistered = language == Languages.Turkish ? "Kullanıcı kaydedildi." : "User registered.";
            RoleNotFound = language == Languages.Turkish ? "Rol bulunamadı!" : "Role not found!";
        }
    }
}
