using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.Tokens
{
	public class TransferAction : Action
	{
		public TransferAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			decimal amount,
			Address from,
			Address to,
			ITaggedTokenPickStrategy pickStrategy = null)
			: base(hash, origin, sender, target)
		{
			this.Amount = amount;
			this.From = from;
			this.To = to;
			this.PickStrategy = pickStrategy;
		}

		public decimal Amount { get; }

		public Address From { get; }

		public Address To { get; }

		public ITaggedTokenPickStrategy PickStrategy { get; }
	}
}