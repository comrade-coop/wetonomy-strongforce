// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Exceptions;
using Action = ContractsCore.Actions.Action;

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
			if (from == null)
			{
				throw new ArgumentNullException(nameof(to));
			}

			if (to == null)
			{
				throw new ArgumentNullException(nameof(to));
			}

			NonPositiveTokenAmountException.RequirePositiveAmount(amount);

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