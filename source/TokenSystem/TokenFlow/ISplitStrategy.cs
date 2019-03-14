using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
    public interface ISplitStrategy
    {
        void Split(IList<Address> recipients, ITokenManager tokenManager);
    }
}