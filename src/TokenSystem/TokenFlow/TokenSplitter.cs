// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using StrongForce.Core.Permissions;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenSplitter : RecipientManager
	{
		protected abstract void Split(Address tokenManager, IReadOnlyTaggedTokens availableTokens);

		protected override void Initialize(IDictionary<string, object> payload)
		{
			this.Acl.AddPermission(AccessControlList.AnyAddress, TokensReceivedEvent.Type, this.Address);

			base.Initialize(payload);
		}

		protected override void HandleMessage(Message message)
		{
			switch (message.Type)
			{
				case TokensReceivedEvent.Type:
					var tokens = message.Payload.GetDictionary(TokensReceivedEvent.TokensTotal);
					this.Split(message.Sender, new ReadOnlyTaggedTokens(tokens));
					break;
				default:
					base.HandleMessage(message);
					return;
			}
		}
	}
}