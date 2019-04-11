using System.Collections.Generic;
using ContractsCore;
using TokenSystem.TokenManager;

namespace TokenSystem.TokenFlow
{
	public interface ISplitStrategy<TTokenTagType>
	{
		void Split(IList<Address> recipients, ITokenManager<TTokenTagType> tokenManager);
	}
}