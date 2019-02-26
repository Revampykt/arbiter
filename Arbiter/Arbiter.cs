using System.IO;

namespace Arbiter
{
    /// <summary>
    /// Арбитр
    /// </summary>
    public class Arbiter
    {
        private readonly string LanguagesPath;
        private readonly string QueuePath;
        private readonly string ResultsPath;

        public bool sleep;

        public Arbiter()
        {
            LanguagesPath = CheckAndCreateDirectory("Languages");
            QueuePath = CheckAndCreateDirectory("Queue");
            ResultsPath = CheckAndCreateDirectory("Results");
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
            return true;
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


            }

            return isEmpty;
        }
    }
}