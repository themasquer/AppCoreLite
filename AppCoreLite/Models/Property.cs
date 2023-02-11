namespace AppCoreLite.Models
{
    public class Property
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public Property(string name, string displayName = "")
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}
