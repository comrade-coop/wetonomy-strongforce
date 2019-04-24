// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensMintedEventArgs<TTagType> : EventArgs
	{
		public TokensMintedEventArgs(BigInteger amount, IReadOnlyTaggedTokens<TTagType> tokens, Address to)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.To = to;
		}

		public BigInteger Amount { get; }

		public IReadOnlyTaggedTokens<TTagType> Tokens { get; }

		public Address To { get; }
	}
}