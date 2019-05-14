using System;
using ContractsCore;
using ContractsCore.Contracts;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem.Tests
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