using ContractsCore;
using ContractsCore.Actions;
using WorkTrack.TrackerGuards;

namespace WorkTrack.Actions
{
	public class RemoveTrackerGuardAction : Action
	{
		public ITrackerGuard Guard { get; set; }

		public RemoveTrackerGuardAction(string hash, Address target, ITrackerGuard guard)
			: base(hash, target)
		{
			this.Guard = guard;
		}
	}
}