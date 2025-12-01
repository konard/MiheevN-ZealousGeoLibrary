using ZealousMindedPeopleGeo.Models;

namespace ZealousMindedPeopleGeo.Services.GeoDataContainer;

/// <summary>
/// Интерфейс именованного контейнера гео-данных.
/// Позволяет организовать хранение данных участников в отдельных именованных контейнерах,
/// что упрощает работу с несколькими глобусами и разными наборами данных.
/// </summary>
public interface IGeoDataContainer
{
    /// <summary>
    /// Уникальный идентификатор контейнера
    /// </summary>
    string ContainerId { get; }

    /// <summary>
    /// Добавляет участника в контейнер
    /// </summary>
    /// <param name="participant">Данные участника</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> AddParticipantAsync(Participant participant, CancellationToken ct = default);

    /// <summary>
    /// Добавляет нескольких участников в контейнер
    /// </summary>
    /// <param name="participants">Коллекция участников</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> AddParticipantsAsync(IEnumerable<Participant> participants, CancellationToken ct = default);

    /// <summary>
    /// Получает всех участников из контейнера
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Коллекция участников</returns>
    ValueTask<IEnumerable<Participant>> GetAllParticipantsAsync(CancellationToken ct = default);

    /// <summary>
    /// Получает участника по ID
    /// </summary>
    /// <param name="id">ID участника</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Участник или null</returns>
    ValueTask<Participant?> GetParticipantByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Обновляет данные участника
    /// </summary>
    /// <param name="participant">Обновленные данные</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> UpdateParticipantAsync(Participant participant, CancellationToken ct = default);

    /// <summary>
    /// Удаляет участника из контейнера
    /// </summary>
    /// <param name="id">ID участника</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> RemoveParticipantAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Очищает все данные в контейнере
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    ValueTask<GeoDataOperationResult> ClearAsync(CancellationToken ct = default);

    /// <summary>
    /// Получает количество участников в контейнере
    /// </summary>
    int Count { get; }
}

/// <summary>
/// Результат операции с гео-данными
/// </summary>
public class GeoDataOperationResult
{
    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке (если есть)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Количество обработанных записей
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// ID записи (для операций с одной записью)
    /// </summary>
    public Guid? RecordId { get; set; }

    /// <summary>
    /// Создает успешный результат
    /// </summary>
    public static GeoDataOperationResult Ok(int processedCount = 1, Guid? recordId = null) => new()
    {
        Success = true,
        ProcessedCount = processedCount,
        RecordId = recordId
    };

    /// <summary>
    /// Создает результат с ошибкой
    /// </summary>
    public static GeoDataOperationResult Fail(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };
}
