// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class BurnAction : Action
	{
		public BurnAction(
			string hash,
			Address target,
			BigInteger amount,
			Address from,
			ITokenPicker customPicker = null)
			: base(hash, target)
		{
			this.Amount = amount;
			this.From = from;
			this.CustomPicker = customPicker;
		}

		public BigInteger Amount { get; }

		public Address From { get; }

		public ITokenPicker CustomPicker { get; }
	}
}