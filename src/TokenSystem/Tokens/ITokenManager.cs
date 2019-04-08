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
	}
}