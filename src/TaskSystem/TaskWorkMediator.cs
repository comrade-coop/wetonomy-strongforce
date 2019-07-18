using ContractsCore;
using WorkTracker;

namespace TaskSystem
{
	public class TaskWorkMediator
	{
		private readonly TrackWorkHours trackCallback;

		public TaskWorkMediator(WorkTracker.WorkTracker workTracker, TrackWorkHours callback)
		{
			workTracker.TrackedWork += (_, actionArgs) => this.HandleTrackedWork(actionArgs);
			this.trackCallback = callback;
		}

		public delegate bool TrackWorkHours(Address employeeAddress, decimal amount, Address taskAddress);

		private void HandleTrackedWork(WorkEventArgs eventArgs)
		{
			this.trackCallback(eventArgs.Employee, eventArgs.Hours, eventArgs.TaskAddress);
		}
	}
}