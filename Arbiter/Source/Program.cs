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

            Solution solution = new Solution();
            solution.compilation = true;
            solution.language = "C#";
            solution.realName = "Ivan Ivanov";
            solution.userName = "Ivan";
            solution.total = 100;
            solution.time = DateTime.Now;
            solution.source = @"using System;
using System.Diagnostics;
//[USING]

class Program
{
    static void Main(string[] args)
    {
        var watch = Stopwatch.StartNew();

        long beforeMemory = GC.GetTotalMemory(false);

        PerformSolution(args);

        long afterMemory = GC.GetTotalMemory(false);

        watch.Stop();

        long usedMemory = afterMemory - beforeMemory;

    }
    }";




            solution.results = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, Result>>();

            System.Collections.Generic.Dictionary<string, Result> r1 = new System.Collections.Generic.Dictionary<string, Result>();
            Result result1 = new Result();
            result1.verdict = Verdict.OK;
            result1.memoryUsed = 2.1f;
            result1.elapsedTime = 0.5f;
            r1.Add("01", result1);

            System.Collections.Generic.Dictionary<string, Result> r2 = new System.Collections.Generic.Dictionary<string, Result>();
            Result result2 = new Result();
            result2.verdict = Verdict.OK;
            result2.memoryUsed = 2.1f;
            result2.elapsedTime = 0.5f;
            r2.Add("02", result2);

            solution.results.Add("subtask1", r1);
            solution.results.Add("subtask2", r2);


            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(solution);

            File.WriteAllText(Path.Combine(WorkingDirectory, "testyaml.yaml"), yaml);
            


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