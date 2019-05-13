// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenEventArgs
{
	public class TokensBurnedEventArgs : EventArgs
	{
		public TokensBurnedEventArgs(IReadOnlyTaggedTokens tokens, Address from)
		{
			this.Tokens = tokens;
			this.From = from;
		}

		public IReadOnlyTaggedTokens Tokens { get; }

		public Address From { get; }
	}
}