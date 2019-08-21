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
		protected abstract void Split(Address tokenManager, IReadOnlyTaggedTokens availableTokens);

		protected override bool HandleEventAction(EventAction action)
		{
			switch (action.Type)
			{
				case TokensReceivedEvent.Type:
					var tokens = action.Payload.GetDictionary(TokensReceivedEvent.TokensTotal);
					this.Split(action.Sender, new ReadOnlyTaggedTokens(tokens));
					return true;
				default:
					return base.HandleEventAction(action);
			}
		}
	}
}