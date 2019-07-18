using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.Actions;

namespace WorkTracker.TrackerGuards
{
	public interface ITrackerGuard : IEquatable<ITrackerGuard>
	{
		string Name { get; }

		bool Validate(TrackWorkAction action);
	}
}