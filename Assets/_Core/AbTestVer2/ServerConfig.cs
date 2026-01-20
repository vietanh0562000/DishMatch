using System;
using System.Collections;
using System.Globalization;
using System.Text;
using BasePuzzle.Core.AbTestVer2.Payloads;
using BasePuzzle.Core.AbTestVer2.Repositories;
using BasePuzzle.Core.Scripts.Controllers.Interfaces;
using BasePuzzle.Core.Scripts.Logs;
using BasePuzzle.Core.Scripts.Services.MainThreads;
using BasePuzzle.Core.Scripts.Utils;
using BasePuzzle.Core.Scripts.Utils.Entities;
using BasePuzzle.Core.Scripts.Utils.FActions.Variances.Starts;
using UnityEngine;
using UnityEngine.Scripting;

namespace BasePuzzle.Core.Scripts.ABTesting.Scripts.Model
{
    using BasePuzzle.Core.AbTestVer2.Payloads;
    using BasePuzzle.Core.AbTestVer2.Repositories;
    using BasePuzzle.Core.Scripts.Controllers.Interfaces;
    using BasePuzzle.Core.Scripts.Logs;
    using BasePuzzle.Core.Scripts.Services.MainThreads;
    using BasePuzzle.Core.Scripts.Utils;
    using BasePuzzle.Core.Scripts.Utils.Entities;
    using BasePuzzle.Core.Scripts.Utils.FActions.Variances.Starts;

    public abstract class ServerConfig
    {
        private static readonly FConcurrentDict<Type, ServerConfig> Cache = new FConcurrentDict<Type, ServerConfig>();

        private static readonly object Locker = new object();

        private static string _abTestString;
        public static ExecState UpdateFromNet { get; private set; } = ExecState.NotStarted;
        public static string RunningAbTesting => FConfigRepo.RunningAbTesting;

        public static string AbTestingString
        {
            get
            {
                if (_abTestString == null)
                    lock (Locker)
                    {
                        var builder = new StringBuilder();
                        foreach (var configObj in FConfigRepo.TestingConfigs)
                            builder.Append(Convert.ToString(configObj.Key, CultureInfo.InvariantCulture))
                                .Append(":")
                                .Append(Convert.ToString(configObj.Value, CultureInfo.InvariantCulture))
                                .Append("_");
                        if (builder.Length > 0) builder.Length--;

                        _abTestString = builder.ToString();
                    }

                return _abTestString;
            }
        }

        public static event EventHandler OnUpdateFromNet;

        public static T Instance<T>() where T : ServerConfig, new()
        {
            return (T)Cache.Compute(typeof(T), (hasKey, config) =>
            {
                if (hasKey) return config;
                return CreateInstance<T>();
            });
        }

        private static T CreateInstance<T>() where T : ServerConfig, new()
        {
            try
            {
                return JsonUtil.FromJson<T>(JsonUtil.ToJson(FConfigRepo.Configs));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new T();
            }
        }
    }
}