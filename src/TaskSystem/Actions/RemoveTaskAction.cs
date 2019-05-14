using System;
using ContractsCore;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem.Actions
{
	public class RemoveTaskAction : Action
	{
		public Address TaskAddress { get; }

		public RemoveTaskAction(string hash, Address target, Address taskAddress)
			: base(hash, target)
		{
			this.TaskAddress = taskAddress ?? throw new NullReferenceException();
		}
	}
}