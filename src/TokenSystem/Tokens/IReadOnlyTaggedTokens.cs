// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;

namespace TokenSystem.Tokens
{
	public interface IReadOnlyTaggedTokens<TTagType> : IEnumerable<KeyValuePair<TTagType, decimal>>
	{
		decimal TotalTokens { get; }

		decimal GetAmountByTag(TTagType tag);
	}
}