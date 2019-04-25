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
			IReadOnlyTaggedTokens<TTagType> tokens,
			Address from,
			Address to)
		{
			this.Tokens = tokens;
			this.From = from;
			this.To = to;
		}

		public IReadOnlyTaggedTokens<TTagType> Tokens { get; }

		public Address From { get; }

		public Address To { get; }
	}
}