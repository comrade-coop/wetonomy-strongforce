using ContractsCore;
using System;
using System.Collections.Generic;
using System.Text;
using WorkTrack;
using WorkTrack.Actions;
using WorkTrack.TrackerGuards;
using WorkTrack.WorkEventsArgs;
using Xunit;

namespace WorkTrackTests
{
	public class MonthlyTrackerGuardTest
	{
		public readonly IAddressFactory addressFactory = new RandomAddressFactory();
		public readonly WorkEventLog eventLog;
		public readonly WorkTracker workTracker;
		public readonly MonthlyTrackerGuard trackerGuard;
		public readonly Address address;

		public MonthlyTrackerGuardTest()
		{
			this.address = this.addressFactory.Create();
			var eventSet = new HashSet<WorkEventArgs>()
			{
				new WorkEventArgs(40, new DateTime(2019, 5, 13), this.address),
				new WorkEventArgs(40, new DateTime(2019, 5, 14), this.address),
				new WorkEventArgs(40, new DateTime(2019, 5, 15), this.address),
				new WorkEventArgs(40, new DateTime(2019, 5, 16), this.address),
			};
			this.workTracker = new WorkTracker(null, null, null);
			this.eventLog = new WorkEventLog(30, eventSet, this.workTracker);
			this.trackerGuard = new MonthlyTrackerGuard(this.eventLog, 176);
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
			var trackAction = new TrackWorkAction(string.Empty, null, 25, new DateTime(2019, 6, 19), this.address);
			Assert.True(this.trackerGuard.Validate(trackAction));
		}
	}
}