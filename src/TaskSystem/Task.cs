using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Permissions;
using TaskSystem.Actions;
using TaskSystem.Exceptions;
using TokenSystem.TokenManagerBase.Actions;
using Action = ContractsCore.Actions.Action;

namespace TaskSystem
{
	public enum TaskStage
	{
		Unassigned,
		Assigned,
		InProgress,
		Done,
		Finalized,
		Archived
	}

	public class Task : TokenRelay
	{
		public readonly uint Id;

		public readonly Address Issuer;

		public Address Assignee { get; private set; }

		public TaskStage Stage { get; private set; }

		public string Header { get; private set; }

		public string Description { get; private set; }

		public HashSet<Address> SubTasks { get; private set; }

		public bool AutoDump { get; private set; }

		public Task(Address address, ContractRegistry registry, Address permissionManager, Address receiver, Dictionary<Address, BigInteger> tokenManagers,
			uint id, Address issuer, string header, string description, bool autoDump = false, Address assignee = null, HashSet<Address> tasks = null)
			: base(address, registry, permissionManager, receiver, tokenManagers)
		{
			this.Id = id;
			this.Issuer = issuer ?? throw new NullReferenceException();
			this.Header = header;
			this.Description = description;
			this.SubTasks = tasks ?? new HashSet<Address>();
			this.AutoDump = autoDump;
			this.Assignee = assignee;
			if (this.Assignee != null)
			{
				this.Stage = TaskStage.Unassigned;
			}
			else
			{
				this.Stage = TaskStage.Assigned;
			}

			if (this.AutoDump)
			{
				this.InitializePermissionForAutoDump();
			}

			this.ConfigurePermissionManager(permissionManager);
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case AssignTaskAction assignAction:
					return this.HandleAssignTask(assignAction);

				case ChangeStageAction changeAction:
					return this.HandleChangeStage(changeAction);

				case UpdateTaskAction updateAction:
					return this.HandleUpdateTask(updateAction);

				case AddTaskAction addAction:
					return this.HandleAddTask(addAction);

				case RemoveTaskAction removeAction:
					return this.HandleRemoveTask(removeAction);

				case DumpTokensAction dumpAction:
					return this.HandleDumpTokens(dumpAction);

				default: return base.HandleReceivedAction(action);
			}
		}

		protected override bool HandleDumpTokens(DumpTokensAction action)
		{
			if (this.Stage >= TaskStage.Finalized)
			{
				throw new TaskNotInRequiredStageException(this, typeof(DumpTokensAction));
			}

			base.HandleDumpTokens(action);

			return true;
		}

		protected bool HandleAssignTask(AssignTaskAction action)
		{
			this.Assignee = action.Assignee ?? throw new NullReferenceException();
			return true;
		}

		protected bool HandleChangeStage(ChangeStageAction action)
		{
			switch (action.Stage)
			{
				case TaskStage.Unassigned:
					this.Assignee = null;
					break;

				case TaskStage.Assigned:
					if (this.Assignee == null)
					{
						throw new TaskUnassignedException(this);
					}
					break;
				// TODO test
				case TaskStage.Finalized:
					if (this.AutoDump)
					{
						this.OnSend(new DumpTokensAction(string.Empty, this.Address));
					}
					break;

				default: break;
			}

			this.Stage = action.Stage;
			return true;
		}

		protected bool HandleUpdateTask(UpdateTaskAction action)
		{
			this.Header = action.Header;
			this.Description = action.Description;
			this.SubTasks = action.Tasks;
			this.Assignee = action.Assignee;
			this.HandleChangeStage(new ChangeStageAction(string.Empty, this.Address, action.Stage));
			return true;
		}

		protected bool HandleAddTask(AddTaskAction action)
		{
			var task = action.Task;
			if (task.Address == null || task.Address.CompareTo(this.Address) == 0)
			{
				throw new ArgumentException();
			}

			return this.SubTasks.Add(action.Task.Address);
		}

		protected bool HandleRemoveTask(RemoveTaskAction action)
		{
			if (action.TaskAddress == null)
			{
				throw new NullReferenceException();
			}

			return this.SubTasks.Remove(action.TaskAddress);
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}

		private void InitializePermissionForAutoDump()
		{
			var permission = new Permission(typeof(DumpTokensAction));
			this.acl.AddPermission(this.Address, permission, this.Address);
		}

		private void ConfigurePermissionManager(Address permissionManager)
		{
			this.acl.AddPermission(permissionManager, new Permission(typeof(AssignTaskAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(ChangeStageAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(UpdateTaskAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(AddTaskAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(RemoveTaskAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(DumpTokensAction)), this.Address);
			this.acl.AddPermission(permissionManager, new Permission(typeof(ChnageTaskRewardReceiverAction)), this.Address);
		}
	}
}