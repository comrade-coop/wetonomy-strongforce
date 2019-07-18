using System;
using ContractsCore;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem.Actions
{
	public class AddTaskAction : Action
	{
		public Task Task { get; }

		public AddTaskAction(string hash, Address target, Task task)
			: base(hash, target)
		{
			this.Task = task ?? throw new NullReferenceException();
		}
	}
}