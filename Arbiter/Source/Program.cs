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


            //Task task = new Task();
            //task.name = "Гулливер";
            //task.timeout = 2f;
            //task.timeLimit = 1f;
            //task.memoryLimit = 64f;
            //task.inputFile = "input.txt";
            //task.outputFile = "output.txt";
            //task.preliminary = new System.Collections.Generic.Dictionary<string, string>();
            //task.testSuites = new System.Collections.Generic.Dictionary<string, Subtask>();

            //Subtask st1 = new Subtask();
            //st1.name = "Подзадача 1";
            //st1.scoring = Scoring.Partial;
            //st1.testScore = 10;
            //st1.results = "full";

            //Subtask st2 = new Subtask();
            //st2.name = "Подзадача 2";
            //st2.scoring = Scoring.Partial;
            //st2.testScore = 15;
            //st2.results = "full";

            //Subtask st3 = new Subtask();
            //st3.name = "Подзадача 3";
            //st3.scoring = Scoring.Partial;
            //st3.testScore = 20;
            //st3.results = "full";

            //task.testSuites.Add("subtask1", st1);
            //task.testSuites.Add("subtask2", st2);
            //task.testSuites.Add("subtask3", st3);

            //var serializer = new SerializerBuilder().EmitDefaults().Build();
            //var yaml = serializer.Serialize(task);
            //File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "task.yaml"), yaml);


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