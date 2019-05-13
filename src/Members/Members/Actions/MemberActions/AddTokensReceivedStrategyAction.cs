using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class AddTokensReceivedStrategyAction : TokenManagerAction
	{
		public ITokensReceivedStrategy Strategy;

		public AddTokensReceivedStrategyAction(string hash, Address target, Address tokenManager, ITokensReceivedStrategy strategy)
			: base(hash, target, tokenManager)
		{
			this.Strategy = strategy;
		}
	}
}