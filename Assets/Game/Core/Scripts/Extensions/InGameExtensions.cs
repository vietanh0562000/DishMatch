namespace BasePuzzle
{
	using UnityEngine;

	public static class InGameExtensions
	{
		public static bool IsOnScreen(this Transform transform)
		{
			var main = Camera.main;
			if (main != null)
			{
				Vector3 screenPoint = main.WorldToViewportPoint(transform.position);

				bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

				return onScreen;
			}

			return false;
		}
	}
}