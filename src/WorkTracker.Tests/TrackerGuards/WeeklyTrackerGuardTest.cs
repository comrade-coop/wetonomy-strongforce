using System;
using System.Collections.Generic;
using ContractsCore;
using WorkTracker;
using WorkTracker.Actions;
using WorkTracker.TrackerGuards;
using Xunit;

namespace WorkTrackTests
{
	public class WeeklyTrackerGuardTest
	{
		public readonly IAddressFactory addressFactory = new RandomAddressFactory();
		public readonly WorkEventLog eventLog;
		public readonly WorkTracker.WorkTracker workTracker;
		public readonly WeeklyTrackerGuard trackerGuard;
		public readonly Address address;

		public WeeklyTrackerGuardTest()
		{
			this.address = this.addressFactory.Create();
			var eventSet = new HashSet<WorkEventArgs>()
			{
				new WorkEventArgs(5, new DateTime(2019, 5, 13), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 14), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 15), this.address),
				new WorkEventArgs(5, new DateTime(2019, 5, 16), this.address),
			};
			this.workTracker = new WorkTracker.WorkTracker(null, null, null);
			this.eventLog = new WorkEventLog(30, eventSet, this.workTracker);
			this.trackerGuard = new WeeklyTrackerGuard(this.eventLog, 40);
		}

		[Fact]
		public void ValidateWorkEvent()
		{
			var trackAction = new TrackWorkAction(string.Empty, null, 8, new DateTime(2019, 5, 16), this.address);
			Assert.True(this.trackerGuard.Validate(trackAction));
		}

		[Fact]
		public void ValidateWorkEvent_WhenHoursExeeds_ReturnFalse()
		{
			var trackAction = new TrackWorkAction(string.Empty, null, 25, new DateTime(2019, 5, 16), this.address);
			Assert.False(this.trackerGuard.Validate(trackAction));
		}

		[Fact]
		public void ValidateWorkEvent_WhenNewWeek_ReturnTrue()
		{
			// week starts from Sunday
			var trackAction = new TrackWorkAction(string.Empty, null, 25, new DateTime(2019, 5, 19), this.address);
			Assert.True(this.trackerGuard.Validate(trackAction));
		}
	}
}