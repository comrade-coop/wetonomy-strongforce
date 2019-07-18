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
		public UniformTokenSplitter(Address address, Address tokenManager)
			: base(address, tokenManager)
		{
		}

		public UniformTokenSplitter(
			Address address,
			Address tokenManager,
			ISet<Address> recipients)
			: base(address, tokenManager, recipients)
		{
		}

		protected override void Split(IReadOnlyTaggedTokens receivedTokens, object options = null)
		{
			BigInteger splitAmount = receivedTokens.TotalBalance / this.Recipients.Count;

			if (splitAmount <= 0)
			{
				return;
			}

			foreach (Address recipient in this.Recipients)
			{
				var transferAction = new TransferAction(
					string.Empty,
					this.TokenManager,
					splitAmount,
					this.Address,
					recipient);
				this.OnSend(transferAction);
			}
		}
	}
}