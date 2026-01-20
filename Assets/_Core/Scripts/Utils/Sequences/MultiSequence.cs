using System.Collections;
using BasePuzzle.Core.Scripts.Utils.Sequences.Core;

namespace BasePuzzle.Core.Scripts.Utils.Sequences
{
	using BasePuzzle.Core.Scripts.Utils.Sequences.Core;

	public class MultiSequence : Sequence
	{
		private readonly Sequence[] sequences;

		public MultiSequence( params Sequence[] sequences )
		{
			this.sequences = sequences;
		}

		protected override void OnException(System.Exception e)
		{
			foreach (Sequence sequence in sequences)
			{
				sequence.Cancel();
			}
		}

		protected override IEnumerator Enumerator()
		{
			while( true )
			{
				var doneSomething = false;
				foreach (Sequence sequence in sequences)
				{
					if( sequence.Done )
					{
						continue;
					}
					sequence.MoveNext();
					doneSomething = true;
					if( sequence.Failed )
					{
						throw sequence.Exception;
					}
				}

				if( !doneSomething )
				{
					yield break;
				}
				yield return null;
			}
		}

		public IEnumerator WaitOne()
		{
			while( true )
			{
				foreach (Sequence sequence in sequences)
				{
					if( sequence.Done )
					{
						foreach (Sequence sequence1 in sequences)
						{
							if( !sequence1.Done )
							{
								sequence1.Cancel();
							}
						}
						yield break;
					}
				}

				MoveNext();
				yield return null;
			}
		}
	}
}