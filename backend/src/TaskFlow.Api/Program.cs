using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Application.WorkTasks.Commands.CreateWorkTask;
using TaskFlow.Application.Common.Behaviors;
using FluentValidation;
using TaskFlow.Api.ExceptionHandling;
using TaskFlow.Infrastructure.Messaging;
using MassTransit;
using TaskFlow.Api.Consumers;
using TaskFlow.Api.Hubs;
using TaskFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TaskFlowDb") ?? throw new InvalidOperationException("Connection string 'TaskFlowDbContext' not found.");
// Add services to the container.
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseNpgsql(connectionString, npsql => npsql.MigrationsHistoryTable(
        "__ef_migrations_history", "taskflow")));
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<IWorkTaskRepository, WorkTaskRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IWorkTaskEventPublisher, WorkTaskEventPublisher>();
builder.Services.AddScoped<ITaskNotificationRepository, TaskNotificationRepository>();
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<
        CreateWorkTaskCommand>();

    configuration.AddOpenBehavior(
        typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<
    CreateWorkTaskCommandValidator>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationCreatedConsumer>()
     .Endpoint(endpoint =>
    {
        endpoint.Name = "taskflow-live-notifications";
    });

    x.AddEntityFrameworkOutbox<TaskFlowDbContext>(outbox =>
    {
        outbox.UsePostgres();

        outbox.QueryDelay = TimeSpan.FromSeconds(1);

        outbox.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            "localhost",
            56783,
            "taskflow",
            host =>
            {
                host.Username("taskflow_user");
                host.Password("taskflow_rabbit_dev_password");
            });
        cfg.ConfigureEndpoints(context);
    });

});

builder.Services.AddSignalR();

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddSignInManager();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            IdentityConstants.ApplicationScheme;

        options.DefaultChallengeScheme =
            IdentityConstants.ApplicationScheme;

        options.DefaultSignInScheme =
            IdentityConstants.ApplicationScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "TaskFlow.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;

    options.Cookie.SecurePolicy =
        builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = false;

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode =
            StatusCodes.Status401Unauthorized;

        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode =
            StatusCodes.Status403Forbidden;

        return Task.CompletedTask;
    };
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";

    options.Cookie.Name = "TaskFlow.Csrf";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;

    options.Cookie.SecurePolicy =
        builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
});

var app = builder.Build();
app.UseExceptionHandler();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();
