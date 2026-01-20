using System;
using UnityEngine;
using System;
using System.Globalization;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
    using Unity.Notifications.iOS;
#endif
using UnityEngine;

public enum NotificationCategory
{
	PushAuto,
	NotifyDaily,
}

public static class ClientNotification
{
	private static string ChanelID  = "Chanel_BlockJam";
	private static string SmallIcon = "small_icon";
	private static string LargeIcon = "large_icon";

	private static string Prefix = "ClientNotify_";

	public static bool IsRegistered { get; private set; }

	/// <summary>
	/// Must be called in the very first moment
	/// </summary>
	public static void Register()
	{
#if UNITY_ANDROID
		RegisterAndroid();
#endif

#if UNITY_IOS
            RegisterIOS();
#endif

		IsRegistered = true;
	}

#if UNITY_ANDROID
	private static void RegisterAndroid()
	{
		var androidChanel = new AndroidNotificationChannel()
		{
			Id          = ChanelID,
			Name        = "Block Jam Chanel",
			Description = "Playful Time",
			Importance  = Importance.Default
		};

		AndroidNotificationCenter.RegisterNotificationChannel(androidChanel);
	}
#endif

#if UNITY_IOS
        private static void RegisterIOS()
        {
            
        }
#endif

	/// <summary>
	/// Register opened callback each time you need to know
	/// </summary>
	public static void RegisterOpenedByNotificationCallback(Action<string> onOpenedByNotification)
	{
		if (!IsRegistered) return;

#if UNITY_ANDROID
		var last = AndroidNotificationCenter.GetLastNotificationIntent();
		if (last != null)
		{
			Debug.Log("Game was open with notification!!!!!!");
			onOpenedByNotification?.Invoke(last.Id.ToString(CultureInfo.InvariantCulture));
		}
#endif

#if UNITY_IOS
            var last = iOSNotificationCenter.GetLastRespondedNotification();
            if (last != null)
            {
                Debug.Log("Game was open with notification!!!!!!");
                onOpenedByNotification?.Invoke(last.Identifier);
            }
#endif
	}

	/// <summary>
	/// Create a notification using UTC-timestamp
	/// </summary>
	public static void CreateNotification(NotificationCategory cat, string id, string title, string content, long utcTimestamp, bool removePrevious = true)
	{
		var targetTime = DateTimeUtils.GetLocalDateTimeFromTimestamp(utcTimestamp);
		CreateNotification(cat, id, title, content, targetTime, removePrevious);
	}

	/// <summary>
	/// Create a notification using Local DateTime.
	/// </summary>
	public static void CreateNotification(NotificationCategory cat, string id, string title, string content, DateTime localTargetTime, bool overridePrevious = true, bool once = false)
	{
		if (!IsRegistered) return;

#if UNITY_ANDROID
		CreateAndroidNotification(cat, id, title, content, localTargetTime, overridePrevious, once);
#endif

#if UNITY_IOS
        CreateIOSNotification(cat, id, title, content, localTargetTime, overridePrevious, once);
#endif

#if UNITY_EDITOR
		Debug.Log($"Create Notification ID: {id}, Title: {title}, Content: {content} in {localTargetTime:F}");
#endif
	}

#if UNITY_ANDROID
	private static void CreateAndroidNotification(NotificationCategory cat, string id, string title, string content, DateTime targetTime, bool overridePrevious, bool once)
	{
		if (string.IsNullOrEmpty(content)) return;

		var key = GetKey(cat, id);
		if (overridePrevious)
		{
			CancelScheduledNotification(key);
		}
		else if (once && SaveLoadHandler.Exist(key))
		{
			Debug.Log("Notification created. Abort");
			return;
		}
		
		AndroidNotification notification = new AndroidNotification()
		{
			Title            = title,
			Text             = content,
			SmallIcon        = SmallIcon,
			LargeIcon        = LargeIcon,
			FireTime         = targetTime,
			ShowTimestamp    = true,
			ShouldAutoCancel = true,
			Group            = "Wood Block Jam Notifications",
			GroupSummary     = true,
		};

		int identifier = AndroidNotificationCenter.SendNotification(notification, ChanelID);
		SaveLoadHandler.Save(key, identifier.ToString(CultureInfo.InvariantCulture));
	}
#endif

	private static void CancelScheduledNotification(string key)
	{
		if (SaveLoadHandler.Exist(key))
		{
			string identifier = SaveLoadHandler.Load<string>(key);

#if UNITY_ANDROID
			if (int.TryParse(identifier, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
			{
				AndroidNotificationCenter.CancelScheduledNotification(value);
			}
#endif

#if UNITY_IOS
                iOSNotificationCenter.RemoveScheduledNotification(identifier);
#endif
		}
	}

#if UNITY_IOS
        private static void CreateIOSNotification(NotificationCategory cat, string id, string title, string content, DateTime targetTime, bool overridePrevious, bool once)
        {
            if (string.IsNullOrEmpty(content)) return;
            
            DateTime now = DateTime.Now;
            var      interval = targetTime - now;
            if (interval.Seconds <= 0) return;
            
            var key = GetKey(cat, id);
            if (overridePrevious)
            {
                CancelScheduledNotification(key);
            }
            else if (once && SaveLoadHandler.Exist(key))
            {
	            return;
            }
            
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
	            TimeInterval = interval,
	            Repeats      = false
            };

            var notification = new iOSNotification()
            {
                // You can specify a custom identifier which can be used to manage the notification later.
                // If you don't provide one, a unique string will be generated automatically.
                Identifier = id,
                Title = title,
                Body = content,
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                CategoryIdentifier = "puzzle",
                ThreadIdentifier = "thread",
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
            SaveLoadHandler.Save(key, notification.Identifier);
        }
#endif

	private static string GetKey(NotificationCategory cat, string id)
	{
		string key = $"{Prefix}{cat}_{id}";
		return key;
	}
	
	public static void DeleteScheduledNotification(NotificationCategory cat, string id)
	{
		string key = GetKey(cat, id);
		CancelScheduledNotification(key);
		SaveLoadHandler.DeleteKey(key);
	}
}