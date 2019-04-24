// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensBurnedEventArgs<TTagType> : EventArgs
	{
		public TokensBurnedEventArgs(BigInteger amount, IReadOnlyTaggedTokens<TTagType> tokens, Address from)
		{
			this.Amount = amount;
			this.Tokens = tokens;
			this.From = from;
		}

		public BigInteger Amount { get; }

		public IReadOnlyTaggedTokens<TTagType> Tokens { get; }

		public Address From { get; }
	}
}