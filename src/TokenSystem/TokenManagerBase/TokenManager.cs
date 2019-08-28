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
		protected IDictionary<Address, ITaggedTokens> Balances { get; set; }
			= new Dictionary<Address, ITaggedTokens>();

		protected ITokenTagger TokenTagger { get; set; } = new FungibleTokenTagger();

		protected ITokenPicker DefaultTokenPicker { get; set; } = new FungibleTokenPicker();

		public override IDictionary<string, object> GetState()
		{
			var state = base.GetState();

			state.Set("Balances", this.Balances.ToDictionary(
				kv => kv.Key.ToString(),
				kv => (object)kv.Value.GetState()));

			// state.Add("Tagger", this.TokenTagger?.ToState());
			// state.Add("DefaultPicker", this.DefaultTokenPicker?.ToState());
			return state;
		}

		protected override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.Balances = state.GetDictionary("Balances").ToDictionary(
				kv => Address.Parse(kv.Key),
				kv => (ITaggedTokens)new TaggedTokens((IDictionary<string, object>)kv.Value));

			// this.TokenTagger = (ITokenTagger)state.GetDictionary("Tagger")?.ToStateObject();

			// this.DefaultTokenPicker = (ITokenPicker)state.GetDictionary("DefaultPicker")?.ToStateObject();
		}

		protected override void Initialize(IDictionary<string, object> payload)
		{
			if (payload.ContainsKey("User"))
			{
				this.Acl.AddPermission(
					payload.Get<Address>("User"),
					BurnAction.Type,
					this.Address);

				this.Acl.AddPermission(
					payload.Get<Address>("User"),
					TransferAction.Type,
					this.Address);
			}

			base.Initialize(payload);
		}

		protected override void HandleMessage(Message message)
		{
			switch (message.Type)
			{
				case MintAction.Type:
					this.Mint(
						BigInteger.Parse(message.Payload.Get<string>(MintAction.Amount)),
						message.Payload.Get<Address>(MintAction.To));
					break;

				case TransferAction.Type:
					this.Transfer(
						BigInteger.Parse(message.Payload.Get<string>(TransferAction.Amount)),
						message.Origin,
						message.Payload.Get<Address>(TransferAction.To));
					break;

				case BurnAction.Type:
					this.Burn(
						BigInteger.Parse(message.Payload.Get<string>(BurnAction.Amount)),
						message.Origin);
					break;

				case BurnOtherAction.Type:
					this.Burn(
						BigInteger.Parse(message.Payload.Get<string>(BurnOtherAction.Amount)),
						message.Payload.Get<Address>(BurnOtherAction.From));
					break;

				default:
					base.HandleMessage(message);
					return;
			}
		}

		private void Mint(BigInteger amount, Address to)
		{
			if (amount <= 0)
			{
				throw new NonPositiveTokenAmountException(nameof(amount), amount);
			}

			IReadOnlyTaggedTokens newTokens = this.TokenTagger.Tag(to, amount);
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

			customPicker = customPicker ?? this.DefaultTokenPicker;

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

			customPicker = customPicker ?? this.DefaultTokenPicker;

			IReadOnlyTaggedTokens tokensToBurn = customPicker.Pick(this.GetBalances(from), amount);
			this.RemoveFromBalances(tokensToBurn, from);
		}

		private void NotifyReceived(IReadOnlyTaggedTokens tokens, Address from, Address to)
		{
			this.SendMessage(to, TokensReceivedEvent.Type, new Dictionary<string, object>()
			{
				{ TokensReceivedEvent.TokensTransfered, tokens.GetState() },
				{ TokensReceivedEvent.TokensTotal, this.GetBalances(to).GetState() },
				{ TokensReceivedEvent.From, from?.ToString() },
			});
		}

		private void AddToBalances(IReadOnlyTaggedTokens tokens, Address holder)
		{
			if (!this.Balances.ContainsKey(holder))
			{
				this.Balances[holder] = new TaggedTokens();
			}

			this.Balances[holder].AddToBalance(tokens);
		}

		private void RemoveFromBalances(IReadOnlyTaggedTokens tokens, Address holder)
		{
			this.Balances[holder].RemoveFromBalance(tokens);

			if (this.Balances[holder].TotalBalance == 0)
			{
				this.Balances.Remove(holder);
			}
		}

		private IReadOnlyTaggedTokens GetBalances(Address holder)
		{
			if (!this.Balances.TryGetValue(holder, out ITaggedTokens totalHolderTokens))
			{
				totalHolderTokens = new TaggedTokens();
			}

			return totalHolderTokens;
		}
	}
}