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
		public RecipientManager(Address address)
			: this(address, new HashSet<Address>())
		{
		}

		public RecipientManager(Address address, ISet<Address> recipients)
			: base(address)
		{
			this.Recipients = recipients;
		}

		public ISet<Address> Recipients { get; }

		public bool AddRecipient(Address recipient)
		{
			return this.Recipients.Add(recipient);
		}

		public bool RemoveRecipient(Address recipient)
		{
			return this.Recipients.Remove(recipient);
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			throw new NotImplementedException();
		}
	}
}