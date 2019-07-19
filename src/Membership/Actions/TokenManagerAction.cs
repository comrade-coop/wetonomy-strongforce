using ContractsCore;
using ContractsCore.Actions;

namespace Membership.Actions
{
	public abstract class TokenManagerAction : Action
	{
		public TokenManagerAction(
			string hash,
			Address target,
			Address tokenManager)
			: base(hash, target)
		{
			this.TokenManager = tokenManager;
		}

		public Address TokenManager { get; }
	}
}