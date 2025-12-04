using ZealousMindedPeopleGeo.Models;

namespace ZealousMindedPeopleGeo.Interfaces;

/// <summary>
/// Менеджер для управления источниками данных
/// </summary>
public interface IDataSourceManager
{
    /// <summary>
    /// Получает все зарегистрированные источники данных
    /// </summary>
    /// <returns>Список источников данных</returns>
    IEnumerable<IDataSource> GetAllDataSources();

    /// <summary>
    /// Получает источник данных по имени
    /// </summary>
    /// <param name="name">Имя источника данных</param>
    /// <returns>Источник данных или null</returns>
    IDataSource? GetDataSource(string name);

    /// <summary>
    /// Получает источники данных по типу
    /// </summary>
    /// <param name="type">Тип источника данных</param>
    /// <returns>Список источников данных</returns>
    IEnumerable<IDataSource> GetDataSourcesByType(DataSourceType type);

    /// <summary>
    /// Получает источник данных с наивысшим приоритетом для чтения
    /// </summary>
    /// <returns>Источник данных для чтения или null</returns>
    IDataSource? GetPrimaryReadSource();

    /// <summary>
    /// Получает источник данных с наивысшим приоритетом для записи
    /// </summary>
    /// <returns>Источник данных для записи или null</returns>
    IDataSource? GetPrimaryWriteSource();

    /// <summary>
    /// Регистрирует новый источник данных
    /// </summary>
    /// <param name="dataSource">Источник данных</param>
    /// <returns>Успешность регистрации</returns>
    bool RegisterDataSource(IDataSource dataSource);

    /// <summary>
    /// Отменяет регистрацию источника данных
    /// </summary>
    /// <param name="name">Имя источника данных</param>
    /// <returns>Успешность отмены регистрации</returns>
    bool UnregisterDataSource(string name);

    /// <summary>
    /// Устанавливает приоритет источника данных
    /// </summary>
    /// <param name="name">Имя источника данных</param>
    /// <param name="priority">Новый приоритет</param>
    /// <returns>Успешность операции</returns>
    bool SetDataSourcePriority(string name, int priority);

    /// <summary>
    /// Получает конфигурацию источника данных
    /// </summary>
    /// <param name="name">Имя источника данных</param>
    /// <returns>Конфигурация или null</returns>
    IDataSourceConfiguration? GetDataSourceConfiguration(string name);

    /// <summary>
    /// Обновляет конфигурацию источника данных
    /// </summary>
    /// <param name="name">Имя источника данных</param>
    /// <param name="configuration">Новая конфигурация</param>
    /// <returns>Успешность обновления</returns>
    bool UpdateDataSourceConfiguration(string name, IDataSourceConfiguration configuration);

    /// <summary>
    /// Проверяет доступность всех источников данных
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Словарь доступности источников</returns>
    ValueTask<Dictionary<string, bool>> CheckAllDataSourcesAsync(CancellationToken ct = default);

    /// <summary>
    /// Получает агрегированную статистику по всем источникам данных
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Агрегированная статистика</returns>
    ValueTask<AggregatedDataSourceStatistics> GetAggregatedStatisticsAsync(CancellationToken ct = default);
}

/// <summary>
/// Менеджер источников данных с поддержкой делегирования и кеширования
/// </summary>
public interface IDataSourceDelegate : IParticipantDataSource
{
    /// <summary>
    /// Целевой источник данных для делегирования
    /// </summary>
    IDataSource TargetDataSource { get; }

    /// <summary>
    /// Добавляет промежуточный обработчик
    /// </summary>
    /// <param name="handler">Обработчик</param>
    void AddHandler(IDataSourceHandler handler);

    /// <summary>
    /// Удаляет промежуточный обработчик
    /// </summary>
    /// <param name="handler">Обработчик</param>
    void RemoveHandler(IDataSourceHandler handler);
}

/// <summary>
/// Промежуточный обработчик для источника данных
/// </summary>
public interface IDataSourceHandler
{
    /// <summary>
    /// Обрабатывает операцию чтения
    /// </summary>
    /// <param name="operation">Тип операции</param>
    /// <param name="context">Контекст операции</param>
    /// <param name="next">Следующий обработчик в цепочке</param>
    /// <returns>Результат операции</returns>
    Task<object?> HandleReadAsync(OperationType operation, object context, Func<Task<object?>> next);

