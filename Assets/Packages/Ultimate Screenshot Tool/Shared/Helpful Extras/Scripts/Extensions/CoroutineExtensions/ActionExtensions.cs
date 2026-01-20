using UnityEngine;

namespace TRS.CaptureTool.Extras
{
	public static class ActionExtensions
	{
		public static void PerformAfterCoroutine<T>(this System.Action action)
			where T : YieldInstruction, new()
		{
			CoroutineBehaviour.StaticStartCoroutineAfterYield<T>(action);
		}
	}
}