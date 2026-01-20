using System.Collections;
using BasePuzzle.Core.Scripts.Utils.Sequences.Core;

namespace BasePuzzle.Core.Scripts.Utils.Sequences
{
	using BasePuzzle.Core.Scripts.Utils.Sequences.Core;

	public class ChainSequence : Sequence
	{
		private readonly Sequence[] sequences;
		private Sequence runningSequence;

		public ChainSequence( params Sequence[] sequences )
		{
			this.sequences = sequences;
		}

		protected override void OnException(System.Exception e)
		{
			runningSequence?.Cancel();
		}

		protected override IEnumerator Enumerator()
		{
			foreach (Sequence sequence in sequences)
			{
				yield return runningSequence = sequence;
				if (runningSequence.Failed)
				{
					throw runningSequence.Exception;
				}
			}
		}
	}
}