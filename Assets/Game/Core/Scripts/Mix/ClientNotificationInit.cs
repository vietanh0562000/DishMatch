using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class NotificationDefinition
{
	public int    Day;
	public string Title   = "Block Out: Wood Puzzle";
	public string Content = "Let's play!";
}

public class ClientNotificationInit : MonoBehaviour
{
	[SerializeField] [Range(0, 23)] private int _hour   = 19;
	[SerializeField] [Range(0, 59)] private int _minute = 45;

	[Title("Days Showing")] [ListDrawerSettings(DraggableItems = false, Expanded = true)] [SerializeField]
	NotificationDefinition[] _days;
	
	private DateTime _autoNotifyTime;
	

	private void Start()
	{
		Init();
		ScheduleNotification();
	}

	private void Init()
	{
		DateTime now = DateTime.Now;
		_autoNotifyTime = new DateTime(now.Year, now.Month, now.Day, _hour, _minute, 0);

		if (now >= _autoNotifyTime)
		{
			_autoNotifyTime = _autoNotifyTime.AddDays(1);
		}

		ClientNotification.Register();
	}

	private void ScheduleNotification()
	{
		DateTime now = DateTime.Now;
		for (int i = 0; i < _days.Length; ++i)
		{
			var d    = _days[i];
			var v    = d.Day;
			var time = now.AddDays(v).AddMinutes(5);
			var id   = $"day_{v}";

			ClientNotification.CreateNotification(NotificationCategory.NotifyDaily, id, d.Title, d.Content, time, false, true);
		}
	}
}