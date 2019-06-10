namespace Arbiter
{
    public enum Verdict
    {
        None,
        /// <summary>
        /// Тест пройден успешно
        /// </summary>
        OK,
        /// <summary>
        /// Неверный ответ
        /// </summary>
        WA,
        /// <summary>
        /// Лимит по времени
        /// </summary>
        TL,
        /// <summary>
        /// Лимит по памяти
        /// </summary>
        ML,
        /// <summary>
        /// Лимит ожидания
        /// </summary>
        IL,
        /// <summary>
        /// Ошибка компиляции
        /// </summary>
        CE,
        /// <summary>
        /// Ошибка исполнения
        /// </summary>
        RE,
        /// <summary>
        /// Ошибка вывода
        /// </summary>
        PE
    }
}