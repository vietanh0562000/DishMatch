using System;
using System.Collections;

namespace BasePuzzle.Core.Scripts.Utils.Sequences.Core
{
    public interface ISequence : IEnumerator
    {
        Exception Exception { get; }
        bool Failed { get; }

        bool Done { get; }
        void Cancel();
        bool TryContinue();
        IEnumerator Wait();
    }
}