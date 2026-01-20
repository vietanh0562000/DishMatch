namespace TRS.CaptureTool.Share
{
    public static class ShellCommands
    {
#if UNITY_EDITOR

        // Convert to MP4
        const string MP4_COMMAND = "ffmpeg -i {0} -movflags faststart -pix_fmt yuv420p -vf \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" {1}";

        // Copy to custom server
        // Windows users may need to install scp: https://success.tanaza.com/s/article/How-to-use-SCP-command-on-Windows
#if UNITY_EDITOR_WIN
        const string SCP_COMMAND = "pscp -scp";
#else
        const string SCP_COMMAND = "scp";
#endif
        const string CURL_COMMAND = "curl";

        public static void ConvertToMP4(string filePath, Shell.ShellEvents shellEvents = null)
        {
            string directory = System.IO.Path.GetDirectoryName(filePath);
            string fileName = System.IO.Path.GetFileName(filePath);

            string mp4Command = string.Format(MP4_COMMAND, fileName, fileName.Replace("gif", "mp4"));
            Shell.RunCommandAsync(mp4Command, directory, shellEvents);
        }

        public static void SerialCopy(string filePath, string serverUser, string serverDirectory, Shell.ShellEvents shellEvents = null)
        {
            string directory = System.IO.Path.GetDirectoryName(filePath);
            string fileName = System.IO.Path.GetFileName(filePath);

            string scpCommand = SCP_COMMAND + " " + fileName + " " + serverUser + ":" + serverDirectory;
            Shell.RunCommandAsync(scpCommand, directory, shellEvents);
        }
#endif
    }
}