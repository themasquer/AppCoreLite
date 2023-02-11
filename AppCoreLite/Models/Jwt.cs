namespace AppCoreLite.Models
{
    public class Jwt
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }

    public class JwtOptions
    {
        public static string Audience { get; set; }
        public static string Issuer { get; set; }
        public static int ExpirationInMinutes { get; set; }
        public static string SecurityKey { get; set; }
        public static bool Exists { get; set; } = false;

        public JwtOptions()
        {
            Exists = true;
        }
    }
}
