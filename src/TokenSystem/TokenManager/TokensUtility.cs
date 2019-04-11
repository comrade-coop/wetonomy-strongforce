using TokenSystem.Exceptions;

namespace TokenSystem.TokenManager
{
	public static class TokensUtility
	{
		public static void RequirePositiveAmount(decimal tokenAmount)
		{
			if (tokenAmount <= 0)
			{
				throw new NonPositiveTokenAmountException(tokenAmount);
			}
		}
	}
}