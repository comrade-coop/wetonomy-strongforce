// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Numerics;

namespace TokenSystem.Tokens
{
	public interface ITaggedTokens : IReadOnlyTaggedTokens
	{
		void AddToBalance(IComparable tag, BigInteger amount);

		void AddToBalance(IReadOnlyTaggedTokens tokens);

		void RemoveFromBalance(IComparable tag, BigInteger amount);

		void RemoveFromBalance(IReadOnlyTaggedTokens tokens);
	}
}