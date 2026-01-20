using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BasePuzzle.Core.Scripts.Exceptions;
using BasePuzzle.Core.Scripts.Logs;
using BasePuzzle.Core.Scripts.Services.GameObjs;
using UnityEngine;

namespace BasePuzzle.Core.Scripts.Utils.Generics
{
    using BasePuzzle.Core.Scripts.Exceptions;
    using BasePuzzle.Core.Scripts.Logs;
    using BasePuzzle.Core.Scripts.Services.GameObjs;

    public static class FGenerics
    {
        public static List<T> GetInstances<T>()
        {
            var types = from t in AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                where t.GetInterfaces().Contains(typeof(T))
                      && t.GetConstructor(Type.EmptyTypes) != null
                select t;

            List<T> result = new List<T>();
            foreach (var type in types)
            {
                try
                {
                    result.Add((T)GetInstance(type));
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Error(e);
                }
            }

            return result;
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        private static object GetInstance(Type type)
        {
            object instance;
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                instance = FGameObj.Instance.AddIfNotExist(type);
            }

            else
            {
                instance = Activator.CreateInstance(type);
            }

            if (instance != null) return instance;
            throw new FSdkException("Failed to created instance of type: " + type);
        }
    }
}