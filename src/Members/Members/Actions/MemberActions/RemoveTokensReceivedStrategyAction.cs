using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public class RemoveTokensReceivedStrategyAction : TokenManagerAction
	{
		public RemoveTokensReceivedStrategyAction(string hash, Address target, Address tokenManager) : base(hash, target, tokenManager)
		{
		}
	}
}