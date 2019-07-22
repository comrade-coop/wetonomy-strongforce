using Achievements;
using ContractsCore;
using TokenSystem.TokenManagerBase;

namespace Kits
{
	public class AchievementsKit : IKit
	{
		private readonly int exchangeRate;
		private Address[] initialMembers;

		public AchievementsKit(int allowanceDebtExchangeRate, Address[] initialMembers)
		{
			this.exchangeRate = allowanceDebtExchangeRate;
			this.initialMembers = initialMembers;
		}

		public void NewInstance(
			ContractRegistry registry,
			IAddressFactory addressFactory,
			Address originAddress)
		{
			// Create tokens for organisation
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

			// Create achievementsFactory
			var achievements = new AchievementFactory(
				addressFactory.Create(),
				registry,
				originAddress,
				allowanceToken.Address,
				debtToken.Address,
				this.exchangeRate,
				achievementsGroup);

			registry.RegisterContract(allowanceToken);
			registry.RegisterContract(debtToken);
			registry.RegisterContract(achievements);
			registry.RegisterContract(achievements);
			// registry.RegisterContract(achievementsGroup);

			// TODO: Add allowanceToken and debtToken to achievementsGroup

			// TODO: Give achievementsGroup permissions to:
			// 1. Burn allowanceTokens
			// 2. Mint debtTokens
			// 3. Transfer debtTokens

			// TODO: Add voting contract
			// Give permissions to voting for almost everything

			// TODO: Create other groups which will be used in the organisation
			// Give permissions to groups to transfer tokens to AchievementsGroup
			// This should probably couple Tokens and Groups.. Needs discussion

			// TODO: Add initial members to groups
		}
	}
}