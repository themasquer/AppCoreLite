using AppCoreLite.Extensions;
using AppCoreLite.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppCoreLite.Utilities
{
    public class JwtUtil
    {
        private readonly AppSettingsUtil _appSettingsUtil;
        private readonly SecurityUtil _securityUtil;

        public JwtUtil(IConfiguration configuration, string appSettingsSectionKey = "JwtOptions")
        {
            _appSettingsUtil = new AppSettingsUtil(configuration, appSettingsSectionKey);
            _appSettingsUtil.Bind<JwtOptions>();
            _securityUtil = new SecurityUtil();
        }

        public JwtUtil()
        {
            _securityUtil = new SecurityUtil();
        }

        public Jwt? CreateJwt(User? user, bool includeBearer = true)
        {
            if (!JwtOptions.Exists || user == null || string.IsNullOrWhiteSpace(user.UserName) || user.Roles == null || user.Roles.Count == 0 || string.IsNullOrWhiteSpace(user.Role))
                return null;
            var signingCredentials = _securityUtil.CreateSigningCredentials(JwtOptions.SecurityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
            };
            if (!string.IsNullOrWhiteSpace(user.Guid))
                claims.Add(new Claim(ClaimTypes.Sid, user.Guid));
            var expiration = DateTime.Now.AddTime(0, JwtOptions.ExpirationInMinutes);
            var jwtSecurityToken = new JwtSecurityToken(JwtOptions.Issuer, JwtOptions.Audience, claims, DateTime.Now, expiration, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            var jwt = new Jwt()
            {
                Token = includeBearer ? "Bearer " + token : token,
                Expiration = expiration
            };
            return jwt;
        }

        public SecurityKey CreateSecurityKey(string securityKey)
        {
            return _securityUtil.CreateSecurityKey(securityKey);
        }
    }
}
