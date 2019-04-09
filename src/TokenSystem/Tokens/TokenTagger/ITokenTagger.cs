using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public interface ITokenTagger<T>
    {
        TaggedTokens<T> Tag(Address owner, decimal amount, object tagProps = null);

        TaggedTokens<T> Pick(TaggedTokens<T> tokens, decimal amount, object tagProps = null);

        TaggedTokens<T> Pick(TaggedTokens<T> tokens, object tagProps = null);
    }
}