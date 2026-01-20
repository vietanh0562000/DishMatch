
using UnityEngine.EventSystems;

namespace BasePuzzle
{
	using System;
	using System.Collections.Generic;
	using MoreMountains.Tools;
	using UnityEngine;

	public static class UIServices
	{
		public static readonly int MAX_CHARACTER_NAME = 15;
		public static readonly int MIN_CHARACTER_NAME = 1;
		
		public static string ConvertCurrency(this int amount, bool thousand = false)
		{
			if (amount >= 1000000000)
			{
				return (amount / 1000000000D).AsString("0.##") + "B";
			}

			if (amount >= 1000000)
			{
				return (amount / 1000000D).AsString("0.##") + "M";
			}
			
			if (amount >= 1000 && thousand)
			{
				return (amount / 1000D).AsString("0.##") + "k";
			}

			return amount.ToString();
		}

		public static string ToTextCompare(this int amount, int requestAmount)
		{
			string compareColor = requestAmount <= amount ? "#50D3F3" : "#FA534A";

			return $"<color={compareColor}>{amount}</color>/{requestAmount}";
		}

		public static bool IsPointerOverUIObject()
		{
			var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
			{
				position = Input.mousePosition
			};
			var results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

			return results.Count > 0;
		}
		
		public static string ToRoman(this int number, bool extraOne = false)
		{
			if (extraOne)
			{
				number += 1;
			}

			if ((number < 1) || (number > 3999)) return String.Empty;
			if (number >= 1000) return "M" + ToRoman(number - 1000);
			if (number >= 900) return "CM" + ToRoman(number - 900);
			if (number >= 500) return "D" + ToRoman(number - 500);
			if (number >= 400) return "CD" + ToRoman(number - 400);
			if (number >= 100) return "C" + ToRoman(number - 100);
			if (number >= 90) return "XC" + ToRoman(number - 90);
			if (number >= 50) return "L" + ToRoman(number - 50);
			if (number >= 40) return "XL" + ToRoman(number - 40);
			if (number >= 10) return "X" + ToRoman(number - 10);
			if (number >= 9) return "IX" + ToRoman(number - 9);
			if (number >= 5) return "V" + ToRoman(number - 5);
			if (number >= 4) return "IV" + ToRoman(number - 4);
			return "I" + ToRoman(number - 1);
		}

		public static string TimestampToText(this long timeLeft)
		{
			TimeSpan timeRemain = TimeSpan.FromSeconds(timeLeft);

			var timeString = timeRemain.Days > 0 ? String.Format(DateTimeUtils.LONG_TIME_FORMAT, timeRemain) 
				: timeRemain.ToString(DateTimeUtils.TIME_FORMAT);

			return timeString;
		}
		
		public static string TimestampToTextShorten(this long timeLeft)
		{
			TimeSpan timeRemain = TimeSpan.FromSeconds(timeLeft);

			string timeString;

			if (timeRemain.Days > 0)
			{
				timeString = String.Format(DateTimeUtils.LONG_TIME_FORMAT, timeRemain);
			}
			else
			{
				timeString = timeRemain.ToString(timeRemain.Hours > 0 ? DateTimeUtils.TIME_FORMAT : DateTimeUtils.TIME_IN_MINUTES_FORMAT);
			}

			return timeString;
		}
	}
}