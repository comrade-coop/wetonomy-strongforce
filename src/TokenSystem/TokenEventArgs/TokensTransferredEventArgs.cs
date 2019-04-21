// Copyright (c) Comrade Coop. All rights reserved.

using System;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensTransferredEventArgs<TTagType> : EventArgs
	{
		public TokensTransferredEventArgs(
			decimal amount,
			IReadOnlyTaggedTokens<TTagType> tokens,
			Address from,
			Address to)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.From = from;
			this.To = to;
		}

		public decimal Amount { get; }

		public IReadOnlyTaggedTokens<TTagType> Tokens { get; }

		public Address From { get; }

		public Address To { get; }
	}
}