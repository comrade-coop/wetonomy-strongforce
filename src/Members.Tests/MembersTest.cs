using ContractsCore;
using TokenSystem.TokenManagerBase;
using Xunit;
using Members.Actions;
using ContractsCore.Actions;
using TokenSystem.TokenManagerBase.Actions;
using ContractsCore.Permissions;

namespace Members.Tests
{
	public class MembersTest
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly TokenManager tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;
		private Member member;

		public MembersTest()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.tokenManager = new TokenManager(
				this.addressFactory.Create(),
				this.permissionManager.Address,
				this.contractRegistry,
				tokenTagger,
				tokenPicker);
			var addMintPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(MintAction)),
				this.permissionManager.Address);

			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.tokenManager);
			this.permissionManager.ExecuteAction(addMintPermission);
		}

		[Fact]
		public void InitializeMember_BurnTokensOnMint_AssertEqual()
		{
			this.member = new Member(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			this.contractRegistry.RegisterContract(this.member);
			var mintedStrategy = new AddTokensMintedStrategyAction(
				string.Empty, this.member.Address, this.tokenManager.Address, new BurnTokensReceivedStrategy());

			this.permissionManager.ExecuteAction(mintedStrategy);

			var addTokenMintPermission = new AddPermissionAction(
				string.Empty,
				this.member.Address,
				new Permission(typeof(TokensMintedAction)),
				this.tokenManager.Address);
			this.permissionManager.ExecuteAction(addTokenMintPermission);

			var addTokenBurnPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(BurnAction)),
				this.member.Address);
			this.permissionManager.ExecuteAction(addTokenBurnPermission);

			var mintAction = new MintAction(
				string.Empty,
				this.tokenManager.Address,
				400,
				this.member.Address);
			this.permissionManager.ExecuteAction(mintAction);

			Assert.Equal(0, this.tokenManager.TaggedBalanceOf(this.member.Address).TotalTokens);

		}
	}
}