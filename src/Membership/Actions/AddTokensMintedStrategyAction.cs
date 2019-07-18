using ContractsCore;

namespace Membership.Actions
{
	public class AddTokensMintedStrategyAction : TokenManagerAction
	{
		public AddTokensMintedStrategyAction(
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