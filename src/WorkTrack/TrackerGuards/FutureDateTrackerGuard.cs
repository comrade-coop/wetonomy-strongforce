using System;
using System.Collections.Generic;
using System.Text;
using WorkTrack.Actions;

namespace WorkTrack.TrackerGuards
{
	public class FutureDateTrackerGuard : ITrackerGuard
	{
		public string Name { get; private set; }

		public FutureDateTrackerGuard()
		{
			this.Name = "FutureDateTrackerGuard";
		}

		public virtual int CompareTo(ITrackerGuard other)
		{
			return this.Name.CompareTo(other.Name);
		}

		public bool Equals(ITrackerGuard other)
		{
			return this.CompareTo(other) == 0;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public bool Validate(TrackWorkAction action)
		{
			return action.Date < DateTime.Now;
		}
	}
}