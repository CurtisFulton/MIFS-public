using System;

namespace Mifs.MEX.Authentication
{
    public class MEXJwtToken
    {
        public MEXJwtToken(string jwtString)
        {
            this.TokenString = jwtString;
        }

        // TODO: Set these up properly
        public DateTimeOffset ExpirationDateTime { get; private set; } = DateTimeOffset.Now.AddDays(365);
        public bool IsExpired => false;

        private string TokenString { get; }

        public override string ToString()
            => this.TokenString;
    }
}
