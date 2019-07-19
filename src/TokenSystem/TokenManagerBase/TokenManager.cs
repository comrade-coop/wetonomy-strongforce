// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Exceptions;
using ContractsCore.Permissions;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;
using Action = ContractsCore.Actions.Action;

namespace TokenSystem.TokenManagerBase
{
	public class TokenManager : AclPermittedContract
	{
		private readonly ITokenTagger tokenTagger;
		private readonly ITokenPicker defaultTokenPicker;

		private readonly IDictionary<Address, ITaggedTokens> holdersToBalances
			= new Dictionary<Address, ITaggedTokens>();

		private readonly ITaggedTokens totalBalance = new TaggedTokens();

		public TokenManager(
			Address address,
			Address permissionManager,
			ContractRegistry registry,
			ITokenTagger tokenTagger,
			ITokenPicker defaultTokenPicker)
			: base(address, registry, permissionManager)
		{
			this.tokenTagger = tokenTagger;
			this.defaultTokenPicker = defaultTokenPicker;
		}

		public TokenManager(
			Address address,
			Address permissionManager,
			ContractRegistry registry,
			AccessControlList acl,
			ITokenTagger tokenTagger,
			ITokenPicker defaultTokenPicker)
			: base(address, registry, permissionManager, acl)
		{
			this.tokenTagger = tokenTagger;
			this.defaultTokenPicker = defaultTokenPicker;
		}

		public event EventHandler<TokensMintedEventArgs> TokensMinted;

		public event EventHandler<TokensTransferredEventArgs> TokensTransferred;

		public event EventHandler<TokensBurnedEventArgs> TokensBurned;

		public IReadOnlyTaggedTokens TaggedBalanceOf(Address tokenHolder)
			=> this.holdersToBalances.ContainsKey(tokenHolder)
				? new ReadOnlyTaggedTokens(this.holdersToBalances[tokenHolder])
				: new ReadOnlyTaggedTokens();

		public IReadOnlyTaggedTokens TaggedTotalBalance() => this.totalBalance;

		protected void TrySendTokenAction(Action action)
		{
			try
			{
				this.OnSend(action);
			}
			catch (NoPermissionException e)
			{
			}
		}

		protected virtual void OnTokensMinted(IReadOnlyTaggedTokens tokens, Address to)
		{
			var mintedAction = new TokensMintedAction(string.Empty, to, tokens);
			this.TrySendTokenAction(mintedAction);

			var tokensMintedArgs = new TokensMintedEventArgs(tokens, to);
			this.TokensMinted?.Invoke(this, tokensMintedArgs);
		}

		protected virtual void OnTokensTransferred(IReadOnlyTaggedTokens tokens, Address from, Address to)
		{
			var sentAction = new TokensSentAction(string.Empty, from, to, tokens);
			var receivedAction = new TokensReceivedAction(string.Empty, to, from, tokens);
			this.TrySendTokenAction(sentAction);
			this.TrySendTokenAction(receivedAction);

			var transferArgs = new TokensTransferredEventArgs(tokens, from, to);
			this.TokensTransferred?.Invoke(this, transferArgs);
		}

		protected virtual void OnTokensBurned(IReadOnlyTaggedTokens tokens, Address from)
		{
			var burnedAction = new TokensBurnedAction(string.Empty, from, tokens);
			this.TrySendTokenAction(burnedAction);

			var burnArgs = new TokensBurnedEventArgs(tokens, from);
			this.TokensBurned?.Invoke(this, burnArgs);
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case MintAction mintAction:
					this.Mint(mintAction.Amount, mintAction.To);
					return true;

				case TransferAction transferAction:
					this.Transfer(
						transferAction.Amount,
						transferAction.From,
						transferAction.To,
						transferAction.CustomPicker);
					return true;

				case BurnAction burnAction:
					this.Burn(
						burnAction.Amount,
						burnAction.From,
						burnAction.CustomPicker);
					return true;

				default:
					return false;
			}
		}

		private void Mint(BigInteger amount, Address to)
		{
			IReadOnlyTaggedTokens newTokens = this.tokenTagger.Tag(to, amount);
			this.AddToBalances(newTokens, to);

			this.OnTokensMinted(newTokens, to);
		}

		private void Transfer(BigInteger amount, Address from, Address to, ITokenPicker customPicker = null)
		{
			if (from.Equals(to))
			{
				throw new ArgumentException("Addresses can't transfer to themselves");
			}

			customPicker = customPicker ?? this.defaultTokenPicker;

			IReadOnlyTaggedTokens tokensToTransfer = customPicker.Pick(this.holdersToBalances[from], amount);
			this.RemoveFromBalances(tokensToTransfer, from);
			this.AddToBalances(tokensToTransfer, to);

			this.OnTokensTransferred(tokensToTransfer, from, to);
		}

		private void Burn(BigInteger amount, Address from, ITokenPicker customPicker = null)
		{
			customPicker = customPicker ?? this.defaultTokenPicker;

			IReadOnlyTaggedTokens tokensToBurn = customPicker.Pick(this.holdersToBalances[from], amount);
			this.holdersToBalances[from].RemoveFromBalance(tokensToBurn);

			this.OnTokensBurned(tokensToBurn, from);
		}

		private void AddToBalances(IReadOnlyTaggedTokens tokens, Address holder)
		{
			if (!this.holdersToBalances.ContainsKey(holder))
			{
				this.holdersToBalances[holder] = new TaggedTokens();
			}

			this.holdersToBalances[holder].AddToBalance(tokens);
			this.totalBalance.AddToBalance(tokens);
		}

		private void RemoveFromBalances(IReadOnlyTaggedTokens tokens, Address holder)
		{
			this.holdersToBalances[holder].RemoveFromBalance(tokens);
			this.totalBalance.RemoveFromBalance(tokens);
		}
	}
}