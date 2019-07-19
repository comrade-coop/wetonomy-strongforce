namespace Achievements
{
	using ContractsCore;
	using ContractsCore.Actions;
	using ContractsCore.Contracts;
	using TokenSystem.TokenManagerBase.Actions;

	public class Achievement : TokenExchange
	{
		public Achievement(
			Address address,
			Address burnTokenManager,
			Address mintTokenManager,
			int exchangeRate,
			Address achiever,
			AchievementMetaData metaData)
			: base(address, burnTokenManager, mintTokenManager, exchangeRate)
		{
		}

		protected override object GetState()
		{
			throw new System.NotImplementedException();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction receivedAction:

				default:
					return false;
			}
		}
	}
}