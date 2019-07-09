using ContractsCore;
using ContractsCore.Actions;
using WorkTrack.TrackerGuards;

namespace WorkTrack.Actions
{
	public class AddTrackerGuardAction : Action
	{
		public ITrackerGuard Guard { get; set; }

		public AddTrackerGuardAction(string hash, Address target, ITrackerGuard guard)
			: base(hash, target)
		{
			this.Guard = guard;
		}
	}
}