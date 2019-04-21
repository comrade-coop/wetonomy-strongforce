// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;

namespace TokenSystem.Tokens
{
	public interface ITaggedTokens<TTagType> : IReadOnlyTaggedTokens<TTagType>
	{
		void AddToBalance(TTagType tag, BigInteger amount);

		void AddToBalance(IReadOnlyTaggedTokens<TTagType> tokens);

		void RemoveFromBalance(TTagType tag, BigInteger amount);

		void RemoveFromBalance(IReadOnlyTaggedTokens<TTagType> tokens);
	}
}