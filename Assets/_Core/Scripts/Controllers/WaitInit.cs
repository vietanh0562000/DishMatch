using System;
using System.Threading;
using BasePuzzle.Core.Scripts.Utils.Entities;
using BasePuzzle.Core.Scripts.Utils.FActions.Base;
using BasePuzzle.Core.Scripts.Utils.FActions.Variances.Starts;

namespace BasePuzzle.Core.Scripts.Controllers
{
    using BasePuzzle.Core.Scripts.Utils.Entities;
    using BasePuzzle.Core.Scripts.Utils.FActions.Base;
    using BasePuzzle.Core.Scripts.Utils.FActions.Variances.Starts;

    public class WaitInit : ChainAction
    {
        public WaitInit(IContinuableAction action) : base(action)
        {
        }
        
        public WaitInit(Action action) : this(new UnitAction(action))
        {
        }

        public override bool CanInvoke()
        {
            if (GameMain.InitState != ExecState.Succeed) return false;
            return base.CanInvoke();
        }
    }

    public class WaitInit<T> : WaitInit, IChainAction<T>, IStartAction<T>
    {
        public WaitInit(IContinuableAction<T> action) : base(action)
        {
        }

        public WaitInit(Func<T> action) : this(new UnitAction<T>(action))
        {
        }

        public bool TryInvoke(out T result)
        {
            if (CanInvoke())
            {
                Invoke();
                result = Result;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        public override bool CanInvoke()
        {
            if (GameMain.InitState != ExecState.Succeed) return false;
            return base.CanInvoke();
        }

        public T Result => ((IContinuableAction<T>)BaseAction).Result;
        
        public T InvokeAndGet()
        {
            while(!CanInvoke()) Thread.Yield();
            Invoke();
            return Result;
        }
    }
}