// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using TokenSystem.Exceptions;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.TokenManagerBase.TokenTags;
using TokenSystem.Tokens;
using Xunit;

namespace TokenSystem.Tests
{
	public class TestTokenManager
	{
		private const int AddressesCount = 10;

		private readonly TokenManager tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;
		private readonly List<Address> addresses = AddressTestUtils.GenerateRandomAddresses(AddressesCount);

		public TestTokenManager()
		{
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addresses[9]);
			this.tokenManager = new TokenManager(
				this.addresses[0],
				this.permissionManager.Address,
				this.contractRegistry,
				tokenTagger,
				tokenPicker);

			this.contractRegistry = new ContractRegistry();
			this.contractRegistry.RegisterContract(this.tokenManager);
			this.contractRegistry.RegisterContract(this.permissionManager);

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
			var burnPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(BurnAction)),
				this.permissionManager.Address);

			this.permissionManager.ExecuteAction(mintPermission);
			this.permissionManager.ExecuteAction(transferPermission);
			this.permissionManager.ExecuteAction(burnPermission);
		}

		[Fact]
		public void TaggedBalance_WhenTokenManagerHasBeenInstantiated_ShouldHaveAllBalancesToZero()
		{
			IReadOnlyTaggedTokens taggedBalance = this.tokenManager.TaggedTotalBalance();
			Assert.Equal(0, taggedBalance.TotalTokens);

			this.addresses.ForEach(address =>
			{
				IReadOnlyTaggedTokens balance = this.tokenManager.TaggedBalanceOf(address);
				Assert.Equal(0, balance.TotalTokens);
			});
		}

		[Theory]
		[InlineData(100)]
		public void Mint_WhenPassedValidAmountAndAddresses_MintsTokensCorrectly(int amount)
		{
			Address receiver = this.addresses[0];

			this.MintTokens(amount, receiver);

			Assert.Equal(amount, this.tokenManager.TaggedBalanceOf(receiver).TotalTokens);
			Assert.Equal(amount, this.tokenManager.TaggedTotalBalance().TotalTokens);
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

			BigInteger balanceFromBeforeTransfer = this.tokenManager.TaggedBalanceOf(from).TotalTokens;
			BigInteger balanceToBeforeTransfer = this.tokenManager.TaggedBalanceOf(to).TotalTokens;

			this.TransferTokens(transferAmount, from, to);

			IReadOnlyTaggedTokens balanceFromAfterTransfer = this.tokenManager.TaggedBalanceOf(from);
			IReadOnlyTaggedTokens balanceOfToAfterTransfer = this.tokenManager.TaggedBalanceOf(to);

			Assert.Equal(
				balanceFromBeforeTransfer - transferAmount,
				balanceFromAfterTransfer.TotalTokens);
			Assert.Equal(
				balanceToBeforeTransfer + transferAmount,
				balanceOfToAfterTransfer.TotalTokens);
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
			BigInteger balanceBeforeBurn = this.tokenManager.TaggedBalanceOf(address).TotalTokens;

			this.BurnTokens(burnAmount, address);

			IReadOnlyTaggedTokens balanceAfterBurn = this.tokenManager.TaggedBalanceOf(address);

			Assert.Equal(balanceBeforeBurn - burnAmount, balanceAfterBurn.TotalTokens);
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

		[Theory]
		[InlineData(1000)]
		public void Mint_RaisesEvent(int mintAmount)
		{
			Address to = this.addresses[0];
			this.tokenManager.TokensMinted += (sender, args) =>
			{
				Assert.Equal(mintAmount, args.Tokens.TotalTokens);
				Assert.Equal(to, args.To);
			};

			this.MintTokens(mintAmount, to);
		}

		[Theory]
		[InlineData(1000, 100)]
		public void Transfer_RaisesEvent(int mintAmount, int transferAmount)
		{
			Address from = this.addresses[0];
			Address to = this.addresses[1];

			this.tokenManager.TokensTransferred += (sender, args) =>
			{
				Assert.Equal(transferAmount, args.Tokens.TotalTokens);
				Assert.Equal(from, args.From);
				Assert.Equal(to, args.To);
			};

			this.MintTokens(mintAmount, from);
			this.TransferTokens(transferAmount, from, to);
		}

		[Theory]
		[InlineData(1000, 100)]
		public void Burn_RaisesEvent(int mintAmount, int burnAmount)
		{
			Address from = this.addresses[0];

			this.tokenManager.TokensBurned += (sender, args) =>
			{
				Assert.Equal(burnAmount, args.Tokens.TotalTokens);
				Assert.Equal(from, args.From);
			};

			this.MintTokens(mintAmount, from);
			this.BurnTokens(burnAmount, from);
		}

		private void MintTokens(BigInteger amount, Address receiver)
		{
			var mintAction = new MintAction(
				string.Empty,
				this.tokenManager.Address,
				amount,
				receiver);
			this.permissionManager.ExecuteAction(mintAction);
		}

		private void TransferTokens(
			BigInteger amount,
			Address from,
			Address to,
			ITokenPicker tokenPicker = null)
		{
			var transferAction = new TransferAction(
				string.Empty,
				this.tokenManager.Address,
				amount,
				from,
				to,
				tokenPicker);
			this.permissionManager.ExecuteAction(transferAction);
		}

		private void BurnTokens(
			int amount,
			Address from,
			ITokenPicker tokenPicker = null)
		{
			var burnAction = new BurnAction(
				string.Empty,
				this.tokenManager.Address,
				amount,
				from,
				tokenPicker);
			this.permissionManager.ExecuteAction(burnAction);
		}
	}
}