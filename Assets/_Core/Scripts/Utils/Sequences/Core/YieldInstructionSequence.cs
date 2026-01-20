using System.Collections;
using BasePuzzle.Core.Scripts.Services.GameObjs;
using UnityEngine;

namespace BasePuzzle.Core.Scripts.Utils.Sequences.Core
{
	using BasePuzzle.Core.Scripts.Services.GameObjs;

	public class YieldInstructionSequence : Sequence
	{
		private bool flag;
		public YieldInstructionSequence( YieldInstruction yieldInstruction )
		{
			YieldInstruction = yieldInstruction;
		}

		private YieldInstruction YieldInstruction { get; }

		private IEnumerator Coroutine()
		{
			yield return YieldInstruction;
			flag = true;
		}

		protected override IEnumerator Enumerator()
		{
			FGameObj.Instance.StartCoroutine(Coroutine());
			while (!flag)
			{
				yield return null;
			}
		}
	}
}