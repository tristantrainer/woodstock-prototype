using KiriathSolutions.Woodstock.Web.Hubs;
using KiriathSolutions.Woodstock.Web.Types;
using KiriathSolutions.Woodstock.Web.Types.ObjectTypes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddWebServices();

builder.Services.AddCors();

builder.Services.AddSignalR();

builder.Services
    .AddGraphQLServer()
    .AddFiltering()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<MutationType>()
    .AddType<QueryType>()
    .AddType<CachedAccountType>()
    .AddType<CachedTransactionType>();

var app = builder.Build();

app.MapGraphQL();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler((options) => {});

app.UseHttpsRedirection();

app.MapHub<CacheSubscriptionHub>("/cache-updates");

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
    .AllowCredentials()); 

app.Run();
