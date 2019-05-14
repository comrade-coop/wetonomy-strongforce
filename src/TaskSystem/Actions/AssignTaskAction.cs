using System;
using ContractsCore;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem.Actions
{
	public class AssignTaskAction : Action
	{
		public Address Assignee { get; }

		public AssignTaskAction(string hash, Address target, Address assignee) : base(hash, target)
		{
			this.Assignee = assignee ?? throw new NullReferenceException();
		}
	}
}