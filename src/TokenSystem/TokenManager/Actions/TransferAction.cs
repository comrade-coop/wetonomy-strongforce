// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.TokenManager.Actions
{
	public class TransferAction<TTokenTagType> : Action
	{
		public TransferAction(
			string hash,
			Address target,
			BigInteger amount,
			Address from,
			Address to,
			ITokenPicker<TTokenTagType> customPicker = null)
			: base(hash, target)
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