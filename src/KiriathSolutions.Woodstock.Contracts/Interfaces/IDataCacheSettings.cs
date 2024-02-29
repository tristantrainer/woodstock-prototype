namespace KiriathSolutions.Woodstock.Contracts.Interfaces;

public interface IDataCacheSettings
{
    string DirectoryPath { get; }
    int LifespanInMinutes { get; }
}

public interface IQueryCacheSettings
{
    string DirectoryPath { get; }
    int LifespanInMinutes { get; }
}