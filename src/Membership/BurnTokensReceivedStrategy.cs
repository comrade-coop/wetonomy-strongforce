using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.TokenManagerBase.Actions;

namespace Membership
{
	public class BurnTokensReceivedStrategy : ITokensReceivedStrategy
	{
		public IEnumerable<Action> Execute(
			BigInteger amount,
			Address tokenManager,
			object tag = null)
		{
			var action = new BurnAction(string.Empty, tokenManager, amount);
			return new List<Action>() { action };
		}
	}
}