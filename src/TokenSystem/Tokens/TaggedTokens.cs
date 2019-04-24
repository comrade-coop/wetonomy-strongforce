// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using TokenSystem.TokenManager;

namespace TokenSystem.Tokens
{
	public class TaggedTokens<TTagType> : ReadOnlyTaggedTokens<TTagType>, ITaggedTokens<TTagType>
	{
		public void AddToBalance(TTagType tag, BigInteger amount)
		{
			TokensUtility.RequirePositiveAmount(amount);
			if (!this.TagsToBalances.ContainsKey(tag))
			{
				this.TagsToBalances[tag] = 0;
			}

			this.TagsToBalances[tag] += amount;
			this.TotalTokens += amount;
		}

		public void AddToBalance(IReadOnlyTaggedTokens<TTagType> tokens)
		{
			foreach ((TTagType tag, BigInteger amount) in tokens)
			{
				this.AddToBalance(tag, amount);
			}
		}

		public void RemoveFromBalance(TTagType tag, BigInteger amount)
		{
			TokensUtility.RequirePositiveAmount(amount);
			if (!this.TagsToBalances.ContainsKey(tag))
			{
				this.TagsToBalances[tag] = 0;
			}

			this.TagsToBalances[tag] -= amount;
			this.TotalTokens -= amount;
		}

		public void RemoveFromBalance(IReadOnlyTaggedTokens<TTagType> tokens)
		{
			foreach ((TTagType tag, BigInteger amount) in tokens)
			{
				this.RemoveFromBalance(tag, amount);
			}
		}
	}
}