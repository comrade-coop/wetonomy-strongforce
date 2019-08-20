// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenSplitter : RecipientManager
	{
		protected Address TokenManager { get; private set; }

		public override IDictionary<string, object> GetState()
		{
			var state = base.GetState();

			state.AddAddress("TokenManager", this.TokenManager);

			return state;
		}

		public override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.TokenManager = state.GetAddress("TokenManager");
		}

		protected override void Initialize(IDictionary<string, object> payload)
		{
			base.Initialize(payload);

			this.TokenManager = payload.GetAddress("TokenManager");
		}

		protected abstract void Split(IReadOnlyTaggedTokens receivedTokens);

		protected override bool HandleEventAction(EventAction action)
		{
			switch (action.Type)
			{
				case TokensReceivedEvent.Type:
					var tokens = action.Payload.GetDictionary(TokensReceivedEvent.Tokens);
					this.OnTokensReceived(action.Sender, new ReadOnlyTaggedTokens(tokens));
					return true;
				default:
					return base.HandleEventAction(action);
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