using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SMS_Gem.Security
{
    public static class JwtTokenHelper
    {
        private static readonly string Secret = ConfigurationManager.AppSettings["Jwt.Secret"];
        private static readonly string Issuer = ConfigurationManager.AppSettings["Jwt.Issuer"];
        private static readonly string Audience = ConfigurationManager.AppSettings["Jwt.Audience"];
        private static readonly int ExpiryMinutes = int.TryParse(ConfigurationManager.AppSettings["Jwt.ExpiryMinutes"], out int exp) ? exp : 120;

        public static string GenerateToken(string userId, string branch, string role = "User")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId ?? "SYSTEM"),
                    new Claim(ClaimTypes.Name, userId ?? "SYSTEM"),
                    new Claim("Branch", branch ?? "CAP"),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(ExpiryMinutes),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null; // Invalid signature, expired, or malformed
            }
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace SMS_Gem.DAL
//{
//    public class JwtTokenHelper : ApiController
//    {
//        // GET api/<controller>
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET api/<controller>/5
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST api/<controller>
//        public void Post([FromBody] string value)
//        {
//        }

//        // PUT api/<controller>/5
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE api/<controller>/5
//        public void Delete(int id)
//        {
//        }
//    }
//}