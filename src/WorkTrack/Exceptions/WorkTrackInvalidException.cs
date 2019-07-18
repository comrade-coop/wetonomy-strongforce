using System;
using WorkTrack.Actions;

namespace WorkTrack
{
	public class WorkTrackInvalidException: ArgumentException
	{
		public WorkTrackInvalidException(Type guardType, TrackWorkAction action)
			: base($"{guardType.ToString()} failed to validate TrackWorkAction {{Date: {action.Date}, Hours: {action.Hours} }}.")
		{
		}
	}
}