using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AppCoreLite.Utilities
{
    public class SecurityUtil
    {
        public SecurityKey CreateSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

        public SigningCredentials CreateSigningCredentials(string securityKey, string securityAlgorithm = SecurityAlgorithms.HmacSha256Signature)
        {
            var key = CreateSecurityKey(securityKey);
            return new SigningCredentials(key, securityAlgorithm);
        }
    }
}
