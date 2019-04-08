using System.Collections.Generic;
using ContractsCore;
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

        public TokenSplitter(IList<Address> recipients, TokenManager tokenManager, ISplitStrategy splitStrategy)
            : base(recipients)
        {
            this.tokenManager = tokenManager;
            this.splitStrategy = splitStrategy;
        }

        protected void Split()
        {
            this.splitStrategy.Split(this.Recipients, this.tokenManager);
        }
    }
}