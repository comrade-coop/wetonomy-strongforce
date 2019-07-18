using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Permissions;
using WorkTrack;
using WorkTrack.Actions;
using WorkTrack.TrackerGuards;
using Xunit;

namespace WorkTrackTests
{
	public class WorkTrackerTest
	{
		private readonly IAddressFactory addressFactory = new RandomAddressFactory();
		private readonly ContractRegistry contractRegistry;
		private readonly ContractExecutor permissionManager;
		private readonly Address address;
		private WorkTrackerMock workTracker;
		private WorkEventLog eventLog;

		public WorkTrackerTest()
		{
			this.address = this.addressFactory.Create();
			this.permissionManager = new ContractExecutor(this.addressFactory.Create());
			this.contractRegistry = new ContractRegistry();
			this.workTracker = new WorkTrackerMock(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);

			this.contractRegistry.RegisterContract(this.permissionManager);
			this.contractRegistry.RegisterContract(this.workTracker);

			this.ConfigurePermissions();
		}

		[Fact]
		public void TrackWorkAction()
		{
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, 5, DateTime.Today, this.address);
			decimal hours = 0;
			DateTime date = DateTime.MaxValue;
			this.workTracker.TrackedWork += (_, args) =>
			{
				hours = args.Hours;
				date = args.Date;
			};

			this.permissionManager.ExecuteAction(track);

			Assert.Equal(5, hours);
			Assert.Equal(DateTime.Today, date);
		}

		[Fact]
		public void TrackWorkAction_MultipleTimes_AddsAllTrakedWork()
		{
			var trackDate = DateTime.Now;
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, 5, trackDate, this.address);
			decimal hours = 0;
			DateTime date = DateTime.MaxValue;
			this.workTracker.TrackedWork += (_, args) =>
			{
				hours += args.Hours;
				date = args.Date;
			};

			this.permissionManager.ExecuteAction(track);

			Assert.Equal(5, hours);
			Assert.Equal(trackDate, date);

			trackDate = DateTime.Now;
			track = new TrackWorkAction(string.Empty, this.workTracker.Address, 5, trackDate, this.address);

			this.permissionManager.ExecuteAction(track);

			Assert.Equal(10, hours);
			Assert.Equal(trackDate, date);

			trackDate = DateTime.Now;
			track = new TrackWorkAction(string.Empty, this.workTracker.Address, 5, trackDate, this.address);

			this.permissionManager.ExecuteAction(track);

			Assert.Equal(15, hours);
			Assert.Equal(trackDate, date);
		}

		[Fact]
		public void AddTrackerGuard()
		{
			var guard = new FutureDateTrackerGuard();
			var addGuard = new AddTrackerGuardAction(string.Empty, this.workTracker.Address, guard);
			this.permissionManager.ExecuteAction(addGuard);

			Assert.Contains(guard, this.workTracker.GetGuards());
		}

		[Fact]
		public void AddTrackerGuard_WhenDuplicate_RemainsSingle()
		{
			this.InitializeEventLogAndGuards();
			var guard = new FutureDateTrackerGuard();
			var addGuard = new AddTrackerGuardAction(string.Empty, this.workTracker.Address, guard);
			this.permissionManager.ExecuteAction(addGuard);

			Assert.Equal(3, this.workTracker.GetGuards().Count);
		}

		[Fact]
		public void RemoveTrackerGuard()
		{
			var guard = new FutureDateTrackerGuard();
			var addGuard = new AddTrackerGuardAction(string.Empty, this.workTracker.Address, guard);
			this.permissionManager.ExecuteAction(addGuard);
			Assert.Contains(guard, this.workTracker.GetGuards());

			var removeGuard = new RemoveTrackerGuardAction(string.Empty, this.workTracker.Address, guard);
			this.permissionManager.ExecuteAction(removeGuard);
			Assert.DoesNotContain(guard, this.workTracker.GetGuards());
		}

		[Fact]
		public void TrackWork_WhenGuarded_AddTrackedWork()
		{
			this.InitializeEventLogAndGuards();
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, 5, new DateTime(2019, 5, 16), this.address);
			this.permissionManager.ExecuteAction(track);
			Assert.Contains(new WorkEventArgs(5, new DateTime(2019, 5, 16), this.address), this.eventLog.EventsHistory);
		}

		[Fact]
		public void TrackWork_WhenGuarded_WhenHoursExeeds_ThrowsExeption()
		{
			this.InitializeEventLogAndGuards();
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, 25, new DateTime(2019, 5, 16), this.address);
			Assert.Throws<WorkTrackInvalidException>(() => this.permissionManager.ExecuteAction(track));
		}

		[Fact]
		public void TrackWork_WhenGuarded_WhenFutureDate_ThrowsExeption()
		{
			this.InitializeEventLogAndGuards();
			var track = new TrackWorkAction(string.Empty, this.workTracker.Address, 25, DateTime.Today.AddDays(1), this.address);
			Assert.Throws<WorkTrackInvalidException>(() => this.permissionManager.ExecuteAction(track));
		}

		private void ConfigurePermissions()
		{
			var perm = new Permission(typeof(TrackWorkAction));
			var addPerm = new AddPermissionAction(string.Empty, this.workTracker.Address, perm, this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPerm);

			perm = new Permission(typeof(AddTrackerGuardAction));
			addPerm = new AddPermissionAction(string.Empty, this.workTracker.Address, perm, this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPerm);

			perm = new Permission(typeof(RemoveTrackerGuardAction));
			addPerm = new AddPermissionAction(string.Empty, this.workTracker.Address, perm, this.permissionManager.Address);
			this.permissionManager.ExecuteAction(addPerm);
		}

		private void InitializeEventLogAndGuards()
		{
			var eventSet = new HashSet<WorkEventArgs>()
			{
				new WorkEventArgs(5, new DateTime(2019, 5, 13), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 14), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 15), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 16), this.address),
			};

			this.workTracker = new WorkTrackerMock(this.addressFactory.Create(), this.contractRegistry, this.permissionManager.Address);
			this.contractRegistry.RegisterContract(this.workTracker);
			this.ConfigurePermissions();

			this.eventLog = new WorkEventLog(30, eventSet, this.workTracker);
			var addGuard = new AddTrackerGuardAction(string.Empty, this.workTracker.Address, new WeeklyTrackerGuard(this.eventLog));
			this.permissionManager.ExecuteAction(addGuard);
			addGuard = new AddTrackerGuardAction(string.Empty, this.workTracker.Address, new MonthlyTrackerGuard(this.eventLog));
			this.permissionManager.ExecuteAction(addGuard);
			addGuard = new AddTrackerGuardAction(string.Empty, this.workTracker.Address, new FutureDateTrackerGuard());
			this.permissionManager.ExecuteAction(addGuard);
		}
	}
}