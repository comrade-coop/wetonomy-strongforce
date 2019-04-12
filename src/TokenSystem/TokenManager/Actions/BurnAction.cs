using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.TokenManager.Actions
{
	public class BurnAction<TTokenTagType> : Action
	{
		public BurnAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			decimal amount,
			Address from,
			ITokenPicker<TTokenTagType> customPicker = null)
			: base(hash, origin, sender, target)
		{
			this.Amount = amount;
			this.From = from;
			this.CustomPicker = customPicker;
		}

		public decimal Amount { get; }

		public Address From { get; }

		public ITokenPicker<TTokenTagType> CustomPicker { get; }
	}
}