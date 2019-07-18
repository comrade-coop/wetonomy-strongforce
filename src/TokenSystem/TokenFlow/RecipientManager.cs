// Copyright (c) Comrade Coop. All rights reserved.

using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using Action = ContractsCore.Actions.Action;

namespace TokenSystem.TokenFlow
{
	public abstract class RecipientManager : Contract
	{
		private readonly IList<Address> recipients;

		public RecipientManager(Address address)
			: this(address, new List<Address>())
		{
		}

		public RecipientManager(Address address, IList<Address> recipients)
			: base(address)
		{
			this.recipients = recipients ?? new List<Address>();
		}

		public IList<Address> Recipients => this.recipients;

		public void AddRecipient(Address recipient)
		{
			if (this.recipients.Contains(recipient))
			{
				return;
			}

			this.recipients.Add(recipient);
		}

		public bool RemoveRecipient(Address recipient)
		{
			return this.recipients.Remove(recipient);
		}

		protected override object GetState()
		{
			return new object();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			throw new NotImplementedException();
		}
	}
}