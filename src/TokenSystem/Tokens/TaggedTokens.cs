// Copyright (c) Comrade Coop. All rights reserved.

using System.Numerics;
using TokenSystem.TokenManagerBase;

namespace TokenSystem.Tokens
{
	public class TaggedTokens : ReadOnlyTaggedTokens, ITaggedTokens
	{
		public void AddToBalance(TokenTagBase tag, BigInteger amount)
		{
			TokensUtility.RequirePositiveAmount(amount);
			if (!this.TagsToBalances.ContainsKey(tag))
			{
				this.TagsToBalances[tag] = 0;
			}

			this.TagsToBalances[tag] += amount;
			this.TotalTokens += amount;
		}

		public void AddToBalance(IReadOnlyTaggedTokens tokens)
		{
			foreach ((TokenTagBase tag, BigInteger amount) in tokens)
			{
				this.AddToBalance(tag, amount);
			}
		}

		public void RemoveFromBalance(TokenTagBase tag, BigInteger amount)
		{
			TokensUtility.RequirePositiveAmount(amount);
			if (!this.TagsToBalances.ContainsKey(tag))
			{
				this.TagsToBalances[tag] = 0;
			}

			this.TagsToBalances[tag] -= amount;
			this.TotalTokens -= amount;
		}

		public void RemoveFromBalance(IReadOnlyTaggedTokens tokens)
		{
			foreach ((TokenTagBase tag, BigInteger amount) in tokens)
			{
				this.RemoveFromBalance(tag, amount);
			}
		}
	}
}