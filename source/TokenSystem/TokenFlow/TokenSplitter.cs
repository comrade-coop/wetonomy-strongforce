using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
    public class TokenSplitter : RecipientManager
    {
        private readonly TokenManager tokenManager;
        private readonly ISplitStrategy splitStrategy;

        public TokenSplitter(TokenManager tokenManager, ISplitStrategy splitStrategy) :
            this(new List<Address>(), tokenManager, splitStrategy)
        {
        }

        public TokenSplitter(List<Address> recipients, TokenManager tokenManager, ISplitStrategy splitStrategy)
            : base(recipients)
        {
            this.tokenManager = tokenManager;
            this.splitStrategy = splitStrategy;
        }

        protected void Split()
        {
            splitStrategy.Split(Recipients, tokenManager);
        }
    }
}