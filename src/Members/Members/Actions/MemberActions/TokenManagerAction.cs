using ContractsCore;
using ContractsCore.Actions;

namespace Members.Actions
{
	public abstract class TokenManagerAction : Action
	{
		public Address TokenManager;

		public TokenManagerAction(string hash, Address target, Address tokenManager) : base(hash, target)
		{
			this.TokenManager = tokenManager;
		}
	}
}