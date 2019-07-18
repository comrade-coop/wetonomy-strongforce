using System;
using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using TaskSystem.Actions;
using TaskSystem.Exceptions;
using TokenSystem.TokenManagerBase;
using Xunit;

namespace TaskSystem.Tests
{
	public class TaskTest
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly TokenManager tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;

		public TaskTest()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.tokenManager = new TokenManager(
				this.addressFactory.Create(),
				this.permissionManager.Address,
				this.contractRegistry,
				tokenTagger,
				tokenPicker);
			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.tokenManager);
		}

		[Fact]
		public void InitializeTask()
		{
			var address = this.addressFactory.Create();
			var receiver = this.addressFactory.Create();
			var issuer = this.addressFactory.Create();
			var assignee = this.addressFactory.Create();
			var managersToBalances = new Dictionary<Address, BigInteger>();
			var header = "First Test Task";
			var description = "The puropose of this task is to test task's fields";
			managersToBalances.Add(this.tokenManager.Address, 0);
			var task = new Task(address, this.contractRegistry, this.permissionManager.Address, receiver, managersToBalances, 1, issuer,
				header, description, true, assignee);

			Assert.Equal(address, task.Address);
			Assert.Equal(receiver, task.TokenReceiver);
			Assert.Equal(managersToBalances, task.TokenManagersToBalances);
			Assert.Equal(1U, task.Id);
			Assert.Equal(TaskStage.Unassigned, task.Stage);
			Assert.Equal(issuer, task.Issuer);
			Assert.Equal(header, task.Header);
			Assert.Equal(description, task.Description);
			Assert.Equal(assignee, task.Assignee);
		}

		[Fact]
		public void UpdateTask()
		{
			var assignee = this.addressFactory.Create();
			var task = this.CreateTask(null);

			var managersToBalances = new Dictionary<Address, BigInteger>();
			var header = "Updated Tast Task";
			var description = "The puropose of this task is to UPDATE task's fields";
			managersToBalances.Add(this.tokenManager.Address, 0);

			var action = new UpdateTaskAction(string.Empty, task.Address, header, description, TaskStage.Assigned, false, assignee);
			this.permissionManager.ExecuteAction(action);
			Assert.Equal(managersToBalances, task.TokenManagersToBalances);
			Assert.Equal(1U, task.Id);
			Assert.Equal(header, task.Header);
			Assert.Equal(description, task.Description);
			Assert.Equal(assignee, task.Assignee);
			Assert.Equal(TaskStage.Assigned, task.Stage);
		}

		[Fact]
		public void UpdateTask_WithInvalidStage_ThrowsExeption()
		{
			var assignee = this.addressFactory.Create();
			var task = this.CreateTask(null);

			var managersToBalances = new Dictionary<Address, BigInteger>();
			var header = "Updated Tast Task";
			var description = "The puropose of this task is to UPDATE task's fields";
			managersToBalances.Add(this.tokenManager.Address, 0);

			var action = new UpdateTaskAction(string.Empty, task.Address, header, description, TaskStage.Assigned, false, null);
			Assert.Throws<TaskUnassignedException>(() => this.permissionManager.ExecuteAction(action));
		}

		[Fact]
		public void AssignTask_WhenAlreadyInitialized_SetsAssignee()
		{
			var assignee = this.addressFactory.Create();
			var task = this.CreateTask(null);
			var action = new AssignTaskAction(string.Empty, task.Address, assignee);
			this.permissionManager.ExecuteAction(action);
			Assert.Equal(assignee, task.Assignee);
			Assert.Equal(TaskStage.Assigned, task.Stage);
		}

		[Fact]
		public void ChangeStage_WhenAssigned_RemoveAssignee()
		{
			var assignee = this.addressFactory.Create();
			var task = this.CreateTask(assignee);
			var action = new ChangeStageAction(string.Empty, task.Address, TaskStage.Unassigned);
			this.permissionManager.ExecuteAction(action);
			Assert.Null(task.Assignee);
			Assert.Equal(TaskStage.Unassigned, task.Stage);
		}

		[Fact]
		public void ChangeStage_WhenUnAssigned_ThrowsException()
		{
			var assignee = this.addressFactory.Create();
			var task = this.CreateTask(null);
			var action = new ChangeStageAction(string.Empty, task.Address, TaskStage.Assigned);
			Assert.Throws<TaskUnassignedException>(() => this.permissionManager.ExecuteAction(action));
		}

		[Fact]
		public void ChangeRewardReceiver()
		{
			var receiver = this.addressFactory.Create();
			var task = this.CreateTask(null);
			var action = new ChnageTaskRewardReceiverAction(string.Empty, task.Address, receiver);
			this.permissionManager.ExecuteAction(action);
			Assert.Equal(receiver, task.TokenReceiver);
		}

		[Fact]
		public void ChangeRewardReceiver_WhenInvalidReceiver_ThrowsException()
		{
			var task = this.CreateTask(null);
			var action = new ChnageTaskRewardReceiverAction(string.Empty, task.Address, task.Address);
			Assert.Throws<UnallowedRewardReceiverException>(() => this.permissionManager.ExecuteAction(action));
		}

		[Fact]
		public void AddTask_AsSubtask()
		{
			var task = this.CreateTask(null);
			var subTask = this.CreateTask(null);
			var action = new AddTaskAction(string.Empty, task.Address, subTask);
			this.permissionManager.ExecuteAction(action);
			Assert.Contains(subTask.Address, task.SubTasks);
		}

		[Fact]
		public void AddTask_CreateRecursiveSubtask_ThrowsExeption()
		{
			var task = this.CreateTask(null);
			var action = new AddTaskAction(string.Empty, task.Address, task);
			Assert.Throws<ArgumentException>(() => this.permissionManager.ExecuteAction(action));
		}

		[Fact]
		public void RemoveSubtask()
		{
			var task = this.CreateTask(null);
			var subTask = this.CreateTask(null);
			var action = new AddTaskAction(string.Empty, task.Address, subTask);
			this.permissionManager.ExecuteAction(action);
			Assert.Contains(subTask.Address, task.SubTasks);
			var removeAction = new RemoveTaskAction(string.Empty, task.Address, subTask.Address);
			this.permissionManager.ExecuteAction(removeAction);
			Assert.DoesNotContain(subTask.Address, task.SubTasks);
		}

		[Fact]
		public void RemoveSubtask_WhenNullIsSubtaskAddress_ThrowsExeption()
		{
			var task = this.CreateTask(null);
			Assert.Throws<NullReferenceException>(() => new RemoveTaskAction(string.Empty, task.Address, null));
		}

		private Task CreateTask(Address assignee)
		{
			var address = this.addressFactory.Create();
			var receiver = this.addressFactory.Create();
			var issuer = this.addressFactory.Create();

			var managersToBalances = new Dictionary<Address, BigInteger>();
			var header = "First Test Task";
			var description = "The puropose of this task is to test task's fields";
			managersToBalances.Add(this.tokenManager.Address, 0);
			var task = new Task(address, this.contractRegistry, this.permissionManager.Address, receiver, managersToBalances, 1, issuer,
				header, description, true, null);
			this.contractRegistry.RegisterContract(task);
			return task;
		}

		// Tests for dumping tokens are in SplitterTests
	}
}