using System.Text.Json.Serialization;
using Api.Features.Registrations;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Features.Registrations.CreateRegistration;
using Application.Features.Registrations.GetRegistrationById;
using Application.Features.Registrations.GetRegistrations;
using Application.Features.Registrations.GetRegistrationSummary;
using Domain.Common;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database: SQLite file (carshow.db) stored next to the Api project
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "carshow.db");
builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Validators
builder.Services.AddValidatorsFromAssembly(typeof(IAppDbContext).Assembly);

// Handlers
builder.Services.AddScoped<ICommandHandler<CreateRegistrationCommand, Result<CreateRegistrationResponse>>, CreateRegistrationCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetRegistrationsQuery, Result<GetRegistrationsResponse>>, GetRegistrationsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetRegistrationByIdQuery, Result<GetRegistrationByIdResponse>>, GetRegistrationByIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetRegistrationSummaryQuery, Result<GetRegistrationSummaryResponse>>, GetRegistrationSummaryQueryHandler>();

// Enums are written/read as strings in JSON ("Competitor", "Pending", ...)
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// CORS: allow the React dev server
builder.Services.AddCors(options =>
    options.AddPolicy("ReactDevServer", policy => policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

// API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Car Show Registration API";
        document.Info.Description = "Car Show 2027 registration system (Vertical Slice Architecture, .NET 10, SQLite)";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Apply migrations + seed data (only when the table is empty)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    DbSeeder.Seed(dbContext);
}

app.UseCors("ReactDevServer");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Root endpoint
app.MapGet("/", () => "Car Show Registration API. Go to /scalar/v1 for API documentation.");

// Map endpoints
app.MapRegistrationEndpoints();

app.Run();
