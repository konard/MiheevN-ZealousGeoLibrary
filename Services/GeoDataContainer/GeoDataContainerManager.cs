using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ZealousMindedPeopleGeo.Models;

namespace ZealousMindedPeopleGeo.Services.GeoDataContainer;

/// <summary>
/// Менеджер именованных контейнеров гео-данных.
/// Обеспечивает централизованное управление несколькими контейнерами данных.
/// </summary>
public class GeoDataContainerManager : IGeoDataContainerManager
{
    private readonly ConcurrentDictionary<string, IGeoDataContainer> _containers = new();
    private readonly ILogger<GeoDataContainerManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public event Action<string, GeoDataChangeType>? OnDataChanged;

    /// <summary>
    /// Создает новый менеджер контейнеров
    /// </summary>
    /// <param name="logger">Логгер</param>
    /// <param name="loggerFactory">Фабрика логгеров для создания логгеров контейнеров</param>
    public GeoDataContainerManager(
        ILogger<GeoDataContainerManager> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IGeoDataContainer GetOrCreateContainer(string containerId)
    {
        if (string.IsNullOrWhiteSpace(containerId))
        {
            throw new ArgumentNullException(nameof(containerId));
        }

        return _containers.GetOrAdd(containerId, id =>
        {
            _logger.LogInformation("Creating new geo-data container: {ContainerId}", id);
            var containerLogger = _loggerFactory.CreateLogger<InMemoryGeoDataContainer>();
            return new InMemoryGeoDataContainer(id, containerLogger, HandleDataChanged);
        });
    }

    /// <inheritdoc />
    public IGeoDataContainer? GetContainer(string containerId)
    {
        if (string.IsNullOrWhiteSpace(containerId))
        {
            return null;
        }

        _containers.TryGetValue(containerId, out var container);
        return container;
    }

    /// <inheritdoc />
    public bool ContainerExists(string containerId)
    {
        return !string.IsNullOrWhiteSpace(containerId) && _containers.ContainsKey(containerId);
    }

    /// <inheritdoc />
    public bool RemoveContainer(string containerId)
    {
        if (string.IsNullOrWhiteSpace(containerId))
        {
            return false;
        }

        if (_containers.TryRemove(containerId, out _))
        {
            _logger.LogInformation("Removed geo-data container: {ContainerId}", containerId);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetContainerIds()
    {
        return _containers.Keys.ToList();
    }

    /// <inheritdoc />
    public async ValueTask<GeoDataOperationResult> LoadDataAsync(string containerId, IEnumerable<Participant> participants, CancellationToken ct = default)
    {
        try
        {
            var container = GetOrCreateContainer(containerId);

            // Очищаем контейнер перед загрузкой новых данных
            await container.ClearAsync(ct);

            // Добавляем участников
            var result = await container.AddParticipantsAsync(participants, ct);

            _logger.LogInformation("Loaded {Count} participants into container '{ContainerId}'", result.ProcessedCount, containerId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data into container '{ContainerId}'", containerId);
            return GeoDataOperationResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public async ValueTask<GeoDataOperationResult> LoadFromJsonFileAsync(string containerId, string jsonFilePath, CancellationToken ct = default)
    {
        try
        {
            if (!File.Exists(jsonFilePath))
            {
                return GeoDataOperationResult.Fail($"File not found: {jsonFilePath}");
            }

            var jsonContent = await File.ReadAllTextAsync(jsonFilePath, ct);
            return await LoadFromJsonAsync(containerId, jsonContent, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data from JSON file '{FilePath}' into container '{ContainerId}'", jsonFilePath, containerId);
            return GeoDataOperationResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public async ValueTask<GeoDataOperationResult> LoadFromJsonAsync(string containerId, string jsonContent, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return GeoDataOperationResult.Fail("JSON content is empty");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var participants = JsonSerializer.Deserialize<List<Participant>>(jsonContent, options);

            if (participants == null)
            {
                return GeoDataOperationResult.Fail("Failed to deserialize JSON content");
            }

            return await LoadDataAsync(containerId, participants, ct);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing JSON content for container '{ContainerId}'", containerId);
            return GeoDataOperationResult.Fail($"JSON parsing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data from JSON into container '{ContainerId}'", containerId);
            return GeoDataOperationResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public async ValueTask<GeoDataOperationResult> SaveToJsonFileAsync(string containerId, string jsonFilePath, CancellationToken ct = default)
    {
        try
        {
            var jsonContent = await ExportToJsonAsync(containerId, ct);

            // Создаем директорию если не существует
            var directory = Path.GetDirectoryName(jsonFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(jsonFilePath, jsonContent, ct);

            _logger.LogInformation("Saved container '{ContainerId}' data to file '{FilePath}'", containerId, jsonFilePath);

            return GeoDataOperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving container '{ContainerId}' data to file '{FilePath}'", containerId, jsonFilePath);
            return GeoDataOperationResult.Fail(ex.Message);
        }
    }

    /// <inheritdoc />
    public async ValueTask<string> ExportToJsonAsync(string containerId, CancellationToken ct = default)
    {
        try
        {
            var container = GetContainer(containerId);
            if (container == null)
            {
                _logger.LogWarning("Container '{ContainerId}' not found for export", containerId);
                return "[]";
            }

            var participants = await container.GetAllParticipantsAsync(ct);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(participants, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting container '{ContainerId}' to JSON", containerId);
            return "[]";
        }
    }

    private void HandleDataChanged(string containerId, GeoDataChangeType changeType)
    {
        try
        {
            _logger.LogDebug("Data changed in container '{ContainerId}': {ChangeType}", containerId, changeType);
            OnDataChanged?.Invoke(containerId, changeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling data change event for container '{ContainerId}'", containerId);
        }
    }
}
