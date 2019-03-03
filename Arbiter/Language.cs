using System.Collections.Generic;
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

        public void Compile()
        {

        }

        public void Execute()
        {

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
                lang.CreateBatch(code, languagesPath);
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