using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KiriathSolutions.GrandExchangeApi.Web.Endpoints;

public class AdminEndpoints : IEndpointDefinitions
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/hello", () => { return new { Message = "Hello world!"}; });
    }
}

