using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Contracts;
using ContractsCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Action = ContractsCore.Actions.Action;

namespace TokenSystem.Tests
{
	class ContractExecutor : Contract
	{
		public ContractExecutor(Address address) : base(address)
		{
		}

		public void ExecuteAction(Action action)
		{
			this.OnSend(action);
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			// throw new NotImplementedException();
			return true;
		}
	}
}