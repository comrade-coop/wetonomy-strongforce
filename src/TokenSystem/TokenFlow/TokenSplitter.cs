using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
    public class TokenSplitter<T> : RecipientManager
    {
        private readonly TokenManager<T> tokenManager;
        private readonly ISplitStrategy<T> splitStrategy;

        public TokenSplitter(TokenManager<T> tokenManager, ISplitStrategy<T> splitStrategy) :
            this(new List<Address>(), tokenManager, splitStrategy)
        {
        }

        public TokenSplitter(List<Address> recipients, TokenManager<T> tokenManager, ISplitStrategy<T> splitStrategy)
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