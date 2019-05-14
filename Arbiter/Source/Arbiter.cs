using Arbiter.Misc;
using System;
using System.Collections.Generic;
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

        private string LanguagesPath;
        private string LanguagesFilePath;
        private string QueuePath;
        private string ResultsPath;

        private DateTime LanguagesLastChangeTime;
        private Dictionary<string, Language> Languages;

        public Arbiter()
        {
            LanguagesPath = CheckAndCreateDirectory("Languages");
            LanguagesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "languages.yaml");
            if (!File.Exists(LanguagesFilePath))
            {
                Logger.Log($"Файл languages.yaml создан");
                File.Create(LanguagesFilePath);
            }
            else
            {
                Logger.Log($"Файл languages.yaml существует");
            }

            QueuePath = CheckAndCreateDirectory("Queue");
            ResultsPath = CheckAndCreateDirectory("Results");

            Languages = new Dictionary<string, Language>();

            DeserializerBuilder builder = new DeserializerBuilder();
            Deserializer = builder.WithNamingConvention(new UnderscoredNamingConvention()).Build();
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
            sleep = LanguagesAreUnchanged() && ProblemsAreUnchanged() && QueueIsEmpty();
        }

        /// <summary>
        /// Проверка языков
        /// </summary>
        /// <returns>Если языки не изменились, то возращает true</returns>
        private bool LanguagesAreUnchanged()
        {
            bool areUnchanged = true;

            var lastChange = File.GetLastWriteTime(LanguagesFilePath);
            if (lastChange > LanguagesLastChangeTime)
            {
                areUnchanged = false;
                Logger.Log("Конфигурация языков была измненена");
                Languages = Language.Load(LanguagesFilePath, LanguagesPath);
                LanguagesLastChangeTime = lastChange;
            }

            return areUnchanged;
        }

        /// <summary>
        /// Проверка задач
        /// </summary>
        /// <returns>Если задачи не изменились, то возвращает true</returns>
        private bool ProblemsAreUnchanged()
        {
            return true;
        }

        /// <summary>
        /// Проверка файлов в очереди
        /// </summary>
        /// <returns>Возвращает true, если очередь пустая</returns>
        private bool QueueIsEmpty()
        {
            bool isEmpty = true;

            var queue = Directory.GetFiles(QueuePath);

            foreach (var fileName in queue)
            {
                Logger.Log($"Обработка файла: {fileName}");

                isEmpty = false;

                string newFileName = fileName.Replace(QueuePath, ResultsPath);
                File.Move(fileName, newFileName);

                ProcessSolution(newFileName);
            }

            return isEmpty;
        }

        /// <summary>
        /// Обработка решения
        /// </summary>
        /// <param name="fileName">путь до файла решения</param>
        private void ProcessSolution(string fileName)
        {
            string key = DefineCompiler(fileName);

            if (key == "error")
                return;

            Languages[key].CompileAndExecute(ResultsPath, fileName);
        }


        /// <summary>
        /// Определение компилятора по расширению файла
        /// </summary>
        /// <param name="fileName">путь до файла</param>
        /// <returns>возвращает ключ из словаря с языками</returns>
        private string DefineCompiler(string fileName)
        {
            var splitted = fileName.Split('.');
            switch (splitted[splitted.Length - 1])
            {
                case "cpp":
                case "c":
                    return "gcc";
                case "cs":
                    return "csc";
                default:
                    Logger.Error($"Неизвестный формат файла решения {fileName}");
                    return "error";
            }

        }
    }
}