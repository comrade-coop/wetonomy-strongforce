using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class RemoveTokensMintedStrategyAction : TokenManagerAction
	{
		public RemoveTokensMintedStrategyAction(string hash, Address target, Address tokenManager) : base(hash, target, tokenManager)
		{
		}
	}
}
