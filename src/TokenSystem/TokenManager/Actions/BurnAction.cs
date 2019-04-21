// Copyright (c) Comrade Coop. All rights reserved.

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
			ITokenPicker<TTokenTagType> pickStrategy = null)
			: base(hash, origin, sender, target)
		{
			this.Amount = amount;
			this.From = from;
			this.PickStrategy = pickStrategy;
		}

		public decimal Amount { get; }

		public Address From { get; }

		public ITokenPicker<TTokenTagType> PickStrategy { get; }
	}
}