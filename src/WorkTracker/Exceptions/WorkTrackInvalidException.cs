using System;
using WorkTracker.Actions;

namespace WorkTracker
{
	public class WorkTrackInvalidException: ArgumentException
	{
		public WorkTrackInvalidException(Type guardType, TrackWorkAction action)
			: base($"{guardType.ToString()} failed to validate TrackWorkAction {{Date: {action.Date}, Hours: {action.Hours} }}.")
		{
		}
	}
}