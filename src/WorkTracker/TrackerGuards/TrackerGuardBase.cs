using System;
using WorkTracker.Actions;
using WorkTracker.TrackerGuards;

namespace WorkTracker
{
	public abstract class TrackerGuardBase: ITrackerGuard
	{
		public string Name { get; private set; }

		public WorkEventLog EventLog { get; set; }

		public TrackerGuardBase(string name, WorkEventLog eventLog)
		{
			this.Name = name;
			this.EventLog = eventLog ?? throw new NullReferenceException();
		}

		public virtual int CompareTo(ITrackerGuard other)
		{
			return this.Name.CompareTo(other.Name);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public abstract bool Validate(TrackWorkAction action);

		public bool Equals(ITrackerGuard other)
		{
			return this.CompareTo(other) == 0;
		}
	}
}