using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Infrastructure.Authentication
{
    public class AuthOptions
    {
        public string Issuer { get; private set; }
        public string Audience { get; private set; }
        public int Lifetime { get; private set; } // Minutes
        private readonly string _key;
        public AuthOptions(IConfiguration config)
        {
            Issuer = config["JwtSettings:Issuer"];
            Audience = config["JwtSettings:Audience"];
            Lifetime = int.Parse(config["JwtSettings:Lifetime"]);
            _key = config["JwtSettings:Key"];
        }
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
        }
    }
}
