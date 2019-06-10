using Arbiter;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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

            solution.results.Clear();

            for (int i = 0; i < subtasks.Length; i++)
            {
                var tests = Directory.GetFiles(subtasks[i]);

                var results = new Dictionary<string, Result>();

                for (int j = 0; j <= tests.Length - 2; j += 2)
                {
                    Result result = new Result();

                    PrepareTest(tests[j]);

                    result = MeasureSolution(args, main, instance, 1024 * 1024, 1000, tests[j + 1]);

                    //Clean();

                    results.Add((j / 2).ToString(), result);
                }

                solution.results.Add($"subtask{i}", results);
            }

            var finalYAML = serializer.Serialize(solution);
            File.WriteAllText(args[0], finalYAML);
        }

        private static void PrepareTest(string pathToTest)
        {
            var testText = File.ReadAllText(pathToTest);

            string pathToInput = Path.Combine(Directory.GetCurrentDirectory(), "input.txt");

            File.WriteAllText(pathToInput, testText);
        }

        private static void Clean()
        {
            string pathToOutput = Path.Combine(Directory.GetCurrentDirectory(), "output.txt");
            File.Delete(pathToOutput);
        }

        private static Result MeasureSolution(string[] args, MethodInfo main, object instance, long memoryLimit, long timeLimit, string pathToAnswer)
        {
            Stopwatch watch;

            watch = Stopwatch.StartNew();

            long beforeMemory = GC.GetTotalMemory(false);

            main.Invoke(instance, new object[] { args });

            long afterMemory = GC.GetTotalMemory(false);

            watch.Stop();

            Result result = new Result
            {
                elapsedTime = watch.ElapsedMilliseconds,
                memoryUsed = afterMemory - beforeMemory,
                verdict = Verdict.OK
            };

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

            if (!CheckAnswer(pathToAnswer))
            {
                result.verdict = Verdict.WA;
                return result;
            }
            else
            {
                result.verdict = Verdict.OK;
            }

            return result;
        }

        private static bool CheckAnswer(string pathToAnswer)
        {
            string pathToOutput = Path.Combine(Directory.GetCurrentDirectory(), "output.txt");
            string outputText = File.ReadAllText(pathToOutput);
            string answerText = File.ReadAllText(pathToAnswer);

            var outputSplitted = outputText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var answerSplitted = answerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (outputSplitted.Length != answerSplitted.Length)
                return false;

            for (int i = 0; i < outputSplitted.Length; i++)
            {
                answerSplitted[i] = Regex.Replace(answerSplitted[i], @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
                outputSplitted[i] = Regex.Replace(outputSplitted[i], @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);


                Console.WriteLine(answerSplitted[i].Length + " " + outputSplitted[i].Length);

                if (outputSplitted[i] == answerSplitted[i])
                    continue;
                else
                    return false;
            }

            return true;
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