using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TaskFlowDb") ?? throw new InvalidOperationException("Connection string 'TaskFlowDbContext' not found.");
// Add services to the container.
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseNpgsql(connectionString,npsql => npsql.MigrationsHistoryTable(
        "__ef_migrations_history", "taskflow")));
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<IWorkTaskRepository, WorkTaskRepository>();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblyContaining<CreateProjectCommand>());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 499;
        }
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();
