using System.Collections.Generic;
using ContractsCore;
using WorkTracker;
using WorkTracker.TrackerGuards;

namespace WorkTrackTests
{
	public class WorkTrackerMock : WorkTracker.WorkTracker
	{
		public WorkTrackerMock(Address address, ContractRegistry registry, Address permissionManager)
			: base(address, registry, permissionManager)
		{
		}

		public HashSet<ITrackerGuard> GetGuards()
		{
			return this.TrackGuards;
		}
	}
}