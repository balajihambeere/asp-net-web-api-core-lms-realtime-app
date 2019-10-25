namespace LearningApiCore.Infrastructure.Security
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Threading.Tasks;
    public class JwtIssuerOptions
    {
        public const string Scheme = "Token";

        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string Audience { get; set; }

        public DateTime NotBefore => DateTime.UtcNow;//DateTime.UtcNow;

        public DateTime IssuedAt => DateTime.UtcNow;//DateTime.UtcNow;

        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(60);

        public DateTime Expiration => IssuedAt.Add(ValidFor);

        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());

        public SigningCredentials SigningCredentials { get; set; }
    }
}
