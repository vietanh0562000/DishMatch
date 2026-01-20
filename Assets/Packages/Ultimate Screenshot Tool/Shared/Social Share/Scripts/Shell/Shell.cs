#if UNITY_EDITOR
using System.Diagnostics;
using System.Collections.Generic;
#endif

namespace TRS.CaptureTool.Share
{
    public static class Shell
    {
#if UNITY_EDITOR
        public class ShellEvents
        {
            public event System.Action<string> LogEvent;
            public event System.Action ErrorEvent;
            public event System.Action DoneEvent;

            public void Log(string log)
            {
                if (LogEvent != null)
                    LogEvent(log);
            }

            public void Error()
            {
                if (ErrorEvent != null)
                    ErrorEvent();
            }

            public void Done()
            {
                if (DoneEvent != null)
                    DoneEvent();
            }
        }

        static string shellName
        {
            get
            {
#if UNITY_EDITOR_WIN
            return "cmd.exe";
#else
                return "bash";
#endif
            }
        }

        static string commandPrefix
        {
            get
            {
#if UNITY_EDITOR_WIN
            return "/c";
#else
                return "-c";
#endif
            }
        }

        public static void RunCommand(string command, string workDirectory, ShellEvents shellEvents = null, List<string> environmentVars = null)
        {
            RunCommandImpl(command, workDirectory, shellEvents, environmentVars);
        }

        public static void RunCommandAsync(string command, string workDirectory, ShellEvents shellEvents = null, List<string> environmentVars = null)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                RunCommandImpl(command, workDirectory, shellEvents, environmentVars);
            });
        }

        static void RunCommandImpl(string command, string workDirectory, ShellEvents shellEvents = null, List<string> environmentVars = null)
        {
            if (shellEvents == null)
                shellEvents = new ShellEvents();

            Process process = ProcessWithErrorHandling();
            process.StartInfo = ProcessInfo(command, workDirectory, environmentVars);
            RunProcess(process, shellEvents);
        }

        public static void RunProcess(Process process, ShellEvents shellEvents)
        {
            process.Start();
            while (true)
            {
                string line = process.StandardOutput.ReadLine();
                if (line == null)
                    break;
                line = line.Replace("\\", "/");

                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    shellEvents.Log(line);
                });
            }

            bool hasError = false;
            while (true)
            {
                string error = process.StandardError.ReadLine();
                if (string.IsNullOrEmpty(error))
                    break;

                hasError = true;
                UnityToolbag.Dispatcher.Invoke(() =>
                {
                    shellEvents.Log(error);
                });
            }

            process.Close();
            UnityToolbag.Dispatcher.Invoke(() =>
            {
                if (hasError)
                    shellEvents.Error();
                else
                    shellEvents.Done();
            });
        }

        public static ProcessStartInfo ProcessInfo(string command, string workDirectory, List<string> environmentVars = null)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo(shellName);
            if (environmentVars != null)
            {
                foreach (string var in environmentVars)
                {
                    processInfo.EnvironmentVariables["PATH"] += (System.IO.Path.PathSeparator + var);
                }
            }

            processInfo.Arguments = commandPrefix + " \"" + command + "\"";
            processInfo.WorkingDirectory = workDirectory;
            processInfo.UseShellExecute = false;

            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            return processInfo;
        }

        static Process ProcessWithErrorHandling()
        {
            Process process = new Process();
            process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                UnityEngine.Debug.LogError(e.Data);
            };
            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                UnityEngine.Debug.LogError(e.Data);
            };
            process.Exited += delegate (object sender, System.EventArgs e)
            {
                UnityEngine.Debug.LogError(e.ToString());
            };
            return process;
        }
#endif
    }
}