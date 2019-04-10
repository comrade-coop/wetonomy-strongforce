using System.Collections.Generic;
using TokenSystem.Exceptions;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
	public class FungibleTokenTagger : ITokenTagger<string>
	{
		public const string DefaultTokenTag = "FungibleToken";

		public TaggedTokens<string> Tag(Address owner, decimal amount)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			var tokens = new TaggedTokens<string>();
			tokens.AddToBalance(DefaultTokenTag, amount);

			return tokens;
		}
	}
}