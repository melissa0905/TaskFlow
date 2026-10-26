using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskFlow.Application.Abstractions;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Worker;
using TaskFlow.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);
var connectionString =
    builder.Configuration.GetConnectionString("TaskFlowDb")
    ?? throw new InvalidOperationException(
        "TaskFlowDb bağlantı bilgisi bulunamadı.");
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<
    ITaskNotificationRepository,
    TaskNotificationRepository>();

builder.Services.AddHostedService<Worker>();

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<TaskFlowDbContext>(outbox =>
    {
        outbox.UsePostgres();
    });
    x.AddConsumer<WorkTaskAssignedConsumer>()
        .Endpoint(endpoint =>
        {
            endpoint.Name = "taskflow-notifications";
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
        cfg.ReceiveEndpoint(
            "taskflow-notifications",
            endpoint =>
            {

                endpoint.UseMessageRetry(retry =>
                {
                    retry.Handle<NpgsqlException>(
                        exception => exception.IsTransient);

                    retry.Intervals(
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5));
                });
                endpoint.UseEntityFrameworkOutbox<TaskFlowDbContext>(context);
                endpoint.ConfigureConsumer<
                    WorkTaskAssignedConsumer>(context);
            });

    });
});

var host = builder.Build();
host.Run();
