using System.Collections.Generic;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager
{
	public class FungibleTokenPicker : ITokenPicker<string>
	{
		public IReadOnlyTaggedTokens<string> Pick(IReadOnlyTaggedTokens<string> tokens, decimal amount)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			decimal availableTokens = tokens.GetAmountByTag(FungibleTokenTagger.DefaultTokenTag);

			if (availableTokens < amount)
			{
				throw new InsufficientTokenAmountException(availableTokens, amount);
			}

			var pickedTokens = new ReadOnlyTaggedTokens<string>(new SortedDictionary<string, decimal>
			{
				[FungibleTokenTagger.DefaultTokenTag] = amount
			});

			return pickedTokens;
		}
	}
}