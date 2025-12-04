namespace ZealousMindedPeopleGeo.Interfaces;

/// <summary>
/// Аргументы события источника данных
/// </summary>
public class DataSourceEventArgs : EventArgs
{
    public string Name { get; }
    public IDataSource DataSource { get; }

    public DataSourceEventArgs(string name, IDataSource dataSource)
    {
        Name = name;
        DataSource = dataSource;
    }
}

/// <summary>
/// Аргументы изменения стратегии композиции
/// </summary>
public class CompositionStrategyChangedEventArgs : EventArgs
{
    public DataSourceCompositionStrategy OldStrategy { get; }
    public DataSourceCompositionStrategy NewStrategy { get; }

    public CompositionStrategyChangedEventArgs(DataSourceCompositionStrategy oldStrategy, DataSourceCompositionStrategy newStrategy)
    {
        OldStrategy = oldStrategy;
        NewStrategy = newStrategy;
    }
}

/// <summary>
/// Тип менеджера источников данных
/// </summary>
public enum DataSourceManagerType
{
    Participant,
    General,
    GeoData
}

/// <summary>
/// Статус менеджера источников данных
/// </summary>
public enum DataSourceManagerStatus
{
    Operational,
    Degraded,
    Error,
    Unknown
}

/// <summary>
/// Стратегия композиции источников данных
/// </summary>
public enum DataSourceCompositionStrategy
{
    UsePrimaryOnly,
    UseAllAndMerge,
    UseFirstAvailable,
    UsePriorityBased
}

/// <summary>
/// Операция источника данных
/// </summary>
public enum DataSourceOperation
{
    Read,
    Create,
    Update,
    Delete,
    ReadWrite
}