using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.Tokens
{
	public class BurnAction : Action
	{
		public BurnAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			decimal amount,
			Address from,
			ITaggedTokenPickStrategy pickStrategy = null)
			: base(hash, origin, sender, target)
		{
			this.Amount = amount;
			this.From = from;
			this.PickStrategy = pickStrategy;
		}

		public decimal Amount { get; }

		public Address From { get; }

		public ITaggedTokenPickStrategy PickStrategy { get; }
	}
}