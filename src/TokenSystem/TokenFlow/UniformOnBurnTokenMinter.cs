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
	public class UniformOnBurnTokenMinter : OnBurnTokenMinter
	{
		public UniformOnBurnTokenMinter(Address address, TokenManager tokenManager)
			: base(address, tokenManager)
		{
		}

		public UniformOnBurnTokenMinter(
			Address address,
			TokenManager tokenManager,
			IList<Address> recipients)
			: base(address, tokenManager, recipients)
		{
		}

		protected override void Mint(BigInteger transferAmount)
		{
			BigInteger mintAmountPerRecipient = transferAmount / this.Recipients.Count;

			foreach (Address recipient in this.Recipients)
			{
				var mintAction = new MintAction(
					string.Empty,
					this.TokenManager.Address,
					mintAmountPerRecipient,
					recipient);
				this.OnSend(mintAction);
			}
		}
	}
}