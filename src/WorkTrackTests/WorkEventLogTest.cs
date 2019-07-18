using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using WorkTrack;
using WorkTrack.Actions;
using Xunit;

namespace WorkTrackTests
{
	public class WorkEventLogTest
	{
		public readonly IAddressFactory addressFactory = new RandomAddressFactory();
		public readonly WorkEventLog eventLog;
		public readonly WorkTracker workTracker;
		public readonly ContractRegistry contractRegistry;
		public readonly ContractExecutor permissionManager;
		public readonly Address address;

		public WorkEventLogTest()
		{
			this.address = this.addressFactory.Create();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.contractRegistry = new ContractRegistry();
			this.workTracker = new WorkTracker(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			var perm = new Permission(typeof(TrackWorkAction));
			var addPerm = new AddPermissionAction(string.Empty, this.workTracker.Address, perm, this.permissionManager.Address);

			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.workTracker);

			this.permissionManager.ExecuteAction(addPerm);

			var eventSet = new HashSet<WorkEventArgs>()
			{
				new WorkEventArgs(5, new DateTime(2019, 5, 13), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 14), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 15), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 16), this.address),
			};

			this.eventLog = new WorkEventLog(30, eventSet, this.workTracker);
		}

		[Fact]
		public void WorkEventLog_ProperInitialized()
		{
			Assert.Equal(30, this.eventLog.HistoryClearPeriodInDays);
			Assert.Equal(4, this.eventLog.EventsHistory.Count);
		}

		[Fact]
		public void RegisterEvent()
		{
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, 5, new DateTime(2019, 5, 16), this.address);
			this.permissionManager.ExecuteAction(track);
			Assert.Equal(5, this.eventLog.EventsHistory.Count);
			Assert.Contains(new WorkEventArgs(5, new DateTime(2019, 5, 16), this.address), this.eventLog.EventsHistory);
		}

		[Fact]
		public void RegisterEvent_ShouldClearHistory()
		{
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, 5, DateTime.Today.AddDays(35), this.address);
			this.permissionManager.ExecuteAction(track);
			Assert.Single(this.eventLog.EventsHistory);
			Assert.Contains(new WorkEventArgs(5, DateTime.Today.AddDays(35), this.address), this.eventLog.EventsHistory);
		}
	}
}