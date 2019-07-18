using System;
using ContractsCore;
using WorkTracker.Actions;
using WorkTracker.TrackerGuards;
using Xunit;

namespace WorkTrackTests
{
	public class FutureDateTrackerGuardTest
	{
		public readonly IAddressFactory addressFactory = new RandomAddressFactory();
		public readonly FutureDateTrackerGuard trackerGuard;
		public readonly Address address;

		public FutureDateTrackerGuardTest()
		{
			this.address = this.addressFactory.Create();
			this.trackerGuard = new FutureDateTrackerGuard();
		}

		[Fact]
		public void ValidateWorkEvent()
		{
			var trackAction = new TrackWorkAction(string.Empty, null, 25, DateTime.Now, this.address);
			Assert.True(this.trackerGuard.Validate(trackAction));
		}

		[Fact]
		public void ValidateWorkEvent_WhenInvalidDate_ReturnFalse()
		{
			var trackAction = new TrackWorkAction(string.Empty, null, 25, DateTime.Now.AddDays(1), this.address);
			Assert.False(this.trackerGuard.Validate(trackAction));
		}
	}
}