// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using System.Numerics;
using StrongForce.Core;
using StrongForce.Core.Extensions;
using StrongForce.Core.Permissions;
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

			state.Set("ExchangeRateNumerator", this.ExchangeRateNumerator);
			state.Set("ExchangeRateDenominator", this.ExchangeRateDenominator);

			return state;
		}

		protected override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.ExchangeRateNumerator = state.Get<int>("ExchangeRateNumerator");
			this.ExchangeRateDenominator = state.Get<int>("ExchangeRateDenominator");
		}

		protected override void Initialize(IDictionary<string, object> payload)
		{
			this.Acl.AddPermission(AccessControlList.AnyAddress, TokensReceivedEvent.Type, this.Address);

			base.Initialize(payload);
		}

		protected override void HandleMessage(Message message)
		{
			switch (message.Type)
			{
				case ExchangeAction.Type:
					this.Exchange(
						message.Origin,
						BigInteger.Parse(message.Payload.Get<string>(ExchangeAction.Amount)),
						message.Payload.Get<Address>(ExchangeAction.FromTokenManager),
						message.Payload.Get<Address>(ExchangeAction.ToTokenManager),
						message.Payload.GetDictionary(ExchangeAction.Picker));
					break;
				default:
					base.HandleMessage(message);
					return;
			}
		}

		protected void Exchange(Address targetAddress, BigInteger amount, Address burnTokenManager, Address mintTokenManager, IDictionary<string, object> picker)
		{
			var exchangedAmount = amount / this.ExchangeRateDenominator;
			var burnAmount = exchangedAmount * this.ExchangeRateDenominator;
			var mintAmount = exchangedAmount * this.ExchangeRateNumerator;

			this.SendMessage(burnTokenManager, BurnOtherAction.Type, new Dictionary<string, object>()
			{
				{ BurnOtherAction.From, targetAddress.ToString() },
				{ BurnOtherAction.Amount, burnAmount.ToString() },
				{ BurnOtherAction.Picker, picker },
			});

			this.SendMessage(mintTokenManager, MintAction.Type, new Dictionary<string, object>()
			{
				{ MintAction.To, targetAddress.ToString() },
				{ MintAction.Amount, mintAmount.ToString() },
			});
		}
	}
}