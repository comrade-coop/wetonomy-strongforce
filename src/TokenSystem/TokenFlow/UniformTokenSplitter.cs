// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Events;
using TokenSystem.TokenManager;
using TokenSystem.TokenManager.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public class UniformTokenSplitter<TTokenTagType> : TokenSplitter<TTokenTagType>
	{
		public UniformTokenSplitter(Address address, TokenManager<TTokenTagType> tokenManager)
			: base(address, tokenManager)
		{
		}

		public UniformTokenSplitter(
			Address address,
			TokenManager<TTokenTagType> tokenManager,
			IList<Address> recipients)
			: base(address, tokenManager, recipients)
		{
		}

		protected override void Split(IReadOnlyTaggedTokens<TTokenTagType> receivedTokens)
		{
			BigInteger splitAmount = receivedTokens.TotalTokens / this.Recipients.Count;

			if (splitAmount <= 0)
			{
				return;
			}

			foreach (Address recipient in this.Recipients)
			{
				TransferAction<TTokenTagType> transferAction = new TransferAction<TTokenTagType>(
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