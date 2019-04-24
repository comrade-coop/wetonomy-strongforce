// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using TokenSystem.TokenFlow;
using TokenSystem.TokenManager;
using TokenSystem.TokenManager.Actions;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenBurners
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly TokenManager<string> tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly IList<Address> addresses;
		private readonly Address permissionManager;

		public TestTokenBurners()
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
		public void Transfer_WhenUsingOnTransferBurner_BurnsCorrectAmountFromReceiver()
		{
			var burner = new OnTransferTokenBurner<string>(
				this.addressFactory.Create(),
				this.tokenManager);

			this.contractRegistry.RegisterContract(burner);

			var burnPermissionAction = new AddPermissionAction(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				burner.Address,
				new Permission(typeof(BurnAction<string>)));

			this.contractRegistry.HandleAction(burnPermissionAction);

			Address sender = this.addresses[3];
			Address receiver = this.addresses[4];
			const int expectedBurnAmount = 100;

			var mintAction = new MintAction(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				expectedBurnAmount,
				sender);
			var transferAction = new TransferAction<string>(
				string.Empty,
				this.addresses[3],
				this.addresses[3],
				this.tokenManager.Address,
				expectedBurnAmount,
				sender,
				receiver);

			BigInteger balanceReceiverBefore = this.tokenManager.TaggedBalanceOf(receiver).TotalTokens;

			this.contractRegistry.HandleAction(mintAction);
			this.contractRegistry.HandleAction(transferAction);

			BigInteger balanceReceiverAfter = this.tokenManager.TaggedBalanceOf(receiver).TotalTokens;
			Assert.Equal(balanceReceiverBefore, balanceReceiverAfter);
		}
	}
}