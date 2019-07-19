using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using WorkTracker.Actions;
using WorkTracker.TrackerGuards;
using Action = ContractsCore.Actions.Action;

namespace WorkTracker
{
	public class WorkTracker : AclPermittedContract
	{
		public WorkTracker(Address address, ContractRegistry registry, Address permissionManager)
			: base(address, registry, permissionManager)
		{
			this.TrackGuards = new HashSet<ITrackerGuard>();
		}

		public event EventHandler<WorkEventArgs> TrackedWork;

		public ISet<ITrackerGuard> TrackGuards { get; }

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TrackWorkAction trackAction:
					return this.HandleTrackWork(trackAction);

				case AddTrackerGuardAction addAction:
					return this.HandleAddTrackerGuard(addAction);

				case RemoveTrackerGuardAction removeAction:
					return this.HandleRemoveTrackerGuard(removeAction);

				default: return false;
			}
		}

		protected virtual bool CheckGuards(TrackWorkAction action)
		{
			foreach (ITrackerGuard guard in this.TrackGuards)
			{
				if (!guard.Validate(action))
				{
					throw new WorkTrackInvalidException(guard.GetType(), action);
				}
			}

			return true;
		}

		protected virtual bool HandleTrackWork(TrackWorkAction action)
		{
			if (this.CheckGuards(action))
			{
				var e = new WorkEventArgs(action.Hours, action.Date, action.Employee, action.TaskAddress);
				this.TrackedWork?.Invoke(this, e);
			}

			return true;
		}

		protected virtual bool HandleAddTrackerGuard(AddTrackerGuardAction action)
		{
			return !this.TrackGuards.Contains(action.Guard) && this.TrackGuards.Add(action.Guard);
		}

		protected virtual bool HandleRemoveTrackerGuard(RemoveTrackerGuardAction action)
		{
			return this.TrackGuards.Remove(action.Guard);
		}

		protected override void BulletTaken(List<Stack<Address>> ways, Action targetAction)
		{
			throw new NotImplementedException();
		}

		protected override object GetState()
		{
			throw new NotImplementedException();
		}
	}
}