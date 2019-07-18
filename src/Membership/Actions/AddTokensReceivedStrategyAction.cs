using ContractsCore;
using ContractsCore.Actions;

namespace Membership.Actions
{
	public class AddTokensReceivedStrategyAction : TokenManagerAction
	{
		public AddTokensReceivedStrategyAction(
			string hash,
			Address target,
			Address tokenManager,
			ITokensReceivedStrategy strategy)
			: base(hash, target, tokenManager)
		{
			this.Strategy = strategy;
		}

		public ITokensReceivedStrategy Strategy { get; }
	}
}