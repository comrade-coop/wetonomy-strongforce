// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using TokenSystem.TokenFlow;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.TokenManagerBase.TokenTags;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenBurners
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly TokenManager tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly IList<Address> addresses;
		private readonly ContractExecutor permissionManager;

		public TestTokenBurners()
		{
			this.contractRegistry = new ContractRegistry();
			this.addresses = AddressTestUtils.GenerateRandomAddresses();
			this.permissionManager = new ContractExecutor(this.addresses[0]);

			this.contractRegistry.RegisterContract(this.permissionManager);

			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.tokenManager = new TokenManager(
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
				new Permission(typeof(TransferAction)),
				this.permissionManager.Address);

			this.permissionManager.ExecuteAction(mintPermission);
			this.permissionManager.ExecuteAction(transferPermission);
		}

		[Fact]
		public void Transfer_WhenUsingOnTransferBurner_BurnsCorrectAmountFromReceiver()
		{
			var burner = new OnTransferTokenBurner(
				this.addressFactory.Create(),
				this.tokenManager);

			this.contractRegistry.RegisterContract(burner);

			var burnPermissionAction = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(BurnAction)),
				burner.Address);

			this.permissionManager.ExecuteAction(burnPermissionAction);

			Address sender = this.addresses[3];
			Address receiver = this.addresses[4];
			const int expectedBurnAmount = 100;

			var mintAction = new MintAction(
				string.Empty,
				this.tokenManager.Address,
				expectedBurnAmount,
				sender);
			var transferAction = new TransferAction(
				string.Empty,
				this.tokenManager.Address,
				expectedBurnAmount,
				sender,
				receiver);

			BigInteger balanceReceiverBefore = this.tokenManager.TaggedBalanceOf(receiver).TotalTokens;

			this.permissionManager.ExecuteAction(mintAction);
			this.permissionManager.ExecuteAction(transferAction);

			BigInteger balanceReceiverAfter = this.tokenManager.TaggedBalanceOf(receiver).TotalTokens;
			Assert.Equal(balanceReceiverBefore, balanceReceiverAfter);
		}
	}
}