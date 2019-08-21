// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using StrongForce.Core.Permissions;
using TokenSystem.TokenFlow;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestSplitter
	{
		private const int RecipientCount = 5;

		private readonly Address splitter;
		private readonly Address tokenManager;
		private readonly Address permissionManager;
		private readonly ContractRegistry registry = new ContractRegistry();
		private readonly ISet<Address> recipients;

		public TestSplitter()
		{
			this.recipients = AddressTestUtils.GenerateRandomAddresses(RecipientCount).ToHashSet();
			this.permissionManager = this.registry.AddressFactory.Create();

			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();

			this.tokenManager = this.registry.CreateContract<TokenManager>(new Dictionary<string, object>()
			{
				{ "Tagger", tokenTagger.ToState() },
				{ "DefaultPicker", tokenPicker.ToState() },
				{ "Admin", this.permissionManager.ToBase64String() },
				{ "User", null },
			});

			this.splitter = this.registry.CreateContract<UniformTokenSplitter>(new Dictionary<string, object>()
			{
				{ "TokenManager", this.tokenManager.ToBase64String() },
				{ "Recipients", this.recipients.Select(x => (object)x.ToBase64String()).ToList() },
			});

			this.registry.SendAction(this.permissionManager, this.tokenManager, AddPermissionAction.Type, new Dictionary<string, object>()
			{
				{ AddPermissionAction.PermissionType, MintAction.Type },
				{ AddPermissionAction.PermissionSender, this.permissionManager.ToBase64String() },
				{ AddPermissionAction.PermissionTarget, this.tokenManager.ToBase64String() },
			});
		}

		[Theory]
		[InlineData(100)]
		public void Mint_WhenMintingToSplitter_ShouldSplitToRecipients(int splitAmount)
		{
			this.registry.SendAction(this.permissionManager, this.tokenManager, MintAction.Type, new Dictionary<string, object>()
			{
				{ MintAction.To, this.splitter.ToBase64String() },
				{ MintAction.Amount, splitAmount.ToString() },
			});

			foreach (Address recipient in this.recipients)
			{
				BigInteger expectedBalance = splitAmount / this.recipients.Count;
				BigInteger actualBalance = new ReadOnlyTaggedTokens(
					this.registry.GetContract(this.tokenManager).GetState()
					.GetDictionary("Balances")
					.GetDictionary(recipient.ToBase64String()) ?? new Dictionary<string, object>()).TotalBalance;
				Assert.Equal(expectedBalance, actualBalance);
			}
		}

		[Theory]
		[InlineData(100)]
		public void Transfer_WhenTransferringToSplitter_ShouldSplitToRecipients(int splitAmount)
		{
			this.registry.SendAction(this.permissionManager, this.tokenManager, MintAction.Type, new Dictionary<string, object>()
			{
				{ MintAction.To, this.permissionManager.ToBase64String() },
				{ MintAction.Amount, splitAmount.ToString() },
			});

			this.registry.SendAction(this.permissionManager, this.tokenManager, TransferAction.Type, new Dictionary<string, object>()
			{
				{ TransferAction.To, this.splitter.ToBase64String() },
				{ TransferAction.Amount, splitAmount.ToString() },
			});

			foreach (Address recipient in this.recipients)
			{
				BigInteger expectedBalance = splitAmount / this.recipients.Count;
				BigInteger actualBalance = new ReadOnlyTaggedTokens(
					this.registry.GetContract(this.tokenManager).GetState()
					.GetDictionary("Balances")
					.GetDictionary(recipient.ToBase64String()) ?? new Dictionary<string, object>()).TotalBalance;
				Assert.Equal(expectedBalance, actualBalance);
			}
		}
	}
}