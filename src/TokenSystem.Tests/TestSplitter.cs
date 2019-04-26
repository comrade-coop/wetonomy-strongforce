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

		private readonly ContractExecutor permissionManager;

		public TestSplitter()
		{
			this.recipients = AddressTestUtils.GenerateRandomAddresses(RecipientCount);
			this.contractRegistry = new ContractRegistry();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());

			this.contractRegistry.RegisterContract(this.permissionManager);

			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.tokenManager = new TokenManager<string>(
				this.addressFactory.Create(),
				this.permissionManager.Address,
				this.contractRegistry,
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
				this.tokenManager.Address,
				new Permission(typeof(MintAction)),
				this.permissionManager.Address);

			AddressWildCard card = new AddressWildCard(){ this.splitter.Address, this.permissionManager.Address };

			var transferPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(TransferAction<string>)),
				card);

			this.permissionManager.ExecuteAction(mintPermission);
			this.permissionManager.ExecuteAction(transferPermission);
		}

		[Theory]
		[InlineData(100)]
		public void Mint_WhenMintingToSplitter_ShouldSplitToRecipients(int splitAmount)
		{
			var mintAction = new MintAction(
				string.Empty,
				this.tokenManager.Address,
				splitAmount,
				this.splitter.Address);
			this.permissionManager.ExecuteAction(mintAction);

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
				this.tokenManager.Address,
				splitAmount,
				this.permissionManager.Address);

			var transferAction = new TransferAction<string>(
				string.Empty,
				this.tokenManager.Address,
				splitAmount,
				this.permissionManager.Address,
				this.splitter.Address);

			this.permissionManager.ExecuteAction(mintAction);
			this.permissionManager.ExecuteAction(transferAction);

			foreach (Address recipient in this.recipients)
			{
				BigInteger expectedBalance = splitAmount / this.recipients.Count;
				BigInteger actualBalance = this.tokenManager.TaggedBalanceOf(recipient).TotalTokens;
				Assert.Equal(expectedBalance, actualBalance);
			}
		}
	}
}