using Arbiter;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Task = Arbiter.Task;

namespace CSharpInvoker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
                return;

            string input = File.ReadAllText(args[0]);

            var deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
            var solution = deserializer.Deserialize<Solution>(input);

            var serializer = new SerializerBuilder().EmitDefaults().Build();

            var compilationResults = CompileSource(solution.source);

            if (compilationResults.Errors.HasErrors)
            {
                solution.compilation = false;
                var yaml = serializer.Serialize(solution);
                File.WriteAllText(args[0], yaml);
                Console.WriteLine("Compilation error");
                return;
            }

            solution.compilation = true;

            Assembly assembly = compilationResults.CompiledAssembly;

            Type program = assembly.GetType("Program");

            MethodInfo main = program.GetMethod("Main");

            var instance = assembly.CreateInstance("Program");

            var type = instance.GetType();

            string pathToTests = Path.Combine(Directory.GetCurrentDirectory(), "Tests");
            var subtasks = Directory.GetDirectories(pathToTests);

            var task = LoadTask(pathToTests);

            solution.results.Clear();

            for (int i = 0; i < subtasks.Length; i++)
            {
                var tests = Directory.GetFiles(subtasks[i]);

                var results = new Dictionary<string, Result>();

                for (int j = 0; j <= tests.Length - 2; j += 2)
                {
                    Result result = new Result();

                    PrepareTest(tests[j], task.inputFile);

                    result = MeasureSolution(args, main, instance, 1024 * 1024 * task.memoryLimit, task.timeLimit * 1000, tests[j + 1], task.outputFile);

                    results.Add(((j / 2) + 1).ToString(), result);
                }

                solution.results.Add($"subtask{i + 1}", results);
            }

            solution.total = CalculateScore(solution.results, task.testSuites);

            var finalYAML = serializer.Serialize(solution);
            File.WriteAllText(args[0], finalYAML);

            Clean(task.inputFile, task.outputFile);
        }

        private static Task LoadTask(string pathToTests)
        {
            string taskConfig = File.ReadAllText(Path.Combine(pathToTests, "task.yaml"));

            var deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
            var task = deserializer.Deserialize<Task>(taskConfig);

            return task;
        }

        private static int CalculateScore(Dictionary<string, Dictionary<string, Result>> results, Dictionary<string, Subtask> testSuites)
        {
            int total = 0;
            foreach (var result in results)
            {
                int accepted = 0;
                foreach (var verdict in result.Value)
                {
                    if (verdict.Value.verdict == Verdict.OK)
                        accepted++;
                }

                total += accepted * testSuites[result.Key].testScore;
            }

            return total;
        }

        private static void PrepareTest(string pathToTest, string inputFile)
        {
            var testText = File.ReadAllText(pathToTest);

            string pathToInput = Path.Combine(Directory.GetCurrentDirectory(), inputFile);

            File.WriteAllText(pathToInput, testText);
        }

        private static void Clean(string inputFile, string outputFile)
        {
            string pathToOutput = Path.Combine(Directory.GetCurrentDirectory(), inputFile);
            string pathToInput = Path.Combine(Directory.GetCurrentDirectory(), outputFile);
            File.Delete(pathToOutput);
            File.Delete(pathToInput);
        }

        private static Result MeasureSolution(string[] args, MethodInfo main, object instance, long memoryLimit, long timeLimit, string pathToAnswer, string outputFile)
        {
            Stopwatch watch = null;

            

            long beforeMemory = 0;
            long afterMemory = 0;

            Verdict runtimeVerdict = Verdict.OK;

            try
            {
                watch = Stopwatch.StartNew();
                beforeMemory = GC.GetTotalMemory(false);

                main.Invoke(instance, new object[] { args });

                afterMemory = GC.GetTotalMemory(false);
                watch.Stop();
            }
            catch
            {
                runtimeVerdict = Verdict.RE;
                watch.Stop();
            }

            GC.Collect();

            Result result = new Result
            {
                elapsedTime = watch.ElapsedMilliseconds,
                memoryUsed = afterMemory - beforeMemory,
                verdict = Verdict.OK
            };

            if (runtimeVerdict == Verdict.RE)
            {
                result.verdict = runtimeVerdict;
                return result;
            }

            if (result.memoryUsed > memoryLimit)
            {
                result.verdict = Verdict.ML;
                return result;
            }

            if (result.elapsedTime > timeLimit)
            {
                result.verdict = Verdict.TL;
                return result;
            }

            if (!CheckAnswer(pathToAnswer, outputFile, out result.verdict))
            {
                return result;
            }
            else
            {
                result.verdict = Verdict.OK;
            }

            return result;
        }

        private static bool CheckAnswer(string pathToAnswer, string outputFile, out Verdict verdict)
        {
            string pathToOutput = Path.Combine(Directory.GetCurrentDirectory(), outputFile);
            string outputText = File.ReadAllText(pathToOutput);
            string answerText = File.ReadAllText(pathToAnswer);

            var outputSplitted = outputText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var answerSplitted = answerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (outputSplitted.Length != answerSplitted.Length)
            {
                verdict = Verdict.PE;
                return false;
            }
            for (int i = 0; i < outputSplitted.Length; i++)
            {
                answerSplitted[i] = RemoveEmptyLines(answerSplitted[i]);
                outputSplitted[i] = RemoveEmptyLines(outputSplitted[i]);

                if (outputSplitted[i] == answerSplitted[i])
                    continue;
                else
                {
                    verdict = Verdict.WA;
                    return false;
                }
            }

            verdict = Verdict.OK;
            return true;
        }

        private static string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }

        private static CompilerResults CompileSource(string source)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = true
            };

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);

            return results;
        }
    }
}