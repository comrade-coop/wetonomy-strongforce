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
	public class UniformTokenSplitter : TokenSplitter
	{
		protected BigInteger LeftoverAmount { get; set; } = 0;

		public override IDictionary<string, object> GetState()
		{
			var state = base.GetState();

			state.Add("LeftoverAmount", this.LeftoverAmount);

			return state;
		}

		public override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.LeftoverAmount = BigInteger.Parse(state.GetString("LeftoverAmount"));
		}

		protected override void Split(IReadOnlyTaggedTokens receivedTokens)
		{
			BigInteger amount = this.LeftoverAmount + receivedTokens.TotalBalance;
			BigInteger splitAmount = amount / this.Recipients.Count;
			this.LeftoverAmount = amount % this.Recipients.Count;

			if (splitAmount <= 0)
			{
				return;
			}

			foreach (Address recipient in this.Recipients)
			{
				this.SendAction(this.TokenManager, TransferAction.Type, new Dictionary<string, object>()
				{
					{ TransferAction.To, recipient.ToBase64String() },
					{ TransferAction.Amount, splitAmount.ToString() },
				});
			}
		}
	}
}