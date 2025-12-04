using ZealousMindedPeopleGeo.Models;

namespace ZealousMindedPeopleGeo.Interfaces;

/// <summary>
/// Базовый интерфейс для источника данных
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// Имя источника данных
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Описание источника данных
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Тип источника данных
    /// </summary>
    DataSourceType Type { get; }

    /// <summary>
    /// Приоритет источника (меньшее число = высший приоритет)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Проверяет доступность источника данных
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Доступность источника</returns>
    ValueTask<bool> IsAvailableAsync(CancellationToken ct = default);

    /// <summary>
    /// Проверяет, поддерживает ли источник чтение данных
    /// </summary>
    bool SupportsReading { get; }

    /// <summary>
    /// Проверяет, поддерживает ли источник запись данных
    /// </summary>
    bool SupportsWriting { get; }

    /// <summary>
    /// Проверяет, поддерживает ли источник транзакции
    /// </summary>
    bool SupportsTransactions { get; }

    /// <summary>
    /// Возвращает конфигурацию источника данных
    /// </summary>
    IDataSourceConfiguration Configuration { get; }

    /// <summary>
    /// Получает статистику источника данных
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Статистика источника</returns>
    ValueTask<DataSourceStatistics> GetStatisticsAsync(CancellationToken ct = default);
}

/// <summary>
/// Интерфейс для источника данных, поддерживающего участников
/// </summary>
public interface IParticipantDataSource : IDataSource
{
    /// <summary>
    /// Добавляет участника в источник данных
    /// </summary>
    /// <param name="participant">Данные участника</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    ValueTask<RepositoryResult> AddParticipantAsync(Participant participant, CancellationToken ct = default);

    /// <summary>
    /// Получает всех участников из источника данных
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Список участников</returns>
    ValueTask<IEnumerable<Participant>> GetAllParticipantsAsync(CancellationToken ct = default);

    /// <summary>
    /// Получает участника по ID
    /// </summary>
    /// <param name="id">ID участника</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Участник или null если не найден</returns>
    ValueTask<Participant?> GetParticipantByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Обновляет данные участника
    /// </summary>
    /// <param name="participant">Обновленные данные участника</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    ValueTask<RepositoryResult> UpdateParticipantAsync(Participant participant, CancellationToken ct = default);

    /// <summary>
    /// Удаляет участника
    /// </summary>
    /// <param name="id">ID участника для удаления</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    ValueTask<RepositoryResult> DeleteParticipantAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Находит участников по критериям
    /// </summary>
    /// <param name="predicate">Условие поиска</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Найденные участники</returns>
    ValueTask<IEnumerable<Participant>> FindParticipantsAsync(Func<Participant, bool> predicate, CancellationToken ct = default);

    /// <summary>
    /// Получает количество участников в источнике
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Количество участников</returns>
    ValueTask<int> GetParticipantCountAsync(CancellationToken ct = default);
}

/// <summary>
/// Интерфейс для источника данных, поддерживающего GeoJSON
/// </summary>
public interface IGeoJsonDataSource : IDataSource
{
    /// <summary>
    /// Импортирует участников из GeoJSON
    /// </summary>
    /// <param name="geoJson">Данные в формате GeoJSON</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат импорта</returns>
    ValueTask<ImportResult> ImportFromGeoJsonAsync(string geoJson, CancellationToken ct = default);

    /// <summary>
    /// Экспортирует участников в GeoJSON
    /// </summary>
    /// <param name="participants">Участники для экспорта</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Данные в формате GeoJSON</returns>
    ValueTask<string> ExportToGeoJsonAsync(IEnumerable<Participant> participants, CancellationToken ct = default);

    /// <summary>
    /// Загружает данные из файла GeoJSON
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат загрузки</returns>
    ValueTask<LoadResult> LoadFromFileAsync(string filePath, CancellationToken ct = default);

    /// <summary>
    /// Сохраняет данные в файл GeoJSON
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <param name="participants">Участники для сохранения</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат сохранения</returns>
    ValueTask<SaveResult> SaveToFileAsync(string filePath, IEnumerable<Participant> participants, CancellationToken ct = default);
}

/// <summary>
/// Интерфейс для источника данных с поддержкой кеширования
/// </summary>
public interface ICachedDataSource : IParticipantDataSource
{
    /// <summary>
    /// Очищает кеш
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат очистки</returns>
    ValueTask ClearCacheAsync(CancellationToken ct = default);

    /// <summary>
    /// Принудительно обновляет кеш
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат обновления</returns>
    ValueTask RefreshCacheAsync(CancellationToken ct = default);

    /// <summary>
    /// Получает статистику кеша
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Статистика кеша</returns>
    ValueTask<CacheStatistics> GetCacheStatisticsAsync(CancellationToken ct = default);
}

/// <summary>
/// Интерфейс для источника данных с поддержкой резервного копирования
/// </summary>
public interface IBackupDataSource : IParticipantDataSource
{
    /// <summary>
    /// Создает резервную копию данных
    /// </summary>
    /// <param name="backupPath">Путь для сохранения резервной копии</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат создания резервной копии</returns>
    ValueTask<BackupResult> CreateBackupAsync(string backupPath, CancellationToken ct = default);

