using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
	public interface ITokenManager<TTagType>
	{
		TaggedTokens<TTagType> TaggedBalanceOf(Address tokenHolder);

		TaggedTokens<TTagType> TaggedTotalBalance();

		void Mint(decimal amount, Address to);

		void Transfer(decimal amount, Address from, Address to, ITokenPicker<TTagType> customPicker = null);

		void Burn(decimal amount, Address from, ITokenPicker<TTagType> customPicker = null);
	}
}