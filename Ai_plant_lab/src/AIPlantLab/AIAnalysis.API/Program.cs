using AIAnalysis.Application.Commands.AnalyzePhoto;
using AIAnalysis.Application.Interfaces;
using AIAnalysis.Infrastructure.AI;
using AIAnalysis.Infrastructure.Persistence;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(AnalyzePhotoCommand).Assembly));

builder.Services.AddSingleton(_ =>
{
    var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]
                   ?? throw new InvalidOperationException("Azure OpenAI Endpoint is missing.");
    var apiKey = builder.Configuration["AzureOpenAI:ApiKey"]
                 ?? throw new InvalidOperationException("Azure OpenAI ApiKey is missing.");
                 
    return new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
});

builder.Services.AddScoped<IAiVisionService, AzureOpenAiVisionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await app.RunAsync();