    /// <summary>
    /// Восстанавливает данные из резервной копии
    /// </summary>
    /// <param name="backupPath">Путь к файлу резервной копии</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат восстановления</returns>
    ValueTask<RestoreResult> RestoreFromBackupAsync(string backupPath, CancellationToken ct = default);

    /// <summary>
    /// Получает список доступных резервных копий
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Список резервных копий</returns>
    ValueTask<IEnumerable<BackupInfo>> GetAvailableBackupsAsync(CancellationToken ct = default);
}

/// <summary>
/// Интерфейс для источника данных с поддержкой синхронизации
/// </summary>
public interface ISyncDataSource : IParticipantDataSource
{
    /// <summary>
    /// Синхронизирует данные с удаленным источником
    /// </summary>
    /// <param name="target">Целевой источник для синхронизации</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат синхронизации</returns>
    ValueTask<SyncResult> SyncWithAsync(IDataSource target, CancellationToken ct = default);

    /// <summary>
    /// Получает последнее время синхронизации
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Время последней синхронизации</returns>
    ValueTask<DateTime?> GetLastSyncTimeAsync(CancellationToken ct = default);

    /// <summary>
    /// Устанавливает время последней синхронизации
    /// </summary>
    /// <param name="syncTime">Время синхронизации</param>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    ValueTask SetLastSyncTimeAsync(DateTime syncTime, CancellationToken ct = default);
}

/// <summary>
/// Типы источников данных
/// </summary>
public enum DataSourceType
{
    /// <summary>
    /// Память (In-Memory)
    /// </summary>
    Memory,

    /// <summary>
    /// JSON файл
    /// </summary>
    JsonFile,

    /// <summary>
    /// CSV файл
    /// </summary>
    CsvFile,

    /// <summary>
    /// База данных
    /// </summary>
    Database,

    /// <summary>
    /// Google Sheets
    /// </summary>
    GoogleSheets,

    /// <summary>
    /// Внешний API
    /// </summary>
    Api,

    /// <summary>
    /// Кешированный источник
    /// </summary>
    Cached,

    /// <summary>
    /// Резервный источник
    /// </summary>
    Backup
}

/// <summary>
/// Конфигурация источника данных
/// </summary>
public interface IDataSourceConfiguration
{
    /// <summary>
    /// Строка подключения
    /// </summary>
    string? ConnectionString { get; }

    /// <summary>
    /// Настройки источника данных
    /// </summary>
    Dictionary<string, object> Settings { get; }

    /// <summary>
    /// Таймаут операций в секундах
    /// </summary>
    int TimeoutSeconds { get; set; }

    /// <summary>
    /// Максимальное количество попыток повтора
    /// </summary>
    int MaxRetries { get; set; }

    /// <summary>
    /// Интервал между попытками в миллисекундах
    /// </summary>
    int RetryDelayMs { get; set; }
}

/// <summary>
/// Статистика источника данных
/// </summary>
public class DataSourceStatistics
{
    /// <summary>
    /// Количество записей
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// Размер данных в байтах
    /// </summary>
    public long DataSizeBytes { get; set; }

    /// <summary>
    /// Время последнего доступа
    /// </summary>
    public DateTime? LastAccessed { get; set; }

    /// <summary>
    /// Время последнего обновления
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Состояние источника данных
    /// </summary>
    public DataSourceStatus Status { get; set; }

    /// <summary>
    /// Сообщение о состоянии
    /// </summary>
    public string? StatusMessage { get; set; }
}

/// <summary>
/// Состояние источника данных
/// </summary>
public enum DataSourceStatus
{
    /// <summary>
    /// Доступен
    /// </summary>
    Available,

    /// <summary>
    /// Недоступен
    /// </summary>
    Unavailable,

    /// <summary>
    /// Ошибка
    /// </summary>
    Error,

    /// <summary>
    /// Инициализируется
    /// </summary>
    Initializing,

    /// <summary>
    /// Обновляется
    /// </summary>
    Updating,

    /// <summary>
    /// Синхронизируется
    /// </summary>
    Syncing
}

/// <summary>
/// Результат импорта данных
/// </summary>
public class ImportResult
{
    public bool Success { get; set; }
    public int ImportedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Результат загрузки данных
/// </summary>
public class LoadResult
{
    public bool Success { get; set; }
    public int LoadedCount { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Результат сохранения данных
/// </summary>
public class SaveResult
{
    public bool Success { get; set; }
    public int SavedCount { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Результат создания резервной копии
/// </summary>
public class BackupResult
{
    public bool Success { get; set; }
    public string? BackupPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Результат восстановления из резервной копии
/// </summary>
public class RestoreResult
{
    public bool Success { get; set; }
    public int RestoredCount { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Информация о резервной копии
/// </summary>
public class BackupInfo
{
    public string Path { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public long SizeBytes { get; set; }
    public string Description { get; set; } = "";
}

/// <summary>
/// Результат синхронизации
/// </summary>
public class SyncResult
{
    public bool Success { get; set; }
    public int SyncedCount { get; set; }
    public int AddedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int DeletedCount { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Статистика кеша
/// </summary>
public class CacheStatistics
{
    public int HitCount { get; set; }
    public int MissCount { get; set; }
    public int CurrentSize { get; set; }
    public int MaxSize { get; set; }
    public double HitRatio => HitCount + MissCount > 0 ? (double)HitCount / (HitCount + MissCount) : 0;
}