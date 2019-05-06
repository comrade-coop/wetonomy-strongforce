// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensMintedEventArgs : EventArgs
	{
		public TokensMintedEventArgs(IReadOnlyTaggedTokens tokens, Address to)
		{
			this.Tokens = tokens;
			this.To = to;
		}

		public IReadOnlyTaggedTokens Tokens { get; }

		public Address To { get; }
	}
}