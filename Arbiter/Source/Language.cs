using Arbiter.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using YamlDotNet.Core;

namespace Arbiter
{
    public class Language
    {
        public string Name;
        public string Execution;
        public string Compilation;

        public string CompilationBatch;
        public string templateText;

        public EventHandler<StringArgs> Compiled;

        public void CompileCCPP(string workingDir, string fileName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = fileName,
                FileName = CompilationBatch,
                WorkingDirectory = workingDir,
                UseShellExecute = false
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
            process.WaitForExit();
            Logger.Log("Процесс компиляции C/C++ завершен");
        }

        /// <summary>
        /// Запуск исполняемого файла скомпилированного кода
        /// </summary>
        public void ExecuteCCPP(string workingDir, string fileName)
        {
            //uint memory = 0;
            //uint time = 0;
            //uint verdict = 0;

            ////Invoker.InteractiveInputOutput(fileName, "", "", ref verdict, ref memory, ref time);
            //Invoker2.MesaureProcess(fileName, "", "", out memory, out time);
            //Logger.Log($"Процесс исполнения завершен.\nИспользованная память: {memory} bytes\nЗатраченное время: {time} ms");

            Verdict verdict = Verdict.None;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = "",
                FileName = fileName,
                WorkingDirectory = workingDir,
                UseShellExecute = false
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            long memory = 0;

            var watch = Stopwatch.StartNew();

            process.Start();

            double timer = 0;

            while (!process.HasExited)
            {
                memory = process.PagedMemorySize64;
                if (watch.ElapsedMilliseconds > 10000)
                {
                    verdict = Verdict.TimeLimit;
                    break;
                }
            }

            watch.Stop();
            
            if (verdict != Verdict.TimeLimit)
            {
                if (memory >= 10000000)
                    verdict = Verdict.MemoryLimit;
            }

            Logger.Log("Процесс выполнения C/C++ завершен " + memory + " " + watch.ElapsedMilliseconds);

            if (verdict == Verdict.None)
                verdict = Verdict.Accept;

            WriteResult(fileName + ".res", memory, watch.ElapsedMilliseconds, verdict);
        }

        private void WriteResult(string fileName, long memory, double time, Verdict verdict)
        {
            File.WriteAllText(fileName, $"UsedMemory: {memory}\nElapsedTime: {time}\nVerdict: {verdict.ToString()}");
        }

        public void CompileCSharp(string workingDir, string fileName)
        {
            string code = File.ReadAllText(fileName);

            var a = code.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

            string newCode = templateText;

            //newCode = newCode.Replace("//[USING]", a[0]);

            string method = "";
            foreach (var s in a)
            {
                if (s.Contains("static void Main") || s.Contains("class") || string.IsNullOrEmpty(s))
                    continue;
                method += s;
            }

            newCode = newCode.Replace("//[CODE]", method);

            string newFileName = Path.Combine(workingDir, "ttt.cs"); 

            File.WriteAllText(newFileName, newCode);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = newFileName,
                FileName = CompilationBatch,
                WorkingDirectory = workingDir,
                UseShellExecute = false
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
            process.WaitForExit();
            Logger.Log("Процесс компиляции C# завершен");
        }

        public void ExecuteCSharp(string workingDir, string fileName)
        {
            long memory = 0;

            Logger.Log("Процесс выполнения C# завершен " + memory);
        }

        /// <summary>
        /// Загрузка списка языков программирования из languages.yaml
        /// </summary>
        /// <param name="languagesFilePath">путь до файла languages.yaml</param>
        /// <param name="languagesPath">путь до папки с бат-файлами для компиляции</param>
        /// <returns>возвращает список языков</returns>
        public static Dictionary<string, Language> Load(string languagesFilePath, string languagesPath)
        {
            Dictionary<string, Language> result = new Dictionary<string, Language>();

            var oldBatch = Directory.GetFiles(languagesPath, "*.bat");
            foreach (var batch in oldBatch)
            {
                File.Delete(batch);
                Logger.Log($"{batch} удален");
            }

            try
            {
                var text = File.ReadAllText(languagesFilePath);
                result = Arbiter.Deserializer.Deserialize<Dictionary<string, Language>>(text);
                Logger.Log($"Список языков программирования загружен: {result.Count}");
            }
            catch (YamlException ex)
            {
                if (ex.InnerException == null)
                {
                    Logger.Error($"Ошибка при десериализации languages.yaml:\nYamlException: {ex.Message}");
                }
                else
                {
                    Logger.Error($"Ошибка при десериализации languages.yaml:\nYamlException: {ex.Message}\nInnerException: {ex.InnerException.Message}");
                }
            }

            foreach (var l in result)
            {
                string code = l.Key;
                Language lang = l.Value;
                lang.Name = l.Key;
                lang.CreateBatch(code, languagesPath);

                if (l.Key == "csc")
                {
                    var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Templates\\CSharpTemplate.txt");
                    lang.templateText = File.ReadAllText(fileName);
                }
            }

            return result;
        }

        /// <summary>
        /// Создание batch файла для компиляции кода
        /// </summary>
        /// <param name="code">компилятор</param>
        /// <param name="languagesPath">путь до директории с batch файлами</param>
        private void CreateBatch(string code, string languagesPath)
        {
            CompilationBatch = Path.Combine(languagesPath, $"{code}.bat");
            File.WriteAllText(CompilationBatch, Compilation);
            Logger.Log($"{code}.bat создан");
        }
    }
}