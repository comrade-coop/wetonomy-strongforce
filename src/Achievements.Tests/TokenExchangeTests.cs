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

	public class TokenExchangeTests
	{
		private const int ExchangeRate = 2;
		private readonly TokenManager burnTokenManager;
		private readonly TokenManager mintTokenManager;
		private readonly TokenExchange tokenExchange;
		private readonly ContractExecutor contractExecutor;

		public TokenExchangeTests()
		{
			var contractRegistry = new ContractRegistry();
			IAddressFactory addressFactory = new RandomAddressFactory();
			this.contractExecutor = new ContractExecutor(addressFactory.Create());
			this.burnTokenManager = new TokenManager(
				addressFactory.Create(),
				this.contractExecutor.Address,
				contractRegistry,
				new FungibleTokenTagger(),
				new FungibleTokenPicker());
			this.mintTokenManager = new TokenManager(
				addressFactory.Create(),
				this.contractExecutor.Address,
				contractRegistry,
				new FungibleTokenTagger(),
				new FungibleTokenPicker());
			this.tokenExchange = new TokenExchange(
				addressFactory.Create(),
				this.burnTokenManager.Address,
				this.mintTokenManager.Address,
				ExchangeRate);

			contractRegistry.RegisterContract(this.burnTokenManager);
			contractRegistry.RegisterContract(this.mintTokenManager);
			contractRegistry.RegisterContract(this.tokenExchange);
			contractRegistry.RegisterContract(this.contractExecutor);

			var burnPermission = new AddPermissionAction(
				string.Empty,
				this.burnTokenManager.Address,
				new Permission(typeof(BurnAction)),
				this.tokenExchange.Address);
			var mintPermission = new AddPermissionAction(
				string.Empty,
				this.mintTokenManager.Address,
				new Permission(typeof(MintAction)),
				this.tokenExchange.Address);
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

			this.contractExecutor.ExecuteAction(burnPermission);
			this.contractExecutor.ExecuteAction(mintPermission);
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
			var transferAction = new TransferAction(
				string.Empty,
				this.burnTokenManager.Address,
				100,
				this.contractExecutor.Address,
				this.tokenExchange.Address);
			this.contractExecutor.ExecuteAction(transferAction);

			IReadOnlyTaggedTokens exchangedBalance =
				this.mintTokenManager.TaggedBalanceOf(this.contractExecutor.Address);
			BigInteger expectedBalance = 100 * ExchangeRate;
			Assert.Equal(expectedBalance, exchangedBalance.TotalBalance);
		}
	}
}