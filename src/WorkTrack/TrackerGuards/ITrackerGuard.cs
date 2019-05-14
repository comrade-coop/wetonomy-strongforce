using System;
using System.Collections.Generic;
using System.Text;
using WorkTrack.Actions;

namespace WorkTrack.TrackerGuards
{
	public interface ITrackerGuard : IEquatable<ITrackerGuard>
	{
		string Name { get; }

		bool Validate(TrackWorkAction action);
	}
}