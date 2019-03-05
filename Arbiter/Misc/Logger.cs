using System;
using System.IO;
using System.Text;

namespace Arbiter
{
    public class Logger
    {
        private static Logger instance;

        private string logFilePath;

        /// <summary>
        /// Инициализация логгера
        /// </summary>
        public static void Initialization()
        {
            instance = new Logger();

            string path = Path.Combine(Program.WorkingDirectory, "Log");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            int i = 1;
            while (true)
            {
                instance.logFilePath = Path.Combine(path, $"[{i}] {DateTime.Now.ToString("yyyy'-'MM'-'dd")}.log");

                if (!File.Exists(instance.logFilePath))
                {
                    File.Create(instance.logFilePath).Close();
                    break;
                }
                else
                {
                    i++;
                }
            }
            var splitted = instance.logFilePath.Split('\\');
            string logFileName = splitted[splitted.Length - 1];
            Logger.Log($"Файл логов создан: \"{logFileName}\"");
        }

        /// <summary>
        /// Вывод простого сообщения
        /// </summary>
        /// <param name="message">текст сообщения</param>
        public static void Log(string message)
        {
            instance.Write($"[LOG] {message}");
        }

        /// <summary>
        /// Вывод ошибки
        /// </summary>
        /// <param name="message">текст ошибки</param>
        public static void Error(string message)
        {
            instance.Write($"[ERROR] {message}");
        }

        /// <summary>
        /// Вывод предупреждения
        /// </summary>
        /// <param name="message">текст предупреждения</param>
        public static void Warning(string message)
        {
            instance.Write($"[WARNING] {message}");
        }

        /// <summary>
        /// Запись текста в файл
        /// </summary>
        /// <param name="text">текст</param>
        private void Write(string text)
        {
            string output = $"[{DateTime.Now.ToString("HH':'mm':'ss")}] {text} {Environment.NewLine}";

            Console.Write(output);

            lock (instance)
            {
                File.AppendAllText(logFilePath, output, Encoding.UTF8);
            }
        }
    }
}