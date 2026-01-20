using System;
using UnityEngine;

public static class AntiTimeCheatUtils
{
	internal static Action<DateTime> onTimeServerUpdated;
	
	internal static DateTime GetDateTimeUTC()
	{
		return DateTime.UtcNow;
	}

	internal static void ListenToUpdateTimeFromServer()
	{
		//FNetManager.Instance.OnSessionStarted(UpdateFollowTimeServer);
	}
		
	private static void UpdateFollowTimeServer()
	{
		// new FTimePing().AddSCListener<FTimePong>((message,timeout,success) =>
		// {
		// 	if(success)
		// 	{
		// 		var            timeServer=message.timeServer;
		// 		DateTimeOffset date      =DateTimeOffset.FromUnixTimeMilliseconds(timeServer);
		// 		var            utcNow    =date.UtcDateTime;
		// 		onTimeServerUpdated?.Invoke(utcNow);
		// 	}
		// }).Send();
	}
}