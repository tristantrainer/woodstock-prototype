using KiriathSolutions.Woodstock.Contracts.Interfaces;

namespace KiriathSolutions.Woodstock.Web.Settings;

public class HostedServiceOptions : IHostedServiceOptions
{
    public int Capacity => 10;
}