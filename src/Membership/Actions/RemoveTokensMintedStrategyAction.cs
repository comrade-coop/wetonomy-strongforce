using ContractsCore;
using ContractsCore.Actions;

namespace Membership.Actions
{
	public class RemoveTokensMintedStrategyAction : TokenManagerAction
	{
		public RemoveTokensMintedStrategyAction(string hash, Address target, Address tokenManager)
			: base(hash, target, tokenManager)
		{
		}
	}
}