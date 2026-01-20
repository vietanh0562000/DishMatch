using System.Collections;

namespace BasePuzzle.Core.Scripts.Controllers.Interfaces
{
    /// <summary>
    /// Make sure the implement class is sealed and has a no arg constructor with preserve attribute, containing basic initialization.
    /// The constructor is called in side thread.
    /// </summary>
    public interface IFInit
    {
        IEnumerator Init();
    }
}