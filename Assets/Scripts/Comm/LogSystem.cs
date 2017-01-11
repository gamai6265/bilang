namespace Cloth3D {
    public enum LogType {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public delegate void LogOutputDelegate(LogType type, string msg);

    internal class LogSystem {
        public static LogOutputDelegate OnLogOutput;

        public static void Debug(string format, params object[] args) {
            var str = string.Format("[Debug]:" + format, args);
            if (null != OnLogOutput) {
                OnLogOutput(LogType.Debug, str);
            }
        }

        public static void Info(string format, params object[] args) {
            var str = string.Format("[Info]:" + format, args);
            if (null != OnLogOutput) {
                OnLogOutput(LogType.Info, str);
            }
        }

        public static void Warn(string format, params object[] args) {
            var str = string.Format("[Warn]:" + format, args);
            if (null != OnLogOutput) {
                OnLogOutput(LogType.Warn, str);
            }
        }

        public static void Error(string format, params object[] args) {
            var str = string.Format("[Error]:" + format, args);
            if (null != OnLogOutput) {
                OnLogOutput(LogType.Error, str);
            }
        }

        public static void Fatal(string format, params object[] args) {
            var str = string.Format("[Fatal]:" + format, args);
            if (null != OnLogOutput) {
                OnLogOutput(LogType.Fatal, str);
            }
        }

        public static void Output(LogType type, string msg) {
            if (null != OnLogOutput) {
                OnLogOutput(type, msg);
            }
        }
    }
}