// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class ReadOnlyTaggedTokens: IReadOnlyTaggedTokens
	{
		public ReadOnlyTaggedTokens(IDictionary<TokenTagBase, BigInteger> initialTagsToBalances)
		{
			foreach ((TokenTagBase _, BigInteger amount) in initialTagsToBalances)
			{
				if (amount < 0)
				{
					throw new NonPositiveTokenAmountException(amount);
				}

				this.TotalTokens += amount;
			}

			this.TagsToBalances = initialTagsToBalances;
		}

		public ReadOnlyTaggedTokens()
			: this(new SortedDictionary<TokenTagBase, BigInteger>())
		{
		}

		public BigInteger TotalTokens { get; protected set; }

		protected IDictionary<TokenTagBase, BigInteger> TagsToBalances { get; set; }

		public BigInteger GetAmountByTag(TokenTagBase tag)
			=> this.TagsToBalances.ContainsKey(tag) ? this.TagsToBalances[tag] : 0;

		public IEnumerator<KeyValuePair<TokenTagBase, BigInteger>> GetEnumerator()
		{
			return this.TagsToBalances.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}