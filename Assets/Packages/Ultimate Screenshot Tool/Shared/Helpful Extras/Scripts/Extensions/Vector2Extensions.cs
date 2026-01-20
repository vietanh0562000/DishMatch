using UnityEngine;

namespace TRS.CaptureTool.Extras
{
	public static class Vector2Extensions
	{
		public static Vector2 FloorToInt(this Vector2 vector2)
		{
			return new Vector2(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
		}

		public static Vector2 CeilToInt(this Vector2 vector2)
		{
			return new Vector2(Mathf.CeilToInt(vector2.x), Mathf.CeilToInt(vector2.y));
		}
	}
}
