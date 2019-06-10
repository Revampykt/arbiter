using Arbiter;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
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

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = true
            };

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, solution.source);
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }

                throw new InvalidOperationException(sb.ToString());
            }

            Assembly assembly = results.CompiledAssembly;

            Type program = assembly.GetType("Program");

            MethodInfo main = program.GetMethod("Main");

            var ms = program.GetMethods();
            foreach (var m in ms)
            {
                Console.WriteLine(m.Name);
                var ps = m.GetParameters();
                foreach (var p in ps)
                    Console.WriteLine(p.ParameterType.Name);
            }

            var instance = assembly.CreateInstance("Program");

            var type = instance.GetType();

            var watch = Stopwatch.StartNew();

            long beforeMemory = GC.GetTotalMemory(false);

            main.Invoke(instance, new object[] { args });

            long afterMemory = GC.GetTotalMemory(false);

            watch.Stop();

            long usedMemory = afterMemory - beforeMemory;

            Console.WriteLine("UsedMemory = " + usedMemory + " ElapsedTime = " + watch.ElapsedMilliseconds);
        }
    }
}