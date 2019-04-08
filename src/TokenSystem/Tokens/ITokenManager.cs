using ContractsCore;

namespace TokenSystem.Tokens
{
	public interface ITokenManager
	{
		string Symbol();

		decimal BalanceOf(Address tokenHolder);

		decimal TotalBalance();

		TaggedTokens TaggedBalanceOf(Address tokenHolder);

		TaggedTokens TaggedTotalBalance();

		void Mint(decimal amount, Address to);

		void Transfer(decimal amount, Address from, Address to, ITaggedTokenPickStrategy tokenPickStrategy = null);

		void Burn(decimal amount, Address from, ITaggedTokenPickStrategy tokenPickStrategy = null);
	}
}