// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class ReadOnlyTaggedTokens<TTagType> : IReadOnlyTaggedTokens<TTagType>
	{
		public ReadOnlyTaggedTokens(IDictionary<TTagType, decimal> initialTagsToBalances)
		{
			foreach ((TTagType _, decimal amount) in initialTagsToBalances)
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
			: this(new SortedDictionary<TTagType, decimal>())
		{
		}

		public decimal TotalTokens { get; protected set; }

		protected IDictionary<TTagType, decimal> TagsToBalances { get; set; }

		public decimal GetAmountByTag(TTagType tag)
			=> this.TagsToBalances.ContainsKey(tag) ? this.TagsToBalances[tag] : 0;

		public IEnumerator<KeyValuePair<TTagType, decimal>> GetEnumerator()
		{
			return this.TagsToBalances.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}