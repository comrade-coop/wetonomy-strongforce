using System.Collections.Generic;
using ContractsCore;
using WorkTrack;
using WorkTrack.TrackerGuards;

namespace WorkTrackTests
{
	public class WorkTrackerMock : WorkTracker
	{
		public WorkTrackerMock(Address address, ContractRegistry registry, Address permissionManager)
			: base(address, registry, permissionManager)
		{
		}

		public HashSet<ITrackerGuard> GetGuards()
		{
			return this.trackGuards;
		}
	}
}