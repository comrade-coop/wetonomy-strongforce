using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using TaskSystem.Actions;
using TokenSystem.TokenManagerBase;
using TokenSystem.TokenManagerBase.Actions;
using WorkTracker.Actions;
using Xunit;

namespace TaskSystem.Tests
{
	public class SplitterPerTaskHoursTests
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly TokenManager tokenManager;
		private readonly ContractRegistry contractRegistry;
		private readonly TaskRegistry taskRegistry;
		private readonly SplitterPerTaskHoursMock splitterPerHours;
		private readonly ContractExecutor permissionManager;
		private readonly WorkTracker.WorkTracker workTracker;

		public SplitterPerTaskHoursTests()
		{
			this.contractRegistry = new ContractRegistry();
			var tokenTagger = new FungibleTokenTagger();
			var tokenPicker = new FungibleTokenPicker();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.workTracker = new WorkTracker.WorkTracker(
				this.addressFactory.Create(),
				this.contractRegistry,
				this.permissionManager.Address);
			this.tokenManager = new TokenManager(
				this.addressFactory.Create(),
				this.permissionManager.Address,
				this.contractRegistry,
				tokenTagger,
				tokenPicker);
			this.splitterPerHours =
				new SplitterPerTaskHoursMock(this.addressFactory.Create(), this.tokenManager.Address, this.workTracker);
			this.taskRegistry = new TaskRegistry(
				this.addressFactory.Create(),
				this.contractRegistry,
				this.permissionManager.Address,
				this.splitterPerHours.Address);
			this.contractRegistry.RegisterContract(this.taskRegistry);
			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.tokenManager);
			this.contractRegistry.RegisterContract(this.workTracker);
			this.contractRegistry.RegisterContract(this.splitterPerHours);
			this.InitializePermissions();
		}

		[Fact]
		public void TrackWorkForTask()
		{
			Task task = this.CreateTask(null);
			Address employee = this.addressFactory.Create();
			var employeeHours = this.TrackHours_ReturnsSplitterTrack(employee, task.Address, 5);

			Assert.Equal(5M, employeeHours);
		}

		[Fact]
		public void TrackWorkForTask_WhenMultipleEmployees()
		{
			var task = this.CreateTask(null);
			var employee1 = this.addressFactory.Create();
			var employee2 = this.addressFactory.Create();
			var employee3 = this.addressFactory.Create();

			var employee1Hours = this.TrackHours_ReturnsSplitterTrack(employee1, task.Address, 5);
			var employee2Hours = this.TrackHours_ReturnsSplitterTrack(employee2, task.Address, 15);
			var employee3Hours = this.TrackHours_ReturnsSplitterTrack(employee3, task.Address, 20);

			var hours = this.splitterPerHours.GetTasksAddresToEmployeesHoursPerAddress();
			var employeesHours = hours.FirstOrDefault(x => x.Key.Equals(task.Address)).Value.Sum(x => x.Value);

			Assert.Equal(5M, employee1Hours);
			Assert.Equal(15M, employee2Hours);
			Assert.Equal(20M, employee3Hours);
			Assert.Equal(40M, employee1Hours + employee2Hours + employee3Hours);
		}

		[Fact]
		public void ReceiveTransferredTokens()
		{
			var task = this.CreateTask(null);
			this.CreateTokenInteractionPermissions(task.Address);

			this.permissionManager.ExecuteAction(
				new MintAction(string.Empty, this.tokenManager.Address, 160));

			var transferAction =
				new TransferAction(string.Empty, this.tokenManager.Address, 160, task.Address);
			this.permissionManager.ExecuteAction(transferAction);

			Assert.Equal(160, task.TokenManagersToBalances[this.tokenManager.Address]);
		}

		[Fact]
		public void SplitReward_AccordingToTheHours()
		{
			var task = this.CreateTask(null);
			this.CreateTokenInteractionPermissions(task.Address);

			var mintAction = new MintAction(string.Empty, this.tokenManager.Address, 160);
			this.permissionManager.ExecuteAction(mintAction);
			this.permissionManager.ExecuteAction(
				new TransferAction(
					string.Empty,
					this.tokenManager.Address,
					160,
					task.Address));

			var employee1 = this.addressFactory.Create();
			var employee2 = this.addressFactory.Create();
			var employee3 = this.addressFactory.Create();

			this.TrackHours_ReturnsSplitterTrack(employee1, task.Address, 5);
			this.TrackHours_ReturnsSplitterTrack(employee2, task.Address, 15);
			this.TrackHours_ReturnsSplitterTrack(employee3, task.Address, 20);

			var stageAction = new ChangeStageAction(string.Empty, task.Address, TaskStage.Finalized);
			this.permissionManager.ExecuteAction(stageAction);

			var employee1Tokens = this.tokenManager.TaggedBalanceOf(employee1).TotalBalance;
			var employee2Tokens = this.tokenManager.TaggedBalanceOf(employee2).TotalBalance;
			var employee3Tokens = this.tokenManager.TaggedBalanceOf(employee3).TotalBalance;
			Assert.Equal(20, employee1Tokens);
			Assert.Equal(60, employee2Tokens);
			Assert.Equal(80, employee3Tokens);
		}

		private decimal TrackHours_ReturnsSplitterTrack(Address employee, Address task, decimal amount)
		{
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, amount, new DateTime(2019, 5, 16),
				employee, task);
			this.permissionManager.ExecuteAction(track);
			var hours = this.splitterPerHours.GetTasksAddresToEmployeesHoursPerAddress();
			var employeeHours = hours.FirstOrDefault(x => x.Key.Equals(task)).Value
				.Where(y => y.Key.Equals(employee)).First().Value;
			return employeeHours;
		}

		private void InitializePermissions()
		{
			var perm = new Permission(typeof(TrackWorkAction));
			var addPerm = new AddPermissionAction(string.Empty, this.workTracker.Address, perm,
				this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPerm);

			perm = new Permission(typeof(MintAction));
			addPerm = new AddPermissionAction(string.Empty, this.tokenManager.Address, perm,
				this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPerm);

			perm = new Permission(typeof(TransferAction));
			addPerm = new AddPermissionAction(string.Empty, this.tokenManager.Address, perm,
				this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPerm);
		}

		private void CreateTokenInteractionPermissions(Address address)
		{
			var transferPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(TransferAction)),
				address);
			this.permissionManager.ExecuteAction(transferPermission);

			transferPermission = new AddPermissionAction(
				string.Empty,
				this.tokenManager.Address,
				new Permission(typeof(TransferAction)),
				this.splitterPerHours.Address);
			this.permissionManager.ExecuteAction(transferPermission);

			var receiveTokens = new AddPermissionAction(
				string.Empty,
				address,
				new Permission(typeof(TokensReceivedAction)),
				this.tokenManager.Address);
			this.permissionManager.ExecuteAction(receiveTokens);

			var receiveMintedTokens = new AddPermissionAction(
				string.Empty,
				address,
				new Permission(typeof(TokensMintedAction)),
				this.tokenManager.Address);
			this.permissionManager.ExecuteAction(receiveMintedTokens);

			receiveTokens = new AddPermissionAction(
				string.Empty,
				this.splitterPerHours.Address,
				new Permission(typeof(TokensReceivedAction)),
				this.tokenManager.Address);
			this.permissionManager.ExecuteAction(receiveTokens);
		}

		private Task CreateTask(Address assignee)
		{
			var address = this.addressFactory.Create();
			var issuer = this.addressFactory.Create();

			var managersToBalances = new Dictionary<Address, BigInteger>();
			var header = "First Test Task";
			var description = "The puropose of this task is to test task's fields";
			managersToBalances.Add(this.tokenManager.Address, 0);
			var task = new Task(address, this.contractRegistry, this.permissionManager.Address,
				this.splitterPerHours.Address, managersToBalances, 1, issuer,
				header, description, true, null);
			this.contractRegistry.RegisterContract(task);
			return task;
		}
	}
}