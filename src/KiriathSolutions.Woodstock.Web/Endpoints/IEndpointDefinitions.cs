namespace KiriathSolutions.GrandExchangeApi.Web.Endpoints;

public interface IEndpointDefinitions
{
    void DefineEndpoints(WebApplication app);
    void DefineServices(IServiceCollection services) { }
}