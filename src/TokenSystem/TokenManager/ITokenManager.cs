using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager
{
	public interface ITokenManager<TTagType>
	{
		IReadOnlyTaggedTokens<TTagType> TaggedBalanceOf(Address tokenHolder);

		IReadOnlyTaggedTokens<TTagType> TaggedTotalBalance();

		void Mint(decimal amount, Address to);

		void Transfer(decimal amount, Address from, Address to, ITokenPicker<TTagType> customPicker = null);

		void Burn(decimal amount, Address from, ITokenPicker<TTagType> customPicker = null);
	}
}