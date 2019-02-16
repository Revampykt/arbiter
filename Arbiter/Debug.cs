using System;
using System.IO;
using System.Text;

namespace Arbiter
{
    public class Debug
    {
        private static Debug instance;

        private string logFilePath;

        public static void Initialization()
        {
            instance = new Debug();

            string path = Path.Combine(Program.WorkingDirectory, "Log");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            instance.logFilePath = Path.Combine(path, $"{DateTime.Now.ToString("yyyy'-'MM'-'dd")}.log");

            if (!File.Exists(instance.logFilePath))
            {
                File.Create(instance.logFilePath).Close();
            }
        }

        public static void Log(string message)
        {
            instance.Write($"[{DateTime.Now.ToString("HH':'mm':'ss")}] [LOG] {message}");
        }

        public static void LogError(string message)
        {
            instance.Write($"[{DateTime.Now.ToString("HH':'mm':'ss")}] [ERROR] {message}");
        }

        public static void LogWarning(string message)
        {
            instance.Write($"[{DateTime.Now.ToString("HH':'mm':'ss")}] [WARNING] {message}");
        }

        private void Write(string text)
        {
            Console.WriteLine(text);

            lock (instance)
            {
                File.AppendAllText(logFilePath, text + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}