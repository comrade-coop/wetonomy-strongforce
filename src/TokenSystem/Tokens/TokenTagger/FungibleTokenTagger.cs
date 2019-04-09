using System.Collections.Generic;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public class FungibleTokenTagger : ITokenTagger<string>
    {
        public const string DefaultTokenTag = "FungibleToken";

        public TaggedTokens<string> Tag(Address owner, decimal amount, object tagProps = null)
        {
            if (amount <= 0)
            {
                throw new NonPositiveTokenAmountException(amount);
            }

            return new TaggedTokens<string> { [DefaultTokenTag] = amount };
        }


        public TaggedTokens<string> Pick(TaggedTokens<string> tokens, decimal amount,
            object tagProps = null)
        {
            if (amount < 0)
            {
                throw new NonPositiveTokenAmountException(amount);
            }

            if (tokens[DefaultTokenTag] < amount)
            {
                throw new InsufficientTokenAmountException(tokens[DefaultTokenTag], amount);
            }

            var pickedTokens = new TaggedTokens<string> { [DefaultTokenTag] = amount };
            return pickedTokens;
        }

        public TaggedTokens<string> Pick(TaggedTokens<string> tokens,
            object tagProps = null)
            => tokens;
    }
}