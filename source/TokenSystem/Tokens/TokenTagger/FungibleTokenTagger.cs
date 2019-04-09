using System.Collections.Generic;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public class FungibleTokenTagger : ITokenTagger
    {
        public const string DefaultTokenTag = "FungibleToken";

        public IDictionary<string, decimal> Tag(Address owner, decimal amount, ITagProperties tagProperties = null)
        {
            if (amount <= 0)
            {
                throw new NonPositiveTokenAmountException(amount);
            }

            return new SortedDictionary<string, decimal> {[DefaultTokenTag] = amount};
        }


        public IDictionary<string, decimal> Pick(IDictionary<string, decimal> tokens, decimal amount,
            ITagProperties tagProperties = null)
        {
            if (amount < 0)
            {
                throw new NonPositiveTokenAmountException(amount);
            }
            
            if (tokens[DefaultTokenTag] < amount)
            {
                throw new InsufficientTokenAmountException(tokens[DefaultTokenTag], amount);
            }

            var pickedTokens = new SortedDictionary<string, decimal> {[DefaultTokenTag] = amount};
            return pickedTokens;
        }

        public IDictionary<string, decimal> Pick(IDictionary<string, decimal> tokens,
            ITagProperties tagProperties = null)
            => tokens;
    }
}