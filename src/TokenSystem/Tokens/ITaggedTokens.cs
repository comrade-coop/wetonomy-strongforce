// Copyright (c) Comrade Coop. All rights reserved.

namespace TokenSystem.Tokens
{
	public interface ITaggedTokens<TTagType> : IReadOnlyTaggedTokens<TTagType>
	{
		void AddToBalance(TTagType tag, decimal amount);

		void AddToBalance(IReadOnlyTaggedTokens<TTagType> tokens);

		void RemoveFromBalance(TTagType tag, decimal amount);

		void RemoveFromBalance(IReadOnlyTaggedTokens<TTagType> tokens);
	}
}