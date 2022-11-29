using System;
using System.IO;

namespace Code.Logger
{
    public class GameLogger
    {
        private string _path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="classname">Class name is needed to create the Logfile</param>
        public GameLogger(string classname)
        {
            CreateLogFile(classname);
        }

        /// <summary>
        /// Class LogEvent
        /// Creates a Log Object, containing the Date, a Type and the Message
        /// </summary>
        private class LogEvent
        {
            public DateTime LogTime { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }
            public int LineNumber { get; set; }
        }
        
        /// <summary>
        /// Creates the Log File
        /// </summary>
        /// <param name="classname">Class name to Log</param>
        private void CreateLogFile(string classname)
        {
            var currentPath = Directory.GetCurrentDirectory();
            _path = $"{currentPath}/Assets/Code/Logger/Log Files/{classname}.log";
            if (!File.Exists(_path))
                File.Create(_path);
        }

        /// <summary>
        /// Logs the content
        /// </summary>
        /// <param name="type">Type can be, Exception, Information</param>
        /// <param name="message">Message to log</param>
        /// <param name="lineNumber">Current Line Number</param>
        public void LogEntry(string type, string message, int lineNumber)
        {
            AddLogEntry(new LogEvent{ LogTime = DateTime.Now, Type = type, Message = message, LineNumber = lineNumber});
        }

        /// <summary>
        /// Returns the Current LineNumber
        /// </summary>
        /// <returns>The Current Line Number</returns>
        public int GetLineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber; 
        }

        /// <summary>
        /// Appends the Entry into the LogFile
        /// </summary>
        /// <param name="log"></param>
        private void AddLogEntry(LogEvent log)
        {
            using StreamWriter writer = new StreamWriter(_path, append:true);
            writer.WriteLine($"LogTime = {log.LogTime} | LogType: {log.Type} \nLogMessage: {log.Message} \nLine Number: {log.LineNumber}");
        }
    }
}