using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimerCountdownTMP : MonoBehaviour
{
    private TextMeshProUGUI timeText;

    public Action onStartTimer;
    public Action onStopTimer;

    private bool timerIsRunning;
    private double timeRemaining;
    private long timeRemainingLong = -1;
    private string formatTimer;

    private float _currentTimeInterval = 1;
    private DateTime _targetTime;

    private void Awake()
    {
        GetTimeTextTMP();
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            _currentTimeInterval -= Time.unscaledDeltaTime;
            if (_currentTimeInterval > 0)
            {
                return;
            }

            _currentTimeInterval = 1;

            UpdateTime();
        }
    }

    public TextMeshProUGUI GetTimeTextTMP() 
    {
        if (timeText == null)
        {
            timeText = GetComponent<TextMeshProUGUI>();
            timeText.SetText("--");
        }

        return timeText;
    }

    public string GetFormatTimeDefault()
    {
        return "{0}{1}{2}{3}";
    }

    public void StartTimer(TimeSpan timeSpan, string format = "{0}{1}{2}{3}")
    {
        var timeRemain = timeSpan.TotalSeconds;

        timerIsRunning = true;
        _targetTime = DateTimeUtils.UtcNow.AddSeconds(timeRemain);
        timeRemaining = timeRemain;
        timeRemainingLong = (long)timeRemain;
        formatTimer = format;
        onStartTimer?.Invoke();

        //First Update
        GetTimeTextTMP();
        DisplayTime(timeRemaining);
    }

    public void Stop()
    {
        timerIsRunning = false;
        onStopTimer?.Invoke();
    }

    private void UpdateTime()
    {
        timeRemaining = (_targetTime - DateTimeUtils.UtcNow).TotalSeconds;
        if (timeRemaining > 0)
        {
            if (timeRemainingLong != (long)timeRemaining)
            {
                DisplayTime(timeRemaining);
                timeRemainingLong = (long)timeRemaining;
            }
        }
        else
        {
            timeRemaining = 0;
            DisplayTime(timeRemaining);
            Stop();
        }
    }

    private void DisplayTime(double timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        var seconds = (int)(timeToDisplay % 60);
        var minutes = (int)(timeToDisplay / 60) % 60;
        var hours = (int)(timeToDisplay / 3600) % 24;
        var days = (int)(timeToDisplay / (3600 * 24));

        if (timeText != null)
        {
            string GetFormatTime()
            {
                if (days > 0)
                {
                    return string.Format(formatTimer, $"{days}d ", $"{hours}h", string.Empty, string.Empty);
                }

                if (hours > 0)
                {
                    return string.Format(formatTimer, string.Empty, $"{hours}h ", $"{minutes}m", string.Empty);
                }

                return string.Format(formatTimer, string.Empty, string.Empty, $"{minutes:00}:", $"{seconds:00}");
            }
            timeText.SetText(GetFormatTime());
        }
    }
}
