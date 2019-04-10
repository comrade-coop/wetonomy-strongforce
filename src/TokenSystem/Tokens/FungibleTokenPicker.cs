using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class FungibleTokenPicker : ITokenPicker<string>
	{
		public TaggedTokens<string> Pick(TaggedTokens<string> tokens, decimal amount)
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

			var pickedTokens = new TaggedTokens<string>();
			pickedTokens.AddToBalance(FungibleTokenTagger.DefaultTokenTag, amount);

			return pickedTokens;
		}
	}
}