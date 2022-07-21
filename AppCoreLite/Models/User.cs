using AppCoreLite.Records.Bases;

namespace AppCoreLite.Models
{
    public class User : Record
    {
        public string? UserName { get; set; }
        public List<string>? Roles { get; set; }
        public string? Role => Roles?.FirstOrDefault();
    }
}
