using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using TaskSystem.Actions;
using TokenSystem.TokenManagerBase;
using WorkTrack;
using WorkTrack.Actions;
using Xunit;

namespace TaskSystem.Tests
{
	public class TaskRegistryTests
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly TokenManager tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly TaskRegistry taskRegistry;
		private readonly SplitterPerTaskHoursMock splitterPerHours;
		private readonly ContractExecutor permissionManager;
		private readonly WorkTracker workTracker;

		public TaskRegistryTests()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.workTracker = new WorkTracker(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			this.tokenManager = new TokenManager(
				this.addressFactory.Create(),
				this.permissionManager.Address,
				this.contractRegistry,
				tokenTagger,
				tokenPicker);
			this.splitterPerHours = new SplitterPerTaskHoursMock(this.addressFactory.Create(), this.tokenManager, this.workTracker);
			this.taskRegistry = new TaskRegistry(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address, this.splitterPerHours.Address);
			this.contractRegistry.RegisterContract(this.taskRegistry);
			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.tokenManager);
			this.contractRegistry.RegisterContract(this.workTracker);
			this.InitializePermissions();
		}

		[Fact]
		public void AddTask()
		{
			var task = this.CreateTask(null);
			var addAction = new AddTaskAction(string.Empty, this.taskRegistry.Address, task);
			this.permissionManager.ExecuteAction(addAction);
			ISet<Address> tasks = this.taskRegistry.GetAllTasks();
			Assert.Contains(task.Address, tasks);
		}

		[Fact]
		public void AddTask_WhenDuplicates_RemainsSingle()
		{
			var task = this.CreateTask(null);
			var addAction = new AddTaskAction(string.Empty, this.taskRegistry.Address, task);
			this.permissionManager.ExecuteAction(addAction);
			addAction = new AddTaskAction(string.Empty, this.taskRegistry.Address, task);
			this.permissionManager.ExecuteAction(addAction);
			IEnumerable<Address> tasks = this.taskRegistry.GetAllTasks().Where(x => x.Equals(task.Address));
			Assert.Single(tasks);
		}

		[Fact]
		public void RemoveTask()
		{
			var task = this.CreateTask(null);
			var addAction = new AddTaskAction(string.Empty, this.taskRegistry.Address, task);
			this.permissionManager.ExecuteAction(addAction);
			var removeAction = new RemoveTaskAction(string.Empty, this.taskRegistry.Address, task.Address);
			this.permissionManager.ExecuteAction(removeAction);
			ISet<Address> tasks = this.taskRegistry.GetAllTasks();
			Assert.DoesNotContain(task.Address, tasks);
		}

		[Fact]
		public void ChangeDefaultSpletter()
		{
			var splitterAddress = this.addressFactory.Create();
			var action = new ChangeDefaultSplitterAction(string.Empty, this.taskRegistry.Address, splitterAddress);
			this.permissionManager.ExecuteAction(action);
			Assert.Equal(splitterAddress, this.taskRegistry.DefaultTokenSplitter);
		}

		private void InitializePermissions()
		{
			var addPermAction = new AddPermissionAction(
				string.Empty,
				this.taskRegistry.Address,
				new Permission(typeof(AddTaskAction)),
				this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPermAction);
			addPermAction = new AddPermissionAction(
				string.Empty,
				this.taskRegistry.Address,
				new Permission(typeof(RemoveTaskAction)),
				this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPermAction);
			addPermAction = new AddPermissionAction(
				string.Empty,
				this.taskRegistry.Address,
				new Permission(typeof(ChangeDefaultSplitterAction)),
				this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPermAction);

			// work traker
			var perm = new Permission(typeof(TrackWorkAction));
			var addPerm = new AddPermissionAction(string.Empty, this.workTracker.Address, perm, this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPerm);
		}

		private Task CreateTask(Address assignee)
		{
			var address = this.addressFactory.Create();
			var issuer = this.addressFactory.Create();

			var managersToBalances = new Dictionary<Address, BigInteger>();
			var header = "First Test Task";
			var description = "The puropose of this task is to test task's fields";
			managersToBalances.Add(this.tokenManager.Address, 0);
			var task = new Task(
				address,
				this.contractRegistry,
				this.permissionManager.Address,
				this.splitterPerHours.Address,
				managersToBalances,
				1,
				issuer,
				header,
				description,
				true,
				null);
			this.contractRegistry.RegisterContract(task);
			return task;
		}
	}
}