namespace ShiftPad.Core.Utility
{
    [Flags]
    public enum LogLevel
    {
        Debug = 1,
        Info = 1 << 1,
        Warning = 1 << 2,
        Error = 1 << 3,

        Default = Info | Warning | Error,
        All = Debug | Default
    }
    
    public class Logger
    {
        public delegate void LogDelegate(string message, LogLevel level);

        private static LogLevel _logLevel = LogLevel.Default;
        private static LogDelegate _logDelegate;

        static Logger()
        {
            _logDelegate = DebugLogDelegate;

#if DEBUG
            _logLevel = LogLevel.All;
#endif
        }

        public static Logger GetInstance(Type type)
        {
            return new Logger(type.ToString(), DoLog);
        }

        public static Logger GetInstance(Type type, string prefix)
        {
            return new Logger($"{type} | {prefix}", DoLog);
        }

        public static void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
        }

        public static void SetLogDelegate(LogDelegate logDelegate)
        {
            _logDelegate = logDelegate;
        }

        private static void DebugLogDelegate(string message, LogLevel level)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"{level}: {message}");
#endif
        }

        private static void DoLog(string message, LogLevel logLevel)
        {
            _logDelegate?.Invoke(message, logLevel);
        }



        private string _messagePrefix;
        private LogDelegate _logOutput;

        private Logger(string messagePrefix, LogDelegate logOutput)
        {
            _messagePrefix = messagePrefix;
            _logOutput = logOutput;
        }

        public void LogDebug(string message) => Log(message, LogLevel.Debug);
        public void LogInfo(string message) => Log(message, LogLevel.Info);
        public void LogWarning(string message) => Log(message, LogLevel.Warning);
        public void LogError(string message) => Log(message, LogLevel.Error);
        public void Log(string message, LogLevel logLevel)
        {
            if (_logLevel.HasFlag(logLevel))
            {
                _logOutput($"({_messagePrefix}) {message}", logLevel);
            }
        }

        /// <summary>
        /// Log an array of bytes (Debug Only).
        /// </summary>
        public void Log(byte[] bytes)
        {
            if (_logLevel.HasFlag(LogLevel.Debug))
            {
                Log(BitConverter.ToString(bytes), LogLevel.Debug);
            }
        }
    }
}
