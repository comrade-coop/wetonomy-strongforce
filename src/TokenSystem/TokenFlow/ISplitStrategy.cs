using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
    public interface ISplitStrategy<T>
    {
        void Split(IList<Address> recipients, ITokenManager<T> tokenManager);
    }
}