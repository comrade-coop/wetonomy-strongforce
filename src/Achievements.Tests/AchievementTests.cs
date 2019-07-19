namespace Achievements.Tests
{
	using System.Numerics;
	using ContractsCore;
	using ContractsCore.Actions;
	using ContractsCore.Permissions;
	using TestUtils;
	using TokenSystem.TokenManagerBase;
	using TokenSystem.TokenManagerBase.Actions;
	using TokenSystem.Tokens;
	using Xunit;

	public class AchievementTests
	{
		private const int ExchangeRate = 2;
		private readonly TokenManager burnTokenManager;
		private readonly TokenManager mintTokenManager;
		private readonly ContractExecutor contractExecutor;
		private readonly IAddressFactory addressFactory;
		private readonly ContractRegistry contractRegistry;

		public AchievementTests()
		{
			this.contractRegistry = new ContractRegistry();
			this.addressFactory = new RandomAddressFactory();
			this.contractExecutor = new ContractExecutor(this.addressFactory.Create());
			this.burnTokenManager = new TokenManager(
				this.addressFactory.Create(),
				this.contractExecutor.Address,
				this.contractRegistry,
				new FungibleTokenTagger(),
				new FungibleTokenPicker());
			this.mintTokenManager = new TokenManager(
				this.addressFactory.Create(),
				this.contractExecutor.Address,
				this.contractRegistry,
				new FungibleTokenTagger(),
				new FungibleTokenPicker());

			this.contractRegistry.RegisterContract(this.burnTokenManager);
			this.contractRegistry.RegisterContract(this.mintTokenManager);
			this.contractRegistry.RegisterContract(this.contractExecutor);

			var mintForExecutorPermission = new AddPermissionAction(
				string.Empty,
				this.burnTokenManager.Address,
				new Permission(typeof(MintAction)),
				this.contractExecutor.Address);
			var transferActionForExecutor = new AddPermissionAction(
				string.Empty,
				this.burnTokenManager.Address,
				new Permission(typeof(TransferAction)),
				this.contractExecutor.Address);

			this.contractExecutor.ExecuteAction(mintForExecutorPermission);
			this.contractExecutor.ExecuteAction(transferActionForExecutor);

			var mintAction = new MintAction(
				string.Empty,
				this.burnTokenManager.Address,
				100,
				this.contractExecutor.Address);
			this.contractExecutor.ExecuteAction(mintAction);
		}

		[Fact]
		public void TokensReceived_ExchangesTokensCorrectly()
		{
			Address achiever = this.addressFactory.Create();
			var achievement = new Achievement(
				this.addressFactory.Create(),
				this.burnTokenManager.Address,
				this.mintTokenManager.Address,
				ExchangeRate,
				achiever,
				new AchievementMetaData("Hello world!"));
			this.contractRegistry.RegisterContract(achievement);

			var burnPermission = new AddPermissionAction(
				string.Empty,
				this.burnTokenManager.Address,
				new Permission(typeof(BurnAction)),
				achievement.Address);
			var mintPermission = new AddPermissionAction(
				string.Empty,
				this.mintTokenManager.Address,
				new Permission(typeof(MintAction)),
				achievement.Address);
			var transferPermission = new AddPermissionAction(
				string.Empty,
				this.mintTokenManager.Address,
				new Permission(typeof(TransferAction)),
				achievement.Address);
			this.contractExecutor.ExecuteAction(mintPermission);
			this.contractExecutor.ExecuteAction(transferPermission);
			this.contractExecutor.ExecuteAction(burnPermission);

			BigInteger transferAmount = 100;
			this.contractExecutor.ExecuteAction(
				new TransferAction(
					string.Empty,
					this.burnTokenManager.Address,
					transferAmount,
					this.contractExecutor.Address,
					achievement.Address));

			IReadOnlyTaggedTokens exchangedBalance =
				this.mintTokenManager.TaggedBalanceOf(achiever);
			BigInteger expectedBalance = transferAmount * ExchangeRate;
			Assert.Equal(expectedBalance, exchangedBalance.TotalBalance);
		}
	}
}