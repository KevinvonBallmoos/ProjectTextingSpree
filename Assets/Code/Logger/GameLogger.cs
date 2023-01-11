using System;
using System.Collections.Generic;
using System.IO;

namespace Code.Logger
{
    /// <summary>
    /// Logger Class, to Log Messages and Exceptions
    /// </summary>
    /// <para name="author">Kevin von Ballmoos></para>
    /// <para name="date">04.12.2022</para>
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
            AddLogEntry(new LogEvent
            {
                LogTime = DateTime.Now, 
                Type = type, 
                Message = message, 
                LineNumber = lineNumber
            });
        }

        /// <summary>
        /// Returns the Current LineNumber
        /// </summary>
        /// <returns>The Current Line Number</returns>
        public static int GetLineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber; 
        }

        /// <summary>
        /// Appends the Entry into the LogFile
        /// </summary>
        /// <param name="log">Log Event Object</param>
        private void AddLogEntry(LogEvent log)
        {
            using (var writer = new StreamWriter(_path, append:true)){
                writer.WriteLine($"LogTime = {log.LogTime} | LogType: {log.Type} \nLogMessage: {log.Message} \nLine Number: {log.LineNumber}");
            }
            RemoveLogEntry(_path);
        }

        /// <summary>
        /// Removes Log Entry
        /// </summary>
        private static void RemoveLogEntry(string path)
        {
            var hasEntryToRemove = true;
            while (hasEntryToRemove)
            {
                var readLines = new List<string>();
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                        readLines.Add(reader.ReadLine());
                }
                
                var writeLines = new List<string>();
                if ((DateTime.Now - Convert.ToDateTime(readLines[0].Substring(10, 19))).Days > 5)
                {
                    for (int i = 3; i < readLines.Count; i++)
                        writeLines.Add(readLines[i]);
                    using var writer = new StreamWriter(path);
                    foreach (var line in writeLines)
                        writer.WriteLine(line);
                }
                else
                {
                    hasEntryToRemove = false;
                }
            }
        }
    }
}