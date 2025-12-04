using ZealousMindedPeopleGeo.Models;

namespace ZealousMindedPeopleGeo.Interfaces;

/// <summary>
/// Упрощенный сервис геокодирования
/// </summary>
public interface IGeocodingServiceSimple
{
    string Name { get; }
    string Description { get; }

    /// <summary>
    /// Геокодирует адрес в координаты
    /// </summary>
    ValueTask<GeocodingResultSimple> GeocodeAsync(string address, CancellationToken ct = default);

    /// <summary>
    /// Обратное геокодирование - координаты в адрес
    /// </summary>
    ValueTask<GeocodingResultSimple> ReverseGeocodeAsync(GeoJsonCoordinates coordinates, CancellationToken ct = default);

    /// <summary>
    /// Пакетное геокодирование
    /// </summary>
    ValueTask<IEnumerable<GeocodingResultSimple>> GeocodeBatchAsync(IEnumerable<string> addresses, int maxConcurrency = 5, CancellationToken ct = default);

    /// <summary>
    /// Валидирует адрес перед геокодированием
    /// </summary>
    bool ValidateAddress(string address);

    /// <summary>
    /// Проверяет доступность сервиса
    /// </summary>
    ValueTask<bool> IsAvailableAsync(CancellationToken ct = default);
}

/// <summary>
/// Упрощенный результат геокодирования
/// </summary>
public class GeocodingResultSimple
{
    public bool IsSuccess { get; set; }
    public string Address { get; set; } = "";
    public GeoJsonCoordinates? Coordinates { get; set; }
    public string FormattedAddress { get; set; } = "";
    public string Provider { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
}