    /// <summary>
    /// Обрабатывает операцию записи
    /// </summary>
    /// <param name="operation">Тип операции</param>
    /// <param name="context">Контекст операции</param>
    /// <param name="next">Следующий обработчик в цепочке</param>
    /// <returns>Результат операции</returns>
    Task<object?> HandleWriteAsync(OperationType operation, object context, Func<Task<object?>> next);
}

/// <summary>
/// Менеджер композиции источников данных
/// </summary>
public interface IDataSourceCompositionManager
{
    /// <summary>
    /// Создает композитный источник данных из нескольких источников
    /// </summary>
    /// <param name="sources">Источники данных для композиции</param>
    /// <param name="strategy">Стратегия композиции</param>
    /// <returns>Композитный источник данных</returns>
    IDataSource CreateCompositeSource(IEnumerable<IDataSource> sources, CompositionStrategy strategy);

    /// <summary>
    /// Создает кешированный источник данных
    /// </summary>
    /// <param name="source">Базовый источник данных</param>
    /// <param name="cacheConfig">Конфигурация кеша</param>
    /// <returns>Кешированный источник данных</returns>
    ICachedDataSource CreateCachedSource(IDataSource source, CacheConfiguration cacheConfig);

    /// <summary>
    /// Создает источник данных с резервным копированием
    /// </summary>
    /// <param name="primary">Основной источник данных</param>
    /// <param name="backup">Резервный источник данных</param>
    /// <returns>Источник данных с резервным копированием</returns>
    IBackupDataSource CreateBackupSource(IDataSource primary, IDataSource backup);

    /// <summary>
    /// Создает синхронизированный источник данных
    /// </summary>
    /// <param name="local">Локальный источник данных</param>
    /// <param name="remote">Удаленный источник данных</param>
    /// <param name="syncConfig">Конфигурация синхронизации</param>
    /// <returns>Синхронизированный источник данных</returns>
    ISyncDataSource CreateSyncSource(IDataSource local, IDataSource remote, SyncConfiguration syncConfig);

    /// <summary>
    /// Получает доступные стратегии композиции
    /// </summary>
    /// <returns>Список стратегий композиции</returns>
    IEnumerable<CompositionStrategy> GetAvailableCompositionStrategies();
}

/// <summary>
/// Типы операций для обработчиков
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Добавление участника
    /// </summary>
    AddParticipant,

    /// <summary>
    /// Получение всех участников
    /// </summary>
    GetAllParticipants,

    /// <summary>
    /// Получение участника по ID
    /// </summary>
    GetParticipantById,

    /// <summary>
    /// Обновление участника
    /// </summary>
    UpdateParticipant,

    /// <summary>
    /// Удаление участника
    /// </summary>
    DeleteParticipant,

    /// <summary>
    /// Поиск участников
    /// </summary>
    FindParticipants,

    /// <summary>
    /// Проверка доступности
    /// </summary>
    IsAvailable
}

/// <summary>
/// Стратегии композиции источников данных
/// </summary>
public enum CompositionStrategy
{
    /// <summary>
    /// Объединение результатов из всех источников (Union)
    /// </summary>
    Union,

    /// <summary>
    /// Пересечение результатов из всех источников (Intersection)
    /// </summary>
    Intersection,

    /// <summary>
    /// Использование первого доступного источника (FirstAvailable)
    /// </summary>
    FirstAvailable,

    /// <summary>
    /// Использование источника с наивысшим приоритетом (PriorityBased)
    /// </summary>
    PriorityBased,

    /// <summary>
    /// Распределение нагрузки между источниками (LoadBalanced)
    /// </summary>
    LoadBalanced,

    /// <summary>
    /// Репликация данных во все источники (Replicated)
    /// </summary>
    Replicated,

    /// <summary>
    /// Выборочная синхронизация (SelectiveSync)
    /// </summary>
    SelectiveSync
}

/// <summary>
/// Конфигурация кеша
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// Время жизни кеша в минутах
    /// </summary>
    public int TimeToLiveMinutes { get; set; } = 30;

    /// <summary>
    /// Максимальный размер кеша в записях
    /// </summary>
    public int MaxSize { get; set; } = 1000;

    /// <summary>
    /// Включить автоматическую очистку устаревших записей
    /// </summary>
    public bool AutoCleanup { get; set; } = true;

    /// <summary>
    /// Включить статистику кеша
    /// </summary>
    public bool EnableStatistics { get; set; } = true;
}

