// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Exceptions;
using Action = ContractsCore.Actions.Action;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class MintAction : Action
	{
		public MintAction(
			string hash,
			Address target,
			BigInteger amount)
			: base(hash, target)
		{
			NonPositiveTokenAmountException.RequirePositiveAmount(amount);

			this.Amount = amount;
		}

		public BigInteger Amount { get; }
	}
}