using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.TokenManager.Actions
{
	public class MintAction : Action
	{
		public MintAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			decimal amount,
			Address to)
			: base(hash, origin, sender, target)
		{
			this.Amount = amount;
			this.To = to;
		}

		public decimal Amount { get; }

		public Address To { get; }
	}
}