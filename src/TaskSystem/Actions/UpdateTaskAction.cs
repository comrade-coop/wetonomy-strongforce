using System;
using System.Collections.Generic;
using ContractsCore;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem.Actions
{
	public class UpdateTaskAction : Action
	{
		public Address Assignee { get; }

		public TaskStage Stage { get; }

		public string Header { get; }

		public string Description { get; }

		public HashSet<Address> Tasks { get; }

		public bool AutoDump { get; }

		public UpdateTaskAction(
			string hash,
			Address target,
			string header,
			string description,
			TaskStage stage,
			bool autoDump = false,
			Address assignee = null,
			HashSet<Address> tasks = null)
			: base(hash, target)
		{
			this.Header = header;
			this.Description = description;
			this.AutoDump = autoDump;
			this.Assignee = assignee;
			this.Stage = stage;
		}
	}
}