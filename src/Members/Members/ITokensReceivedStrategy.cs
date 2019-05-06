using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;

namespace Members
{
	public interface ITokensReceivedStrategy
	{
		List<Action> OnTokensReceived(BigInteger amount, Address tokenManager, Address from, object tag = null);
	}
}