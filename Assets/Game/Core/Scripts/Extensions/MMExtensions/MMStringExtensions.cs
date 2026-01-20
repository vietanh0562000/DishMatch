using System.Globalization;

namespace MoreMountains.Tools
{
	using UnityEngine;

	/// <summary>
	/// String Extensions
	/// </summary>
	public static class MMStringExtensions
	{
		public static string ChangeColor(this string text, string hexColor) { return $"<color=#{hexColor}>{text}</color>"; }
		public static string ChangeColor(this float number, string hexColor) { return number.AsString().ChangeColor(hexColor); }
		public static string AsString(this float number, string format = "F1")  { return number.ToString(format, CultureInfo.InvariantCulture); }
		public static string AsString(this double number, string format = "F1") { return number.ToString(format, CultureInfo.InvariantCulture); }

		public static string Pad0Left(this int number, int count = 2)
		{
			return number.ToString().PadLeft(count, '0');
		}
		
		public static string AsAbsString(this float number, string format = "F1") { return Mathf.Abs(number).AsString(format); }
		public static string SetBold(this string text)
		{
			return $"<b>{text}</b>";
		} 
	}
}