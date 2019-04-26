// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Contracts;
using ContractsCore.Events;
using ContractsCore.Exceptions;
using ContractsCore.Permissions;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManager.Actions;
using TokenSystem.Tokens;
using Action = ContractsCore.Actions.Action;

namespace TokenSystem.TokenManager
{
	public class TokenManager<TTagType> : AclPermittedContract, ITokenManager<TTagType>
	{
		private readonly ITokenTagger<TTagType> tokenTagger;
		private readonly ITokenPicker<TTagType> defaultTokenPicker;

		private readonly IDictionary<Address, ITaggedTokens<TTagType>> holdersToBalances;
		private readonly ITaggedTokens<TTagType> totalBalance;

		public TokenManager(
			Address address,
			Address permissionManager,
			ContractRegistry registry,
			ITokenTagger<TTagType> tokenTagger,
			ITokenPicker<TTagType> defaultTokenPicker)
			: base(address, registry, permissionManager)
		{
			this.tokenTagger = tokenTagger;
			this.holdersToBalances = new Dictionary<Address, ITaggedTokens<TTagType>>();
			this.totalBalance = new TaggedTokens<TTagType>();
			this.defaultTokenPicker = defaultTokenPicker;
		}

		public TokenManager(
			Address address,
			Address permissionManager,
			ContractRegistry registry,
			AccessControlList acl,
			ITokenTagger<TTagType> tokenTagger,
			ITokenPicker<TTagType> defaultTokenPicker)
			: base(address, registry, permissionManager, acl)
		{
			this.tokenTagger = tokenTagger;
			this.holdersToBalances = new Dictionary<Address, ITaggedTokens<TTagType>>();
			this.totalBalance = new TaggedTokens<TTagType>();
			this.defaultTokenPicker = defaultTokenPicker;
		}

		public event EventHandler<TokensMintedEventArgs<TTagType>> TokensMinted;

		public event EventHandler<TokensTransferredEventArgs<TTagType>> TokensTransferred;

		public event EventHandler<TokensBurnedEventArgs<TTagType>> TokensBurned;

		public IReadOnlyTaggedTokens<TTagType> TaggedBalanceOf(Address tokenHolder)
			=> this.holdersToBalances.ContainsKey(tokenHolder)
				? this.holdersToBalances[tokenHolder]
				: new TaggedTokens<TTagType>();

		public IReadOnlyTaggedTokens<TTagType> TaggedTotalBalance() => this.totalBalance;

		protected virtual void OnTokensMinted(IReadOnlyTaggedTokens<TTagType> tokens, Address to)
		{
			var mintedAction = new TokensMintedAction<TTagType>(string.Empty, to, tokens);

			this.TrySendTokenAction(mintedAction);

			var tokensMintedArgs = new TokensMintedEventArgs<TTagType>(tokens, to);
			this.TokensMinted?.Invoke(this, tokensMintedArgs);
		}

		protected virtual void OnTokensTransferred(IReadOnlyTaggedTokens<TTagType> tokens, Address from, Address to)
		{
			var sentAction = new TokensSentAction<TTagType>(string.Empty, from, to, tokens);
			var receivedAction = new TokensReceivedAction<TTagType>(string.Empty, to, from, tokens);

			this.TrySendTokenAction(sentAction);
			this.TrySendTokenAction(receivedAction);

			var transferArgs = new TokensTransferredEventArgs<TTagType>(tokens, from, to);
			this.TokensTransferred?.Invoke(this, transferArgs);
		}

		protected virtual void OnTokensBurned(IReadOnlyTaggedTokens<TTagType> tokens, Address from)
		{
			var burnArgs = new TokensBurnedEventArgs<TTagType>(tokens, from);
			this.TokensBurned?.Invoke(this, burnArgs);
		}

		protected override object GetState()
		{
			return new object();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case MintAction mintAction:
					this.HandleMintAction(mintAction);
					return true;

				case TransferAction<TTagType> transferAction:
					this.HandleTransferAction(transferAction);
					return true;

				case BurnAction<TTagType> burnAction:
					this.HandleBurnAction(burnAction);
					return true;

				default:
					return false;
			}
		}

		private void Mint(BigInteger amount, Address to)
		{
			if (to == null)
			{
				throw new ArgumentNullException(nameof(to));
			}

			TokensUtility.RequirePositiveAmount(amount);

			IReadOnlyTaggedTokens<TTagType> newTokens = this.tokenTagger.Tag(to, amount);
			this.AddToBalances(newTokens, to);

			this.OnTokensMinted(newTokens, to);
		}

		private void Transfer(BigInteger amount, Address from, Address to, ITokenPicker<TTagType> customPicker = null)
		{
			if (from == null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			if (to == null)
			{
				throw new ArgumentNullException(nameof(to));
			}

			if (from.Equals(to))
			{
				throw new ArgumentException("Addresses can't transfer to themselves");
			}

			TokensUtility.RequirePositiveAmount(amount);

			customPicker = customPicker ?? this.defaultTokenPicker;

			IReadOnlyTaggedTokens<TTagType> tokensToTransfer = customPicker.Pick(this.holdersToBalances[from], amount);
			this.RemoveFromBalances(tokensToTransfer, from);
			this.AddToBalances(tokensToTransfer, to);

			this.OnTokensTransferred(tokensToTransfer, from, to);
		}

		private void Burn(BigInteger amount, Address from, ITokenPicker<TTagType> customPicker = null)
		{
			if (from == null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			TokensUtility.RequirePositiveAmount(amount);

			customPicker = customPicker ?? this.defaultTokenPicker;

			IReadOnlyTaggedTokens<TTagType> tokensToBurn = customPicker.Pick(this.holdersToBalances[from], amount);
			this.holdersToBalances[from].RemoveFromBalance(tokensToBurn);

			this.OnTokensBurned(tokensToBurn, from);
		}

		private void AddToBalances(IReadOnlyTaggedTokens<TTagType> tokens, Address holder)
		{
			if (!this.holdersToBalances.ContainsKey(holder))
			{
				this.holdersToBalances[holder] = new TaggedTokens<TTagType>();
			}

			this.holdersToBalances[holder].AddToBalance(tokens);
			this.totalBalance.AddToBalance(tokens);
		}

		private void RemoveFromBalances(IReadOnlyTaggedTokens<TTagType> tokens, Address holder)
		{
			this.holdersToBalances[holder].RemoveFromBalance(tokens);
			this.totalBalance.RemoveFromBalance(tokens);
		}

		private void HandleMintAction(MintAction action)
		{
			this.CheckPermission(action);
			this.Mint(action.Amount, action.To);
		}

		private void HandleTransferAction(TransferAction<TTagType> action)
		{
			if (!action.Sender.Equals(action.From))
			{
				this.CheckPermission(action);
			}

			this.Transfer(
				action.Amount,
				action.From,
				action.To,
				action.CustomPicker);
		}

		private void HandleBurnAction(BurnAction<TTagType> action)
		{
			if (!action.Sender.Equals(action.From))
			{
				this.CheckPermission(action);
			}

			this.Burn(
				action.Amount,
				action.From,
				action.CustomPicker);
		}

		public virtual void TrySendTokenAction(Action action)
		{
			try
			{
				this.OnSend(action);
			}
			catch (NoPermissionException e)
			{
				if (typeof(TokenAction<TTagType>).IsAssignableFrom(e.Permission.Type))
				{
					Console.WriteLine(e);
				}
				else
				{
					throw e;
				}
			}
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}
	}
}