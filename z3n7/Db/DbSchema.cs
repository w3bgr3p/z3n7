namespace z3n7
{
    /// <summary>
    /// Централизованное хранилище имён таблиц с дефолтными значениями.
    ///
    /// Источники:
    ///   TaskManager.cs  — _settings, _tasks, _commands
    ///   DbExtencions.cs — _wlt (хардкод на строке 150, в SqlGet цепочки кошелька)
    /// </summary>
    public static class DbSchema
    {
        // ── TaskManager ───────────────────────────────────────────────────────

        /// <summary>
        /// InputSettings каждой задачи (переменные + _xml в base64).
        /// TaskManager._settingsTable
        /// </summary>
        public static string Settings  { get; set; } = "_settings";

        /// <summary>
        /// Список задач ZennoPoster (JsonToDb из ZennoPoster.TasksList).
        /// TaskManager._tasksTable
        /// </summary>
        public static string Tasks     { get; set; } = "_tasks";

        /// <summary>
        /// Очередь команд для выполнения (status: pending → done/error).
        /// TaskManager._commandsTable
        /// </summary>
        public static string Commands  { get; set; } = "_commands";

        // ── DbExtencions ──────────────────────────────────────────────────────

        /// <summary>
        /// Таблица кошельков — используется в SqlGet при chainType-запросах.
        /// DbExtencions.cs:150 — хардкод "_wlt"
        /// </summary>
        public static string Wlt { get; set; } = "_wlt";
        
        public static string Instance  { get; set; } = "_instance";
    }
}