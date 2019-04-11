using System.Collections.Generic;

namespace TokenSystem.Tokens
{
	public interface IReadOnlyTaggedTokens<TTagType> : IEnumerable<KeyValuePair<TTagType, decimal>>
	{
		decimal GetAmountByTag(TTagType tag);

		decimal TotalTokens { get; }
	}
}