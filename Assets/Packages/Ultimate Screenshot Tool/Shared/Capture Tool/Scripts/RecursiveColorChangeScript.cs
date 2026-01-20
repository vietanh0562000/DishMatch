using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
	[ExecuteInEditMode]
	public class RecursiveColorChangeScript : MonoBehaviour
	{
		public Color sharedColor;

		Graphic[] graphics;

		void Awake()
		{
			graphics = GetComponentsInChildren<Graphic>();
		}

		void OnValidate()
		{
			if (graphics == null)
				return;

			foreach (Graphic graphic in graphics)
				graphic.color = sharedColor;
		}
	}
}