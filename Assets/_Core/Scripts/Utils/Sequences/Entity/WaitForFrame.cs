using System.Collections;
using BasePuzzle.Core.Scripts.Utils.Sequences.Core;

namespace BasePuzzle.Core.Scripts.Utils.Sequences.Entity
{
	using BasePuzzle.Core.Scripts.Utils.Sequences.Core;

	public class WaitForFrame : Sequence
	{
		private readonly int frames;
		public WaitForFrame( int frames )
		{
			this.frames = frames;
		}
		protected override IEnumerator Enumerator()
		{
			var count = 0;
			while( count < frames )
			{
				count++;
				yield return null;
			}
		}
	}
}