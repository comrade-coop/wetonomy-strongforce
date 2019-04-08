using ContractsCore;
using TokenSystem.Exceptions;

namespace TokenSystem.Tokens
{
	public class FungibleTokenTagger : ITokenTagger
	{
		public const string DefaultTokenTag = "FungibleToken";

		public TaggedTokens Tag(Address owner, decimal amount)
		{
			if (amount <= 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			return new TaggedTokens {[DefaultTokenTag] = amount};
		}
	}
}