/// <summary>
/// Конфигурация синхронизации
/// </summary>
public class SyncConfiguration
{
    /// <summary>
    /// Интервал синхронизации в минутах
    /// </summary>
    public int SyncIntervalMinutes { get; set; } = 60;

    /// <summary>
    /// Таймаут операции синхронизации в секундах
    /// </summary>
    public int SyncTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Максимальное количество попыток синхронизации
    /// </summary>
    public int MaxSyncRetries { get; set; } = 3;

    /// <summary>
    /// Включить автоматическую синхронизацию
    /// </summary>
    public bool AutoSync { get; set; } = true;

    /// <summary>
    /// Стратегия разрешения конфликтов
    /// </summary>
    public ConflictResolutionStrategy ConflictResolution { get; set; } = ConflictResolutionStrategy.LastWriteWins;
}

/// <summary>
/// Стратегии разрешения конфликтов
/// </summary>
public enum ConflictResolutionStrategy
{
    /// <summary>
    /// Последняя запись побеждает
    /// </summary>
    LastWriteWins,

    /// <summary>
    /// Локальные изменения побеждают
    /// </summary>
    LocalWins,

    /// <summary>
    /// Удаленные изменения побеждают
    /// </summary>
    RemoteWins,

    /// <summary>
    /// Ручное разрешение конфликтов
    /// </summary>
    Manual
}

/// <summary>
/// Агрегированная статистика источников данных
/// </summary>
public class AggregatedDataSourceStatistics
{
    /// <summary>
    /// Общее количество записей во всех источниках
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Количество доступных источников данных
    /// </summary>
    public int AvailableSources { get; set; }

    /// <summary>
    /// Общий размер данных в байтах
    /// </summary>
    public long TotalDataSizeBytes { get; set; }

    /// <summary>
    /// Статистика по каждому источнику данных
    /// </summary>
    public Dictionary<string, DataSourceStatistics> SourceStatistics { get; set; } = new();

    /// <summary>
    /// Время последней агрегированной операции
    /// </summary>
    public DateTime LastAggregated { get; set; }

    /// <summary>
    /// Среднее время ответа источников данных
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Процент доступности источников данных
    /// </summary>
    public double AvailabilityPercentage => SourceStatistics.Count > 0 
        ? (double)AvailableSources / SourceStatistics.Count * 100 
        : 0;
}

/// <summary>
/// Фабрика для создания источников данных
/// </summary>
public interface IDataSourceFactory
{
    /// <summary>
    /// Создает источник данных указанного типа
    /// </summary>
    /// <param name="type">Тип источника данных</param>
    /// <param name="configuration">Конфигурация источника данных</param>
    /// <returns>Созданный источник данных</returns>
    IDataSource CreateDataSource(DataSourceType type, IDataSourceConfiguration configuration);

    /// <summary>
    /// Создает источник данных из JSON конфигурации
    /// </summary>
    /// <param name="jsonConfig">JSON конфигурация</param>
    /// <returns>Созданный источник данных</returns>
    IDataSource CreateDataSourceFromJson(string jsonConfig);

    /// <summary>
    /// Получает доступные типы источников данных
    /// </summary>
    /// <returns>Список доступных типов</returns>
    IEnumerable<DataSourceType> GetSupportedDataSourceTypes();

    /// <summary>
    /// Проверяет, поддерживается ли указанный тип источника данных
    /// </summary>
    /// <param name="type">Тип источника данных</param>
    /// <returns>Поддерживается ли тип</returns>
    bool SupportsDataSourceType(DataSourceType type);
}

/// <summary>
/// Результат проверки совместимости
/// </summary>
public class CompatibilityResult
{
    public bool Compatible { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public string? Message { get; set; }
}

/// <summary>
/// Контекст операции источника данных
/// </summary>
public class DataSourceOperationContext
{
    /// <summary>
    /// Идентификатор операции
    /// </summary>
    public string OperationId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Тип операции
    /// </summary>
    public OperationType OperationType { get; set; }

    /// <summary>
    /// Имя источника данных
    /// </summary>
    public string DataSourceName { get; set; } = "";

    /// <summary>
    /// Время начала операции
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дополнительные данные операции
    /// </summary>
    public Dictionary<string, object> OperationData { get; set; } = new();

    /// <summary>
    /// Токен отмены операции
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Пользователь, инициировавший операцию
    /// </summary>
    public string? UserId { get; set; }
}