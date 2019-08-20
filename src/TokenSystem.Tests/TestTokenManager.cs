// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using StrongForce.Core.Permissions;
using TokenSystem.Exceptions;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenManager
	{
		private const int AddressesCount = 10;

		private readonly Address tokenManager;
		private readonly Address permissionManager;
		private readonly ContractRegistry registry = new ContractRegistry();
		private readonly List<Address> addresses = AddressTestUtils.GenerateRandomAddresses(AddressesCount);

		public TestTokenManager()
		{
			this.permissionManager = this.registry.AddressFactory.Create();

			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();

			this.tokenManager = this.registry.CreateContract<TokenManager>(new Dictionary<string, object>()
			{
				{ "Tagger", tokenTagger.ToState() },
				{ "Picker", tokenPicker.ToState() },
				{ "Admin", this.permissionManager.ToBase64String() },
			});

			this.registry.SendAction(this.permissionManager, this.tokenManager, AddPermissionAction.Type, new Dictionary<string, object>()
			{
				{ AddPermissionAction.PermissionType, MintAction.Type },
				{ AddPermissionAction.PermissionSender, this.permissionManager.ToBase64String() },
				{ AddPermissionAction.PermissionTarget, this.tokenManager.ToBase64String() },
			});
		}

		[Fact]
		public void TaggedBalance_WhenTokenManagerHasBeenInstantiated_ShouldHaveAllBalancesToZero()
		{
			IDictionary<string, object> balances = this.registry.GetContract(this.tokenManager).GetState().GetDictionary("Balances");

			Assert.Equal(0, balances.Count);

			this.addresses.ForEach(address =>
			{
				IReadOnlyTaggedTokens balance = this.GetBalanceOf(address);
				Assert.Equal(0, balance.TotalBalance);
			});
		}

		[Theory]
		[InlineData(100)]
		public void Mint_WhenPassedValidAmountAndAddresses_MintsTokensCorrectly(int amount)
		{
			Address receiver = this.addresses[0];

			this.MintTokens(amount, receiver);

			Assert.Equal(amount, this.GetBalanceOf(receiver).TotalBalance);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-100)]
		public void Mint_WhenPassedNonPositiveAmount_Throws(int amount)
		{
			Address receiver = this.addresses[0];
			Assert.Throws<NonPositiveTokenAmountException>(() => this.MintTokens(amount, receiver));
		}

		[Theory]
		[InlineData(1000, 50)]
		public void Transfer_WhenPassedValidAmount_TransfersTokensCorrectly(
			int mintAmount,
			int transferAmount)
		{
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			this.MintTokens(mintAmount, from);
			this.MintTokens(mintAmount, to);

			BigInteger balanceFromBeforeTransfer = this.GetBalanceOf(from).TotalBalance;
			BigInteger balanceToBeforeTransfer = this.GetBalanceOf(to).TotalBalance;

			this.TransferTokens(transferAmount, from, to);

			IReadOnlyTaggedTokens balanceFromAfterTransfer = this.GetBalanceOf(from);
			IReadOnlyTaggedTokens balanceOfToAfterTransfer = this.GetBalanceOf(to);

			Assert.Equal(
				balanceFromBeforeTransfer - transferAmount,
				balanceFromAfterTransfer.TotalBalance);
			Assert.Equal(
				balanceToBeforeTransfer + transferAmount,
				balanceOfToAfterTransfer.TotalBalance);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-234)]
		public void Transfer_WhenPassedNonPositiveAmount_Throws(int transferAmount)
		{
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			Assert.Throws<NonPositiveTokenAmountException>(
				() => this.TransferTokens(transferAmount, from, to));
		}

		[Theory]
		[InlineData(100, 5000)]
		public void Transfer_WhenPassedMoreThanOwned_Throws(
			int mintAmount,
			int transferAmount)
		{
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			this.MintTokens(mintAmount, from);

			Assert.Throws<InsufficientTokenAmountException>(
				() => this.TransferTokens(transferAmount, from, to));
		}

		[Fact]
		public void Transfer_WhenSenderAttemptsToTransferToHimself_Throws()
		{
			Address from = this.addresses[0];
			int mintAmount = 100;
			int transferAmount = 50;

			this.MintTokens(mintAmount, from);

			Assert.Throws<ArgumentException>(
				() => this.TransferTokens(transferAmount, from, from));
		}

		[Theory]
		[InlineData(100, 90)]
		public void Burn_WhenPassedValidArguments_BurnsCorrectly(int mintAmount, int burnAmount)
		{
			Address address = this.addresses[0];

			this.MintTokens(mintAmount, address);
			BigInteger balanceBeforeBurn = this.GetBalanceOf(address).TotalBalance;

			this.BurnTokens(burnAmount, address);

			IReadOnlyTaggedTokens balanceAfterBurn = this.GetBalanceOf(address);

			Assert.Equal(balanceBeforeBurn - burnAmount, balanceAfterBurn.TotalBalance);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1000)]
		public void Burn_WhenPassedNonPositiveAmounts_Throws(int burnAmount)
		{
			Address address = this.addresses[0];
			Assert.Throws<NonPositiveTokenAmountException>(
				() => this.BurnTokens(burnAmount, address));
		}

		[Theory]
		[InlineData(100, 110)]
		public void Burn_WhenAttemptingToBurnMoreThanOwned_Throws(int mintAmount, int burnAmount)
		{
			Address address = this.addresses[0];

			this.MintTokens(mintAmount, address);

			Assert.Throws<InsufficientTokenAmountException>(
				() => this.BurnTokens(burnAmount, address));
		}

		// TODO: Check for EventAction sent when tokens are transfered
		private void MintTokens(BigInteger amount, Address receiver)
		{
			this.registry.SendAction(this.permissionManager, this.tokenManager, MintAction.Type, new Dictionary<string, object>()
			{
				{ MintAction.Amount, amount.ToString() },
				{ MintAction.To, receiver.ToBase64String() },
			});
		}

		private void TransferTokens(
			BigInteger amount,
			Address from,
			Address to,
			ITokenPicker tokenPicker = null)
		{
			this.registry.SendAction(from, this.tokenManager, TransferAction.Type, new Dictionary<string, object>()
			{
				{ TransferAction.Amount, amount.ToString() },
				{ TransferAction.To, to.ToBase64String() },
				{ TransferAction.Picker, tokenPicker?.ToState() },
			});
		}

		private void BurnTokens(
			int amount,
			Address from,
			ITokenPicker tokenPicker = null)
		{
			this.registry.SendAction(from, this.tokenManager, BurnAction.Type, new Dictionary<string, object>()
			{
				{ BurnAction.Amount, amount.ToString() },
				{ BurnAction.Picker, tokenPicker?.ToState() },
			});
		}

		private IReadOnlyTaggedTokens GetBalanceOf(Address address)
		{
			return new ReadOnlyTaggedTokens(
				this.registry.GetContract(this.tokenManager).GetState()
				.GetDictionary("Balances")
				.GetDictionary(address.ToBase64String()) ?? new Dictionary<string, object>());
		}
	}
}