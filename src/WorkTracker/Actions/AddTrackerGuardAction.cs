using ContractsCore;
using ContractsCore.Actions;
using WorkTracker.TrackerGuards;

namespace WorkTracker.Actions
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