// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;
using Action = ContractsCore.Actions.Action;

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
			if (from == null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			NonPositiveTokenAmountException.RequirePositiveAmount(amount);

			this.Amount = amount;
			this.From = from;
			this.CustomPicker = customPicker;
		}

		public BigInteger Amount { get; }

		public Address From { get; }

		public ITokenPicker CustomPicker { get; }
	}
}