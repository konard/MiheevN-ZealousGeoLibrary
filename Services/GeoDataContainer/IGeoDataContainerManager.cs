using ZealousMindedPeopleGeo.Models;

namespace ZealousMindedPeopleGeo.Services.GeoDataContainer;

/// <summary>
/// Интерфейс менеджера именованных контейнеров гео-данных.
/// Обеспечивает централизованное управление несколькими контейнерами данных.
/// </summary>
public interface IGeoDataContainerManager
{
    /// <summary>
    /// Создает или получает существующий контейнер по идентификатору
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <returns>Контейнер гео-данных</returns>
    IGeoDataContainer GetOrCreateContainer(string containerId);

    /// <summary>
    /// Получает контейнер по идентификатору
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <returns>Контейнер или null если не найден</returns>
    IGeoDataContainer? GetContainer(string containerId);

    /// <summary>
    /// Проверяет существование контейнера
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <returns>true если контейнер существует</returns>
    bool ContainerExists(string containerId);

    /// <summary>
    /// Удаляет контейнер
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <returns>true если контейнер был удален</returns>
    bool RemoveContainer(string containerId);

    /// <summary>
    /// Получает список всех идентификаторов контейнеров
    /// </summary>
    /// <returns>Коллекция идентификаторов</returns>
    IEnumerable<string> GetContainerIds();

    /// <summary>
    /// Загружает данные в контейнер из коллекции участников
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <param name="participants">Коллекция участников для загрузки</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> LoadDataAsync(string containerId, IEnumerable<Participant> participants, CancellationToken ct = default);

    /// <summary>
    /// Загружает данные в контейнер из JSON файла
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <param name="jsonFilePath">Путь к JSON файлу</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> LoadFromJsonFileAsync(string containerId, string jsonFilePath, CancellationToken ct = default);

    /// <summary>
    /// Загружает данные в контейнер из JSON строки
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <param name="jsonContent">JSON строка с данными</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> LoadFromJsonAsync(string containerId, string jsonContent, CancellationToken ct = default);

    /// <summary>
    /// Сохраняет данные контейнера в JSON файл
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <param name="jsonFilePath">Путь к JSON файлу</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> SaveToJsonFileAsync(string containerId, string jsonFilePath, CancellationToken ct = default);

    /// <summary>
    /// Экспортирует данные контейнера в JSON строку
    /// </summary>
    /// <param name="containerId">Идентификатор контейнера</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>JSON строка с данными</returns>
    ValueTask<string> ExportToJsonAsync(string containerId, CancellationToken ct = default);

    /// <summary>
    /// Событие, вызываемое при изменении данных в контейнере
    /// </summary>
    event Action<string, GeoDataChangeType>? OnDataChanged;
}

/// <summary>
/// Тип изменения данных в контейнере
/// </summary>
public enum GeoDataChangeType
{
    /// <summary>
    /// Добавление участника
    /// </summary>
    Added,

    /// <summary>
    /// Обновление участника
    /// </summary>
    Updated,

    /// <summary>
    /// Удаление участника
    /// </summary>
    Removed,

    /// <summary>
    /// Очистка контейнера
    /// </summary>
    Cleared,

    /// <summary>
    /// Массовая загрузка данных
    /// </summary>
    BulkLoaded
}
