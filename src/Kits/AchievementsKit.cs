using Achievements;
using ContractsCore;
using TokenSystem.TokenManagerBase;

namespace Kits
{
	public class AchievementsKit : IKit
	{
		public AchievementsKit(int allowanceDebtExchangeRate)
		{
			this.ExchangeRate = allowanceDebtExchangeRate;
		}

		private int ExchangeRate { get; }

		public void NewInstance(
			ContractRegistry registry,
			IAddressFactory addressFactory,
			Address originAddress)
		{
			var uniformTagger = new FungibleTokenTagger();
			var uniformPicker = new FungibleTokenPicker();

			var allowanceToken = new TokenManager(
				addressFactory.Create(),
				originAddress,
				registry,
				uniformTagger,
				uniformPicker);
			var debtToken = new TokenManager(
				addressFactory.Create(),
				originAddress,
				registry,
				uniformTagger,
				uniformPicker);

			// TODO: Create achievementsGroup
			Address achievementsGroup = addressFactory.Create();
			var achievements = new AchievementFactory(
				addressFactory.Create(),
				registry,
				originAddress,
				allowanceToken.Address,
				debtToken.Address,
				this.ExchangeRate,
				achievementsGroup);

			// TODO: Add allowanceToken and debtToken to achievementsGroup

			// TODO: Give achievementsGroup permissions to:
			// 1. Burn allowanceTokens
			// 2. Mint debtTokens
			// 3. Transfer debtTokens

			// TODO: Create other groups which will be used in the organisation

			// TODO: Add initial members to groups
		}
	}
}