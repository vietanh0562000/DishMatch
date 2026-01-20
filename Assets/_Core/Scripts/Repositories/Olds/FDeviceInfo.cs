using System;
using BasePuzzle.Core.Scripts.Repositories.News;

namespace BasePuzzle.Core.Scripts.Utils.Data
{
    using BasePuzzle.Core.Scripts.Repositories.News;

    [Obsolete("Use FDeviceInfoRepo instead")]
    public static class FDeviceInfo
    {
        public static string GameId => FDeviceInfoRepo.PackageName;
        public static string GameName => FDeviceInfoRepo.GameName;
        public static string Platform => FDeviceInfoRepo.Platform;
        public static string AppVersion => FDeviceInfoRepo.AppVersion;
        public static string SdkCoreVersion => FDeviceInfoRepo.SdkCoreVersion;
        public static string DeviceName => FDeviceInfoRepo.DeviceName;
        public static string DeviceId => FDeviceInfoRepo.DeviceId;
        public static DateTime FirstLogInDateTime => FPlayerInfoRepo.FirstLogInDateTime;

        public static long FirstLogInMillis => FPlayerInfoRepo.FirstLogInMillis;
    }
}