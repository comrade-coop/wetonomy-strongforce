// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;

namespace TokenSystem.Tokens
{
	public interface ITaggedTokens : IReadOnlyTaggedTokens
	{
		void AddToBalance(TokenTagBase tag, BigInteger amount);

		void AddToBalance(IReadOnlyTaggedTokens tokens);

		void RemoveFromBalance(TokenTagBase tag, BigInteger amount);

		void RemoveFromBalance(IReadOnlyTaggedTokens tokens);
	}
}