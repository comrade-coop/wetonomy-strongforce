using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class AddTokensMintedStrategyAction : TokenManagerAction
	{
		public ITokensReceivedStrategy Strategy;

		public AddTokensMintedStrategyAction(string hash, Address target, Address tokenManager, ITokensReceivedStrategy strategy)
			: base(hash, target, tokenManager)
		{
			this.Strategy = strategy;
		}
	}
}