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
		private readonly Address permissionManager;

		public TestTokenMinters()
		{
			this.contractRegistry = new ContractRegistry();

			this.addresses = AddressTestUtils.GenerateRandomAddresses();

			this.permissionManager = this.addresses[0];

			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.tokenManager = new TokenManager<string>(
				this.addressFactory.Create(),
				this.permissionManager,
				tokenTagger,
				tokenPicker);

			this.contractRegistry.RegisterContract(this.tokenManager);

			var mintPermission = new AddPermissionAction(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				this.permissionManager,
				new Permission(typeof(MintAction)));

			var transferPermission = new AddPermissionAction(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				this.permissionManager,
				new Permission(typeof(TransferAction<string>)));

			this.contractRegistry.HandleAction(mintPermission);
			this.contractRegistry.HandleAction(transferPermission);
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
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				minter.Address,
				new Permission(typeof(MintAction)));

			this.contractRegistry.HandleAction(mintPermissionAction);

			Address burnAddress = this.addresses[3];

			int burnAmount = expectedMintAmount * receivers.Count;

			var mintAction = new MintAction(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				burnAmount,
				burnAddress);
			var burnAction = new BurnAction<string>(
				string.Empty,
				this.addresses[3],
				this.addresses[3],
				this.tokenManager.Address,
				burnAmount,
				burnAddress);

			this.contractRegistry.HandleAction(mintAction);
			this.contractRegistry.HandleAction(burnAction);

			foreach (Address address in receivers)
			{
				Assert.Equal(expectedMintAmount, this.tokenManager.TaggedBalanceOf(address).TotalTokens);
			}
		}
	}
}