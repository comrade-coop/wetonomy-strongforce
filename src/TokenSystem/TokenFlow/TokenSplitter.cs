// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenSplitter : RecipientManager
	{
		public TokenSplitter(
			Address address,
			Address tokenManager)
			: this(address, tokenManager, new HashSet<Address>())
		{
		}

		public TokenSplitter(
			Address address,
			Address tokenManager,
			ISet<Address> recipients)
			: base(address, recipients)
		{
			this.TokenManager = tokenManager;
		}

		protected Address TokenManager { get; }

		protected abstract void Split(IReadOnlyTaggedTokens receivedTokens, object options = null);

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction tokensReceivedAction:
					this.OnTokensReceived(tokensReceivedAction.Sender, tokensReceivedAction.Tokens);
					return true;
				case TokensMintedAction tokensMintedAction:
					this.OnTokensReceived(tokensMintedAction.Sender, tokensMintedAction.Tokens);
					return true;
				default:
					return false;
			}
		}

		private void OnTokensReceived(Address tokenManagerAddress, IReadOnlyTaggedTokens tokens)
		{
			if (tokenManagerAddress.Equals(this.TokenManager))
			{
				this.Split(tokens);
			}
		}
	}
}