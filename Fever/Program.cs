using Fever.Infraestructure.Postgres;
using Fever.Infraestructure.Postgres.Extensions;
using Fever.Infraestructure.Shared.Extensions;
using Fever.Presentation.Features.Events.Endpoints;
using Fever.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<HandleExceptionMiddleware>();

app.AddEventsEndpoints();

app.ApplyMigrations<ApplicationDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
