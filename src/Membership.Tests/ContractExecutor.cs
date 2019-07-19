using ContractsCore;
using ContractsCore.Contracts;
using System;
using Action = ContractsCore.Actions.Action;

namespace Membership.Tests
{
	internal class ContractExecutor : Contract
	{
		public ContractExecutor(Address address)
			: base(address)
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
			return false;
		}
	}
}