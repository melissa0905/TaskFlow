using MassTransit;
using TaskFlow.Worker;
using TaskFlow.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddMassTransit(x =>
{
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

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
