using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Console = ModAPI.Console;

namespace GriefClientPro.Utils
{
    public static class Logger
    {
        private static readonly string LogsDirectory = Path.Combine(GriefClientPro.DataFolderLocation, "Logs");
        private static readonly string LogFileLocation = Path.Combine(LogsDirectory, "Logger.txt");

        private static readonly List<string> QueuedLines = new List<string>();
        private static readonly Timer LogSaveTimer = new Timer(5000);

        private const int MaxLogFileSizeMb = 10;

        static Logger()
        {
            // Delete old log file
            File.Delete(LogFileLocation);

            // Initialize
            LogSaveTimer.Elapsed += OnLogSaveTimerElapsed;
            LogSaveTimer.Start();
        }

        private static void OnLogSaveTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!Directory.Exists(LogsDirectory))
            {
                Directory.CreateDirectory(LogsDirectory);
            }
            
            var fileInfo = new FileInfo(LogFileLocation);
            if (fileInfo.Exists)
            {
                // Validate log file size
                if (fileInfo.Length / 1024f / 1024f >= MaxLogFileSizeMb)
                {
                    return;
                }
            }
            else
            {
                // Create log file
                fileInfo.Create().Close();
            }

            lock (QueuedLines)
            {
                if (QueuedLines.Count > 0)
                {
                    using (var file = fileInfo.AppendText())
                    {
                        foreach (var line in QueuedLines)
                        {
                            file.WriteLine(line);
                        }
                    }
                    QueuedLines.Clear();
                }
            }
        }

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Severe
        }
        
        private static void WriteLog(params string[] message)
        {
            lock (QueuedLines)
            {
                if (message.Length == 1)
                {
                    QueuedLines.Add(message[0]);
                }
                else if (message.Length > 1)
                {
                    var messages = new List<string>(message);
                    messages.Reverse();
                    QueuedLines.AddRange(messages);
                }
                
            }

            foreach (var line in message)
            {
                Console.Write(line, GriefClientPro.ModName);
            }
        }

        public static void Log(LogLevel level, string format, params object[] args)
        {
            WriteLog(GetLogString(level, format, args));
        }

        public static void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            Log(LogLevel.Warning, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        public static void Severe(string format, params object[] args)
        {
            Log(LogLevel.Severe, format, args);
        }

        private static string GetLogString(LogLevel level, string format, params object[] args)
        {
            return $"[{DateTime.Now:H:mm:ss} - {level}] {string.Format(format, args)}";
        }

        public static void Exception(LogLevel logLevel, string headerMessage, object exceptionObject, params object[] args)
        {
            var completeMessage = new List<string>
            {
                GetLogString(logLevel, "==================================================="),
                GetLogString(logLevel, headerMessage, args),
                GetLogString(logLevel, ""),
                GetLogString(logLevel, "Stacktrace of the Exception:"),
                GetLogString(logLevel, "")
            };
            var exception = exceptionObject as Exception;
            if (exception != null)
            {
                completeMessage.Add(GetLogString(logLevel, "Type: {0}", exception.GetType().FullName));
                completeMessage.Add(GetLogString(logLevel, "Message: {0}", exception.Message));
                completeMessage.Add(GetLogString(logLevel, ""));
                completeMessage.Add(GetLogString(logLevel, "Stracktrace:"));
                completeMessage.Add(GetLogString(logLevel, exception.StackTrace));
                exception = exception.InnerException;
                if (exception != null)
                {
                    completeMessage.Add(GetLogString(logLevel, ""));
                    completeMessage.Add(GetLogString(logLevel, "InnerException(s):"));
                    do
                    {
                        completeMessage.Add(GetLogString(logLevel, "---------------------------------------------------"));
                        completeMessage.Add(GetLogString(logLevel, "Type: {0}", exception.GetType().FullName));
                        completeMessage.Add(GetLogString(logLevel, "Message: {0}", exception.Message));
                        completeMessage.Add(GetLogString(logLevel, ""));
                        completeMessage.Add(GetLogString(logLevel, "Stracktrace:"));
                        completeMessage.Add(GetLogString(logLevel, exception.StackTrace));
                        exception = exception.InnerException;
                    } while (exception != null);
                    completeMessage.Add(GetLogString(logLevel, "---------------------------------------------------"));
                }
            }
            completeMessage.Add(GetLogString(logLevel, "==================================================="));

            completeMessage.Reverse();

            var finalMessage = new List<string>();
            foreach (var line in completeMessage)
            {
                var split = line.Split('\n');
                if (split.Length > 1)
                {
                    finalMessage.Add(split[0]);
                    for (var i = 1; i < split.Length; i++)
                    {
                        finalMessage.Add(GetLogString(logLevel, split[i]));
                    }
                }
                else
                {
                    finalMessage.Add(line);
                }
            }
            WriteLog(finalMessage.ToArray());
        }

        public static void Exception(string headerMessage, object exceptionObject, params object[] args)
        {
            Exception(LogLevel.Error, headerMessage, exceptionObject, args);
        }
    }
}
