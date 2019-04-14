using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.TokenManager.Actions
{
	public class TransferAction<TTokenTagType> : Action
	{
		public TransferAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			BigInteger amount,
			Address from,
			Address to,
			ITokenPicker<TTokenTagType> customPicker = null)
			: base(hash, origin, sender, target)
		{
			this.Amount = amount;
			this.From = from;
			this.To = to;
			this.CustomPicker = customPicker;
		}

		public BigInteger Amount { get; }

		public Address From { get; }

		public Address To { get; }

		public ITokenPicker<TTokenTagType> CustomPicker { get; }
	}
}