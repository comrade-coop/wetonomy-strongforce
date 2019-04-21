// Copyright (c) Comrade Coop. All rights reserved.

using System.Collections.Generic;
using ContractsCore;

namespace TokenSystem.TokenFlow
{
	public class RecipientManager
	{
		public RecipientManager()
			: this(new List<Address>())
		{
		}

		public RecipientManager(IList<Address> recipients)
		{
			this.Recipients = recipients;
		}

		public IList<Address> Recipients { get; }

		public void AddRecipient(Address recipient)
		{
			if (this.Recipients.Contains(recipient))
			{
				return;
			}

			this.Recipients.Add(recipient);
		}

		public bool RemoveRecipient(Address recipient)
		{
			return this.Recipients.Remove(recipient);
		}
	}
}