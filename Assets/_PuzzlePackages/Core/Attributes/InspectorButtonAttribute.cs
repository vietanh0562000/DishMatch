using System;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly string methodName;
        public readonly int marginTop, marginBot;

        public InspectorButtonAttribute(string methodName, int order = 0, int marginTop = 0, int marginBot = 0)
        {
            this.methodName = methodName;
            this.order = order;
            this.marginTop = marginTop;
            this.marginBot = marginBot;
        }
    }
}