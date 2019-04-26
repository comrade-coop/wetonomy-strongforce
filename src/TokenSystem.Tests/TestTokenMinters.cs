// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using TokenSystem.TokenFlow;
using TokenSystem.TokenManager;
using TokenSystem.TokenManager.Actions;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenMinters
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly TokenManager<string> tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly IList<Address> addresses;
		private readonly ContractExecutor permissionManager;

		public TestTokenMinters()
		{
			this.contractRegistry = new ContractRegistry();
			this.addresses = AddressTestUtils.GenerateRandomAddresses();
			this.permissionManager = new ContractExecutor(this.addresses[0]);
			this.contractRegistry.RegisterContract(this.permissionManager);

			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.tokenManager = new TokenManager<string>(
				this.addressFactory.Create(),
				this.permissionManager.Address,
				this.contractRegistry,
				tokenTagger,
				tokenPicker);

			this.contractRegistry.RegisterContract(this.tokenManager);

			var mintPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(MintAction)),
				this.permissionManager.Address);

			var transferPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(TransferAction<string>)),
				this.permissionManager.Address);

			var burnPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(BurnAction<string>)),
				this.permissionManager.Address);

			this.permissionManager.ExecuteAction(mintPermission);
			this.permissionManager.ExecuteAction(transferPermission);
			this.permissionManager.ExecuteAction(burnPermission);
		}

		[Fact]
		public void Burn_WhenUsingUniformOnBurnMinter_MintsCorrectAmountToRecipients()
		{
			IList<Address> receivers = AddressTestUtils.GenerateRandomAddresses(5);
			const int expectedMintAmount = 100;
			var minter = new UniformOnBurnTokenMinter<string>(
				this.addressFactory.Create(),
				this.tokenManager,
				receivers);

			this.contractRegistry.RegisterContract(minter);

			var mintPermissionAction = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(MintAction)),
				minter.Address);

			this.permissionManager.ExecuteAction(mintPermissionAction);

			Address burnAddress = this.addresses[3];

			int burnAmount = expectedMintAmount * receivers.Count;

			var mintAction = new MintAction(
				string.Empty,
				this.tokenManager.Address,
				burnAmount,
				burnAddress);
			var burnAction = new BurnAction<string>(
				string.Empty,
				this.tokenManager.Address,
				burnAmount,
				burnAddress);

			this.permissionManager.ExecuteAction(mintAction);
			this.permissionManager.ExecuteAction(burnAction);

			foreach (Address address in receivers)
			{
				Assert.Equal(expectedMintAmount, this.tokenManager.TaggedBalanceOf(address).TotalTokens);
			}
		}
	}
}