using System;
using System.Collections.Generic;
using System.Text;
using WorkTrack.Actions;

namespace WorkTrack.Exceptions
{
	public class WorkTrackInvalidException: Exception
	{
		public WorkTrackInvalidException(Type guardType, TrackWorkAction action)
			: base($"{guardType.ToString()} failed to validate TrackWorkAction {{Date: {action.Date}, Hours: {action.Hours} }}.")
		{
		}
	}
}