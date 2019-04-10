using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public interface ISplitStrategy<TTokenTagType>
	{
		void Split(IList<Address> recipients, ITokenManager<TTokenTagType> tokenManager);
	}
}