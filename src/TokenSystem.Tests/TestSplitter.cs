// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using StrongForce.Core.Permissions;
using StrongForce.Core.Tests.Mocks;
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
		private readonly TestRegistry registry = new TestRegistry();
		private readonly ISet<Address> recipients = new HashSet<Address>();

		public TestSplitter()
		{
			for (var i = 0; i < RecipientCount; i++)
			{
				// Using TokenSplitter as a contract which would allow token transfers
				this.recipients.Add(this.registry.CreateContract<UniformTokenSplitter>());
			}

			this.permissionManager = this.registry.CreateContract<UniformTokenSplitter>();

			this.tokenManager = this.registry.CreateContract<TokenManager>(new Dictionary<string, object>()
			{
				{ "Admin", this.permissionManager.ToString() },
				{ "User", null },
			});

			this.splitter = this.registry.CreateContract<UniformTokenSplitter>(new Dictionary<string, object>()
			{
				{ "TokenManager", this.tokenManager.ToString() },
				{ "Recipients", this.recipients.Select(x => (object)x.ToString()).ToList() },
			});

			this.registry.SendMessage(this.permissionManager, this.tokenManager, AddPermissionAction.Type, new Dictionary<string, object>()
			{
				{ AddPermissionAction.PermissionType, MintAction.Type },
				{ AddPermissionAction.PermissionSender, this.permissionManager.ToString() },
				{ AddPermissionAction.PermissionTarget, this.tokenManager.ToString() },
			});
		}

		[Theory]
		[InlineData(100)]
		public void Mint_WhenMintingToSplitter_ShouldSplitToRecipients(int splitAmount)
		{
			this.registry.SendMessage(this.permissionManager, this.tokenManager, MintAction.Type, new Dictionary<string, object>()
			{
				{ MintAction.To, this.splitter.ToString() },
				{ MintAction.Amount, splitAmount.ToString() },
			});

			foreach (Address recipient in this.recipients)
			{
				BigInteger expectedBalance = splitAmount / this.recipients.Count;
				BigInteger actualBalance = new ReadOnlyTaggedTokens(
					this.registry.GetContract(this.tokenManager).GetState()
					.GetDictionary("Balances")
					.GetDictionary(recipient.ToString()) ?? new Dictionary<string, object>()).TotalBalance;
				Assert.Equal(expectedBalance, actualBalance);
			}
		}

		[Theory]
		[InlineData(100)]
		public void Transfer_WhenTransferringToSplitter_ShouldSplitToRecipients(int splitAmount)
		{
			this.registry.SendMessage(this.permissionManager, this.tokenManager, MintAction.Type, new Dictionary<string, object>()
			{
				{ MintAction.To, this.permissionManager.ToString() },
				{ MintAction.Amount, splitAmount.ToString() },
			});

			this.registry.SendMessage(this.permissionManager, this.tokenManager, TransferAction.Type, new Dictionary<string, object>()
			{
				{ TransferAction.To, this.splitter.ToString() },
				{ TransferAction.Amount, splitAmount.ToString() },
			});

			foreach (Address recipient in this.recipients)
			{
				BigInteger expectedBalance = splitAmount / this.recipients.Count;
				BigInteger actualBalance = new ReadOnlyTaggedTokens(
					this.registry.GetContract(this.tokenManager).GetState()
					.GetDictionary("Balances")
					.GetDictionary(recipient.ToString()) ?? new Dictionary<string, object>()).TotalBalance;
				Assert.Equal(expectedBalance, actualBalance);
			}
		}
	}
}