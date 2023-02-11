using AppCoreLite.Records.Bases;

namespace AppCoreLite.Models
{
    public class User : Record
    {
        public string? UserName { get; set; }
        public List<string>? Roles { get; set; }
        public string? Role => Roles?.FirstOrDefault();
    }

    public class AccountLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AccountRegister
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
