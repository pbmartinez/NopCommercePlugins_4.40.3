using System;

namespace Nop.Plugin.Payments.EnZona.Models
{
    public class Token
    {
        public Token()
        {
            Created = DateTime.Now;
        }
        public bool HasExpired => ExpiresIn <= 30 || TimeSpan.FromTicks(DateTime.Now.Ticks - Created.Ticks).TotalSeconds >= ExpiresIn;

        public DateTime Created { get; set; }

        public string AccessToken { get; set; }

        public string Scope { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }
    }
}
