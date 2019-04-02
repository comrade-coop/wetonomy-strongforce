using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
    public class UniformSplitStrategy : ISplitStrategy
    {
        public void Split(IList<Address> recipients, ITokenManager tokenManager)
        {
            throw new System.NotImplementedException();
        }
    }
}