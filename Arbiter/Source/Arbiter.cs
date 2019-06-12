using System.Diagnostics;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Arbiter
{
    /// <summary>
    /// Арбитр
    /// </summary>
    public class Arbiter
    {
        public bool sleep;

        public static IDeserializer Deserializer;

        private string QueuePath;
        private string ResultsPath;
        private string TestsPath;

        private string CSharpInvokerEXE;

        public Arbiter()
        {
            QueuePath = CheckAndCreateDirectory("Queue");
            ResultsPath = CheckAndCreateDirectory("Results");
            Deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();

            CSharpInvokerEXE = Path.Combine(Directory.GetCurrentDirectory(), "CSharpInvoker.exe");
            TestsPath = Path.Combine(Directory.GetCurrentDirectory(), "Tests");
        }

        /// <summary>
        /// Проверяет существование директории и создает её, если необходимо
        /// </summary>
        /// <param name="directory">базовая директория</param>
        /// <returns>возвращает путь до проверяемой директории</returns>
        private string CheckAndCreateDirectory(string directory)
        {
            string path = Path.Combine(Program.WorkingDirectory, directory);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Logger.Log($"Директория {directory} создана");
            }
            else
            {
                Logger.Log($"Директория {directory} уже существует");
            }
            return path;
        }

        /// <summary>
        /// Вызов этой функции выполняет обновление текущего состояния арбитра.
        /// </summary>
        public void Update()
        {
            sleep = QueueIsEmpty();
        }

        /// <summary>
        /// Проверка файлов в очереди
        /// </summary>
        /// <returns>Возвращает true, если очередь пустая</returns>
        private bool QueueIsEmpty()
        {
            bool isEmpty = true;

            var queue = Directory.GetFiles(QueuePath);

            foreach (var solutionFile in queue)
            {
                Logger.Log($"Обработка файла: {solutionFile}");

                isEmpty = false;

                string newSolutionFile = solutionFile.Replace(QueuePath, ResultsPath);
                File.Move(solutionFile, newSolutionFile);

                ProcessSolution(newSolutionFile);
            }

            return isEmpty;
        }

        /// <summary>
        /// Обработка решения
        /// </summary>
        /// <param name="solution">путь до файла решения</param>
        private void ProcessSolution(string solutionFile)
        {
            var input = File.ReadAllText(solutionFile);

            var solution = Deserializer.Deserialize<Solution>(input);

            switch (solution.language)
            {
                case "C#":
                    ProcessCSharpSolution(solutionFile);
                    break;
                case "C":
                    ProcessCSolution(solutionFile);
                    break;
                case "C++":
                    ProcessCPPSolution(solutionFile);
                    break;
                case "Python":
                    ProcessPythonSolution(solutionFile);
                    break;
            }
        }

        private void ProcessCSharpSolution(string solutionFile)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = solutionFile + " " + TestsPath,
                FileName = CSharpInvokerEXE,
                WorkingDirectory = ResultsPath,
                UseShellExecute = false
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit();

            Logger.Log($"Файл проверен: {solutionFile}");
        }

        private void ProcessCSolution(string solutionFile)
        {

        }

        private void ProcessCPPSolution(string solutionFile)
        {

        }

        private void ProcessPythonSolution(string solutionFile)
        {

        }
    }
}