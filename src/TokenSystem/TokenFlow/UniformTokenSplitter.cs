// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Events;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public class UniformTokenSplitter : TokenSplitter
	{
		public UniformTokenSplitter(Address address, TokenManager tokenManager)
			: base(address, tokenManager)
		{
		}

		public UniformTokenSplitter(
			Address address,
			TokenManager tokenManager,
			IList<Address> recipients)
			: base(address, tokenManager, recipients)
		{
		}

		protected override void Split(IReadOnlyTaggedTokens receivedTokens, object options = null)
		{
			BigInteger splitAmount = receivedTokens.TotalTokens / this.Recipients.Count;

			if (splitAmount <= 0)
			{
				return;
			}

			foreach (Address recipient in this.Recipients)
			{
				TransferAction transferAction = new TransferAction(
					string.Empty,
					this.TokenManager.Address,
					splitAmount,
					this.Address,
					recipient);
				this.OnSend(transferAction);
			}
		}
	}
}