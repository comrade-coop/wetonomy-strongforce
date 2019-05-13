using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace Members
{
	public class BurnTokensReceivedStrategy : ITokensReceivedStrategy
	{
		public List<Action> OnTokensReceived(BigInteger amount, Address tokenManager, Address from, object tag = null)
		{
			var action = new BurnAction(string.Empty, tokenManager, amount, from);
			return new List<Action>() { action };
		}
	}
}