using System;
using System.Collections.Generic;
using ContractsCore;
using ContractsCore.Contracts;
using WorkTrack.Actions;
using WorkTrack;
using WorkTrack.TrackerGuards;
using Action = ContractsCore.Actions.Action;

namespace WorkTrack
{
	public class WorkTracker : AclPermittedContract
	{
		protected HashSet<ITrackerGuard> trackGuards;

		public event EventHandler<WorkEventArgs> TrackedWork;

		public WorkTracker(Address address, ContractRegistry registry, Address permissionManager)
			: base(address, registry, permissionManager)
		{
			this.trackGuards = new HashSet<ITrackerGuard>();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TrackWorkAction trackAction:
					return this.HandleTrackWork(trackAction);

				case AddTrackerGuardAction addAction:
					return HandleAddTrackerGuard(addAction);

				case RemoveTrackerGuardAction removeAction:
					return HandleRemoveTrackerGuard(removeAction);

				default: return false;
			}
		}

		protected bool CheckGuards(TrackWorkAction action)
		{
			foreach (var guard in this.trackGuards)
			{
				if (!guard.Validate(action))
				{
					throw new WorkTrackInvalidException(guard.GetType(), action);
				}
			}

			return true;
		}

		protected bool HandleTrackWork(TrackWorkAction action)
		{
			if (this.CheckGuards(action))
			{
				var e = new WorkEventArgs(action.Hours, action.Date, action.Employee, action.TaskAddress);
				this.TrackedWork?.Invoke(this, e);
			}

			return true;
		}

		protected bool HandleAddTrackerGuard(AddTrackerGuardAction action)
		{
			if (!this.trackGuards.Contains(action.Guard))
			{
				return this.trackGuards.Add(action.Guard);
			}

			return false;
		}

		protected bool HandleRemoveTrackerGuard(RemoveTrackerGuardAction action)
		{
			return this.trackGuards.Remove(action.Guard);
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