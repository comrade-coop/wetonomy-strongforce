// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using TokenSystem.TokenFlow.Actions;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenFlow
{
	public class TokenExchanger : Contract
	{
		protected int ExchangeRateNumerator { get; set; }

		protected int ExchangeRateDenominator { get; set; }

		public override IDictionary<string, object> GetState()
		{
			var state = base.GetState();

			state.Add("ExchangeRateNumerator", this.ExchangeRateNumerator);
			state.Add("ExchangeRateDenominator", this.ExchangeRateDenominator);

			return state;
		}

		public override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.ExchangeRateNumerator = state.Get<int>("ExchangeRateNumerator");
			this.ExchangeRateDenominator = state.Get<int>("ExchangeRateDenominator");
		}

		protected override bool HandlePayloadAction(PayloadAction action)
		{
			switch (action.Type)
			{
				case ExchangeAction.Type:
					this.Exchange(
						action.Origin,
						BigInteger.Parse(action.Payload.GetString(ExchangeAction.Amount)),
						action.Payload.GetAddress(ExchangeAction.FromTokenManager),
						action.Payload.GetAddress(ExchangeAction.ToTokenManager),
						action.Payload.GetDictionary(ExchangeAction.Picker));
					return true;
				default:
					return base.HandlePayloadAction(action);
			}
		}

		protected void Exchange(Address targetAddress, BigInteger amount, Address burnTokenManager, Address mintTokenManager, IDictionary<string, object> picker)
		{
			var exchangedAmount = amount / this.ExchangeRateDenominator;
			var burnAmount = exchangedAmount * this.ExchangeRateDenominator;
			var mintAmount = exchangedAmount * this.ExchangeRateNumerator;

			this.SendAction(burnTokenManager, BurnOtherAction.Type, new Dictionary<string, object>()
			{
				{ BurnOtherAction.From, targetAddress.ToBase64String() },
				{ BurnOtherAction.Amount, burnAmount.ToString() },
				{ BurnOtherAction.Picker, picker },
			});

			this.SendAction(mintTokenManager, MintAction.Type, new Dictionary<string, object>()
			{
				{ MintAction.To, targetAddress.ToBase64String() },
				{ MintAction.Amount, mintAmount.ToString() },
			});
		}
	}
}