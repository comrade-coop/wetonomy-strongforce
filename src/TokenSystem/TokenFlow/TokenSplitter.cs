// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManager;
using TokenSystem.TokenManager.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenSplitter<TTokenTagType> : RecipientManager
	{
		public TokenSplitter(
			Address address,
			TokenManager<TTokenTagType> tokenManager)
			: this(address, tokenManager, new List<Address>())
		{
		}

		public TokenSplitter(
			Address address,
			TokenManager<TTokenTagType> tokenManager,
			IList<Address> recipients)
			: base(address, recipients)
		{
			this.TokenManager = tokenManager;
		}

		protected TokenManager<TTokenTagType> TokenManager { get; }

		protected abstract void Split(IReadOnlyTaggedTokens<TTokenTagType> receivedTokens);

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction<TTokenTagType> tokensReceivedAction:
					this.OnTokensReceived(tokensReceivedAction.Sender, tokensReceivedAction.Tokens);
					return true;
				case TokensMintedAction<TTokenTagType> tokensMintedAction:
					this.OnTokensReceived(tokensMintedAction.Sender, tokensMintedAction.Tokens);
					return true;
				default:
					return false;
			}
		}

		private void OnTokensReceived(Address tokenManagerAddress, IReadOnlyTaggedTokens<TTokenTagType> tokens)
		{
			if (tokenManagerAddress.Equals(this.TokenManager.Address))
			{
				this.Split(tokens);
			}
		}
	}
}