using System.Collections;
using System.Collections.Generic;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class TaggedTokens<TTagType> : IEnumerable<KeyValuePair<TTagType, decimal>>
	{
		private readonly IDictionary<TTagType, decimal> tagsToBalances;


		public TaggedTokens(IDictionary<TTagType, decimal> initialTagsToBalances)
		{
			foreach ((TTagType _, decimal amount) in initialTagsToBalances)
			{
				if (amount < 0)
				{
					throw new NonPositiveTokenAmountException(amount);
				}
			}

			this.tagsToBalances = initialTagsToBalances;
		}

		public TaggedTokens()
			: this(new SortedDictionary<TTagType, decimal>())
		{
		}

		public void AddToBalance(TTagType tag, decimal amount)
		{
			TokensUtility.RequirePositiveAmount(amount);
			if (!this.tagsToBalances.ContainsKey(tag))
			{
				this.tagsToBalances[tag] = 0;
			}

			this.tagsToBalances[tag] += amount;
			this.TotalTokens += amount;
		}

		public void AddToBalance(TaggedTokens<TTagType> tokens)
		{
			foreach ((TTagType tag, decimal amount) in tokens)
			{
				this.AddToBalance(tag, amount);
			}
		}

		public void RemoveFromBalance(TTagType tag, decimal amount)
		{
			TokensUtility.RequirePositiveAmount(amount);
			if (!this.tagsToBalances.ContainsKey(tag))
			{
				this.tagsToBalances[tag] = 0;
			}

			this.tagsToBalances[tag] -= amount;
			this.TotalTokens -= amount;
		}

		public void RemoveFromBalance(TaggedTokens<TTagType> tokens)
		{
			foreach ((TTagType tag, decimal amount) in tokens)
			{
				this.RemoveFromBalance(tag, amount);
			}
		}

		public decimal GetAmountByTag(TTagType tag)
			=> this.tagsToBalances.ContainsKey(tag) ? this.tagsToBalances[tag] : 0;


		public decimal TotalTokens { get; private set; }


		public IEnumerator<KeyValuePair<TTagType, decimal>> GetEnumerator()
		{
			return this.tagsToBalances.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}