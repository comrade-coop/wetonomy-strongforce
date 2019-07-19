using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;

namespace Membership
{
	public interface ITokensReceivedStrategy
	{
		IEnumerable<Action> Execute(BigInteger amount, Address tokenManager, Address from, object tag = null);
	}
}