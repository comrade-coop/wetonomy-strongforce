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
		protected ISet<Address> Recipients { get; set; } = new HashSet<Address>();

		public override IDictionary<string, object> GetState()
		{
			var state = base.GetState();

			state.Set("Recipients", this.Recipients);

			return state;
		}

		protected override void SetState(IDictionary<string, object> state)
		{
			base.SetState(state);

			this.Recipients = new HashSet<Address>(state.GetList<Address>("Recipients"));
		}

		protected bool AddRecipient(Address recipient)
		{
			return this.Recipients.Add(recipient);
		}

		protected bool RemoveRecipient(Address recipient)
		{
			return this.Recipients.Remove(recipient);
		}
	}
}