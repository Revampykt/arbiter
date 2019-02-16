using System;
using System.Threading;

namespace Arbiter
{
    public class Program
    {
        public static string WorkingDirectory = "";

        private static bool IsCancelled = false;

        public static void Main(string[] args)
        {
            if (args.Length > 0)
                WorkingDirectory = args[0];


            if (WorkingDirectory == "")
            {
                WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
                Debug.Initialization();
                Debug.LogWarning("Не указана рабочая директория.");
            }
            else
            {
                Debug.Initialization();
            }

            Console.CancelKeyPress += Console_CancelKeyPress;

            int k = 0;

            while (!IsCancelled)
            {
                Debug.Log(k++.ToString());
                Thread.Sleep(500);
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                IsCancelled = true;
                e.Cancel = true;
            }
        }
    }
}