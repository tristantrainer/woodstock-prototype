using KiriathSolutions.Woodstock.Contracts.Interfaces;

namespace KiriathSolutions.Woodstock.Web.Settings;

public class DataCacheSettings : IDataCacheSettings
{
    public string DirectoryPath { get; set; } = @"C:\Users\trist\Projects\Peanuts\woodstock\caches\data";
    public int LifespanInMinutes { get; set; } = 6000;
}

public class QueryCacheSettings : IQueryCacheSettings
{
    public string DirectoryPath { get; set; } = @"C:\Users\trist\Projects\Peanuts\woodstock\caches\query";
    public int LifespanInMinutes { get; set; } = 6000;
}