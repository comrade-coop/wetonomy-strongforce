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
	public class TestSplitter
	{
		private const int RecipientCount = 5;
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();

		private readonly TokenSplitter<string> splitter;
		private readonly TokenManager<string> tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly IList<Address> recipients;

		private readonly Address permissionManager;

		public TestSplitter()
		{
			this.recipients = AddressTestUtils.GenerateRandomAddresses(RecipientCount);

			this.contractRegistry = new ContractRegistry();

			this.permissionManager = this.addressFactory.Create();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.tokenManager = new TokenManager<string>(
				this.addressFactory.Create(),
				this.permissionManager,
				tokenTagger,
				tokenPicker);

			this.splitter = new UniformTokenSplitter<string>(
				this.addressFactory.Create(),
				this.tokenManager,
				this.recipients);

			this.contractRegistry.RegisterContract(this.tokenManager);
			this.contractRegistry.RegisterContract(this.splitter);

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

		[Theory]
		[InlineData(100)]
		public void Mint_WhenMintingToSplitter_ShouldSplitToRecipients(int splitAmount)
		{
			var mintAction = new MintAction(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				splitAmount,
				this.splitter.Address);
			this.contractRegistry.HandleAction(mintAction);

			foreach (Address recipient in this.recipients)
			{
				BigInteger expectedBalance = splitAmount / this.recipients.Count;
				BigInteger actualBalance = this.tokenManager.TaggedBalanceOf(recipient).TotalTokens;
				Assert.Equal(expectedBalance, actualBalance);
			}
		}

		[Theory]
		[InlineData(100)]
		public void Transfer_WhenTransferringToSplitter_ShouldSplitToRecipients(int splitAmount)
		{
			var mintAction = new MintAction(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				splitAmount,
				this.permissionManager);

			var transferAction = new TransferAction<string>(
				string.Empty,
				this.permissionManager,
				this.permissionManager,
				this.tokenManager.Address,
				splitAmount,
				this.permissionManager,
				this.splitter.Address);

			this.contractRegistry.HandleAction(mintAction);
			this.contractRegistry.HandleAction(transferAction);

			foreach (Address recipient in this.recipients)
			{
				BigInteger expectedBalance = splitAmount / this.recipients.Count;
				BigInteger actualBalance = this.tokenManager.TaggedBalanceOf(recipient).TotalTokens;
				Assert.Equal(expectedBalance, actualBalance);
			}
		}
	}
}