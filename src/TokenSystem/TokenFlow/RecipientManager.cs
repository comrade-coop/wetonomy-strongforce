// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using StrongForce.Core;
using StrongForce.Core.Extensions;

namespace TokenSystem.TokenFlow
{
	public abstract class RecipientManager : Contract
	{
		public ISet<Address> Recipients { get; private set; } = new HashSet<Address>();

		public override IDictionary<string, object> GetState()
		{
			var state = base.GetState();

			state.Add("Recipients", this.Recipients.Select(x => (object)x.ToBase64String()).ToList());

			return state;
		}

		public override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.Recipients = new HashSet<Address>(
				state.GetList<string>("Recipients").Select(Address.FromBase64String));
		}

		public bool AddRecipient(Address recipient)
		{
			return this.Recipients.Add(recipient);
		}

		public bool RemoveRecipient(Address recipient)
		{
			return this.Recipients.Remove(recipient);
		}

		// protected override bool HandlePayloadAction(PayloadAction action)
	}
}