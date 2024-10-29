using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class Identity
    {
        public string type;
        public string iss;
        public string sub;
        public string email;

        public Identity(string type, string iss, string sub, string email)
        {
            this.type = type;
            this.iss = iss;
            this.sub = sub;
            this.email = email;
        }
    }
}