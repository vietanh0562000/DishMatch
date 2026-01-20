using UnityEngine;

namespace PuzzleGames
{
	using System;
	using System.Collections.Generic;

	public class LayoutByQuantity : MonoBehaviour
	{
		[SerializeField] private List<LayoutPreserve> _layoutPreserves;

		public void Layouts(List<RectTransform> rectTransforms)
		{
			foreach (var layoutPreserve in _layoutPreserves)
			{
				if (layoutPreserve.Count != rectTransforms.Count) continue;
				for (var i = 0; i < rectTransforms.Count; i++)
				{
					rectTransforms[i].anchoredPosition = layoutPreserve.Transforms[i].anchoredPosition;
				}
			}
		}
	}

	[Serializable]
	public struct LayoutPreserve
	{
		public int             Count;
		public RectTransform[] Transforms;
	}
}