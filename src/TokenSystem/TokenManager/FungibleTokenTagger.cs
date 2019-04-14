using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using TokenSystem.Exceptions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager
{
	public class FungibleTokenTagger : ITokenTagger<string>
	{
		public const string DefaultTokenTag = "FungibleToken";

		public IReadOnlyTaggedTokens<string> Tag(Address owner, BigInteger amount)
		{
			if (amount < 0)
			{
				throw new NonPositiveTokenAmountException(amount);
			}

			var tokens = new ReadOnlyTaggedTokens<string>(new SortedDictionary<string, BigInteger>
			{
				[DefaultTokenTag] = amount
			});

			return tokens;
		}
	}
}