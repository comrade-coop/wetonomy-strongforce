using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.TokenManager;

namespace TokenSystem.TokenFlow
{
	public class UniformSplitStrategy : ISplitStrategy<string>
	{
		public void Split(IList<Address> recipients, ITokenManager<string> tokenManager)
		{
			throw new System.NotImplementedException();
		}
	}
}