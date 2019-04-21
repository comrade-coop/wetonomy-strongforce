// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensTransferredEventArgs<TTagType> : EventArgs
	{
		public TokensTransferredEventArgs(
			BigInteger amount,
			IReadOnlyTaggedTokens<TTagType> tokens,
			Address from,
			Address to)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.From = from;
			this.To = to;
		}

		public BigInteger Amount { get; }

		public IReadOnlyTaggedTokens<TTagType> Tokens { get; }

		public Address From { get; }

		public Address To { get; }
	}
}