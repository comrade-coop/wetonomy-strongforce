// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class TransferAction : Action
	{
		public TransferAction(
			string hash,
			Address target,
			BigInteger amount,
			Address from,
			Address to,
			ITokenPicker customPicker = null)
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

		public ITokenPicker CustomPicker { get; }
	}
}