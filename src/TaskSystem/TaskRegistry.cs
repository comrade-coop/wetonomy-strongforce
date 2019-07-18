using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using ContractsCore.Permissions;
using TaskSystem.Actions;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem
{
	public class TaskRegistry : AclPermittedContract
	{
		protected HashSet<Address> tasksAddresses;

		public Address DefaultTokenSplitter { get; protected set; }

		public TaskRegistry(Address address, ContractRegistry registry, Address permissionManager,
			Address defaultTokenSplitter, HashSet<Address> tasks = null)
			: base(address, registry, permissionManager)
		{
			this.DefaultTokenSplitter = defaultTokenSplitter ?? throw new NullReferenceException();
			this.tasksAddresses = tasks ?? new HashSet<Address>();
		}

		public ISet<Address> GetAllTasks() => this.tasksAddresses;

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case AddTaskAction addAction:
					return this.HandleAddTask(addAction);

				case RemoveTaskAction removeAction:
					return HandleRemoveTask(removeAction);

				case ChangeDefaultSplitterAction changeAction:
					return this.HandleChangeDefaultSplitter(changeAction);

				default: return false;
			}
		}

		protected bool HandleAddTask(AddTaskAction action)
		{
			var task = action.Task;
			if (task.TokenReceiver.CompareTo(this.DefaultTokenSplitter) != 0)
			{
				var changeReceiverAction =
					new ChnageTaskRewardReceiverAction(string.Empty, task.Address, this.DefaultTokenSplitter);
				this.OnSend(changeReceiverAction);
			}

			// maybe should also remove contract from registry
			return this.tasksAddresses.Add(action.Task.Address);
		}

		protected bool HandleRemoveTask(RemoveTaskAction action)
		{
			return this.tasksAddresses.Remove(action.TaskAddress);
		}

		protected bool HandleChangeDefaultSplitter(ChangeDefaultSplitterAction action)
		{
			this.DefaultTokenSplitter = action.Splitter;
			return true;
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}
	}
}