using System;
using System.IO;

namespace Arbiter
{
    public class Debug
    {
        private static Debug instance;

        private FileStream fileStream;

        public static void Initialization()
        {
            instance = new Debug();

            string path = Path.Combine(Program.WorkingDirectory, "Log");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.Combine(path, $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}.log");

            instance.fileStream = File.Create(fileName);
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);

        }

        public static void LogError(string message)
        {
            Console.WriteLine(message);

        }

        public static void LogWarning(string message)
        {
            Console.WriteLine(message);

        }
    }
}