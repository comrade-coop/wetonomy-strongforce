// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class MintAction : Action
	{
		public MintAction(
			string hash,
			Address target,
			BigInteger amount,
			Address to)
			: base(hash, target)
		{
			this.Amount = amount;
			this.To = to;
		}

		public BigInteger Amount { get; }

		public Address To { get; }
	}
}