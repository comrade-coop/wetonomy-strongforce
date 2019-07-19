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
			ITokenPicker customPicker = null)
			: base(hash, target)
		{
			NonPositiveTokenAmountException.RequirePositiveAmount(amount);

			this.Amount = amount;
			this.CustomPicker = customPicker;
		}

		public BigInteger Amount { get; }

		public ITokenPicker CustomPicker { get; }
	}
}