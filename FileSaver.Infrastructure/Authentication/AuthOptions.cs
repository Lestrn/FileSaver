namespace FileSaver.Infrastructure.Authentication
{
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    public class AuthOptions
    {
        public string? Issuer { get; private set; }

        public string? Audience { get; private set; }

        public int Lifetime { get; private set; } // Minutes

        private readonly string? key;

        public AuthOptions(IConfiguration config)
        {
            this.Issuer = config["JwtSettings:Issuer"];
            this.Audience = config["JwtSettings:Audience"];
            this.Lifetime = int.Parse(config["JwtSettings:Lifetime"]);
            this.key = config["JwtSettings:Key"];
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        }
    }
}
