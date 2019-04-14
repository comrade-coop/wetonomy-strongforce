using System.Collections.Generic;
using System.Numerics;

namespace TokenSystem.Tokens
{
	public interface IReadOnlyTaggedTokens<TTagType> : IEnumerable<KeyValuePair<TTagType, BigInteger>>
	{
		BigInteger GetAmountByTag(TTagType tag);

		BigInteger TotalTokens { get; }
	}
}