using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
    public class UniformSplitStrategy : ISplitStrategy<object>
    {
        public void Split(IList<Address> recipients, ITokenManager<object> tokenManager)
        {
            throw new System.NotImplementedException();
        }
    }
}