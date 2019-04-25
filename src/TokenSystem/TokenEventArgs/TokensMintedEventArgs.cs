// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensMintedEventArgs<TTagType> : EventArgs
	{
		public TokensMintedEventArgs(IReadOnlyTaggedTokens<TTagType> tokens, Address to)
		{
			this.Tokens = tokens;
			this.To = to;
		}

		public IReadOnlyTaggedTokens<TTagType> Tokens { get; }

		public Address To { get; }
	}
}