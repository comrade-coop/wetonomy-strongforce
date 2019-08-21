// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using StrongForce.Core.Permissions;
using TokenSystem.Exceptions;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase
{
	public class TokenManager : Contract
	{
		private ITokenTagger tokenTagger;
		private ITokenPicker defaultTokenPicker;

		private IDictionary<Address, ITaggedTokens> holdersToBalances
			= new Dictionary<Address, ITaggedTokens>();

		public override IDictionary<string, object> GetState()
		{
			var state = base.GetState();

			state.Add("Balances", this.holdersToBalances.ToDictionary(
				kv => kv.Key.ToBase64String(),
				kv => (object)kv.Value.GetState()));

			state.Add("Tagger", this.tokenTagger?.ToState());
			state.Add("DefaultPicker", this.defaultTokenPicker?.ToState());
			return state;
		}

		public override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.holdersToBalances = state.GetDictionary("Balances").ToDictionary(
				kv => Address.FromBase64String(kv.Key),
				kv => (ITaggedTokens)new TaggedTokens((IDictionary<string, object>)kv.Value));

			this.tokenTagger = (ITokenTagger)state.GetDictionary("Tagger")?.ToStateObject();

			this.defaultTokenPicker = (ITokenPicker)state.GetDictionary("DefaultPicker")?.ToStateObject();
		}

		protected override void Initialize(IDictionary<string, object> payload)
		{
			if (payload.ContainsKey("User"))
			{
				this.Acl.AddPermission(
					payload.GetAddress("User"),
					BurnAction.Type,
					this.Address);

				this.Acl.AddPermission(
					payload.GetAddress("User"),
					TransferAction.Type,
					this.Address);
			}

			base.Initialize(payload);
		}

		protected override bool HandlePayloadAction(PayloadAction action)
		{
			switch (action.Type)
			{
				case MintAction.Type:
					this.Mint(
						BigInteger.Parse(action.Payload.GetString(MintAction.Amount)),
						action.Payload.GetAddress(MintAction.To));
					return true;

				case TransferAction.Type:
					this.Transfer(
						BigInteger.Parse(action.Payload.GetString(TransferAction.Amount)),
						action.Origin,
						action.Payload.GetAddress(TransferAction.To),
						(ITokenPicker)action.Payload.GetDictionary(TransferAction.Picker)?.ToStateObject());
					return true;

				case BurnAction.Type:
					this.Burn(
						BigInteger.Parse(action.Payload.GetString(BurnAction.Amount)),
						action.Origin,
						(ITokenPicker)action.Payload.GetDictionary(BurnAction.Picker)?.ToStateObject());
					return true;

				case BurnOtherAction.Type:
					this.Burn(
						BigInteger.Parse(action.Payload.GetString(BurnOtherAction.Amount)),
						action.Payload.GetAddress(BurnOtherAction.From),
						(ITokenPicker)action.Payload.GetDictionary(BurnOtherAction.Picker)?.ToStateObject());
					return true;

				default:
					return base.HandlePayloadAction(action);
			}
		}

		private void Mint(BigInteger amount, Address to)
		{
			if (amount <= 0)
			{
				throw new NonPositiveTokenAmountException(nameof(amount), amount);
			}

			IReadOnlyTaggedTokens newTokens = this.tokenTagger.Tag(to, amount);
			this.AddToBalances(newTokens, to);
			this.NotifyReceived(newTokens, null, to);
		}

		private void Transfer(BigInteger amount, Address from, Address to, ITokenPicker customPicker = null)
		{
			if (from.Equals(to))
			{
				throw new ArgumentException("Addresses can't transfer to themselves");
			}

			if (amount <= 0)
			{
				throw new NonPositiveTokenAmountException(nameof(amount), amount);
			}

			customPicker = customPicker ?? this.defaultTokenPicker;

			IReadOnlyTaggedTokens tokensToTransfer = customPicker.Pick(this.GetBalances(from), amount);

			this.RemoveFromBalances(tokensToTransfer, from);
			this.AddToBalances(tokensToTransfer, to);
			this.NotifyReceived(tokensToTransfer, from, to);
		}

		private void Burn(BigInteger amount, Address from, ITokenPicker customPicker = null)
		{
			if (amount <= 0)
			{
				throw new NonPositiveTokenAmountException(nameof(amount), amount);
			}

			customPicker = customPicker ?? this.defaultTokenPicker;

			IReadOnlyTaggedTokens tokensToBurn = customPicker.Pick(this.GetBalances(from), amount);
			this.RemoveFromBalances(tokensToBurn, from);
		}

		private void NotifyReceived(IReadOnlyTaggedTokens tokens, Address from, Address to)
		{
			this.SendEvent(to, TokensReceivedEvent.Type, new Dictionary<string, object>()
			{
				{ TokensReceivedEvent.TokensTransfered, tokens.GetState() },
				{ TokensReceivedEvent.TokensTotal, this.GetBalances(to).GetState() },
				{ TokensReceivedEvent.From, from?.ToBase64String() },
			});
		}

		private void AddToBalances(IReadOnlyTaggedTokens tokens, Address holder)
		{
			if (!this.holdersToBalances.ContainsKey(holder))
			{
				this.holdersToBalances[holder] = new TaggedTokens();
			}

			this.holdersToBalances[holder].AddToBalance(tokens);
		}

		private void RemoveFromBalances(IReadOnlyTaggedTokens tokens, Address holder)
		{
			this.holdersToBalances[holder].RemoveFromBalance(tokens);

			if (this.holdersToBalances[holder].TotalBalance == 0)
			{
				this.holdersToBalances.Remove(holder);
			}
		}

		private IReadOnlyTaggedTokens GetBalances(Address holder)
		{
			if (!this.holdersToBalances.TryGetValue(holder, out ITaggedTokens totalHolderTokens))
			{
				totalHolderTokens = new TaggedTokens();
			}

			return totalHolderTokens;
		}
	}
}