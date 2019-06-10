using System;
using System.IO;
using System.Threading;
using YamlDotNet.Serialization;

namespace Arbiter
{
    public class Program
    {
        public static string WorkingDirectory = "";

        private static bool IsCancelled = false;

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                WorkingDirectory = args[0];
            }

            foreach (var a in args)
            {
                Console.WriteLine(a);
            }

            if (string.IsNullOrEmpty(WorkingDirectory))
            {
                WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "WorkingDirectory");
                Logger.Initialization();
                Logger.Warning($"Не указана рабочая директория, программа будет работать в директории {WorkingDirectory}");
            }
            else
            {
                Logger.Initialization();
            }

            Logger.Log("Арбитр запущен");

            Console.CancelKeyPress += Console_CancelKeyPress;

            Arbiter arbiter = new Arbiter();

            while (!IsCancelled)
            {
                try
                {
                    arbiter.Update();
                    if (arbiter.sleep)
                        Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Message: {ex.Message}\nStackTrace: {ex.StackTrace}");
                }
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                Logger.Log("Арбитр остановлен пользователем");
                IsCancelled = true;
                e.Cancel = true;
            }
        }
    }
}