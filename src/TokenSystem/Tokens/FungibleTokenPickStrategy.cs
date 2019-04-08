using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class FungibleTokenPickStrategy : ITaggedTokenPickStrategy
	{
		public TaggedTokens Pick(TaggedTokens tokens, decimal amount)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			if (tokens[FungibleTokenTagger.DefaultTokenTag] < amount)
			{
				throw new InsufficientTokenAmountException(
					tokens[FungibleTokenTagger.DefaultTokenTag],
					amount);
			}

			var pickedTokens = new TaggedTokens {[FungibleTokenTagger.DefaultTokenTag] = amount};
			return pickedTokens;
		}
	}
}