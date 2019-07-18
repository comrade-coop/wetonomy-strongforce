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
			BigInteger amount,
			Address to)
			: base(hash, target)
		{
			if (to == null)
			{
				throw new ArgumentNullException(nameof(to));
			}

			NonPositiveTokenAmountException.RequirePositiveAmount(amount);

			this.Amount = amount;
			this.To = to;
		}

		public BigInteger Amount { get; }

		public Address To { get; }
	}
}