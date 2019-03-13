using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
    public interface ITokenTagger
    {
        IDictionary<string, decimal> Tag(Address owner, decimal amount, ITagProperties tagProperties = null);

        IDictionary<string, decimal> Pick(IDictionary<string, decimal> tokens,
            decimal amount,
            ITagProperties tagProperties = null);

        IDictionary<string, decimal> Pick(IDictionary<string, decimal> tokens, ITagProperties tagProperties = null);
    }
}