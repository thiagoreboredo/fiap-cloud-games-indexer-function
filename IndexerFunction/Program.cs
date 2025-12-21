using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Application.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

#region Elasticsearch
// Configuração do Cliente Elasticsearch
var uri = builder.Configuration["ElasticsearchUri"]
          ?? throw new InvalidOperationException("ElasticsearchUri não definida.");
var apiKey = builder.Configuration["ElasticsearchApiKey"]
             ?? throw new InvalidOperationException("ElasticsearchApiKey não definida.");

var settings = new ElasticsearchClientSettings(new Uri(uri))
    .Authentication(new ApiKey(apiKey))
    .DefaultIndex("jogos-index");

builder.Services.AddSingleton(new ElasticsearchClient(settings));
#endregion

#region MassTransit com RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<JogoCriadoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // No K8s, o host será "rabbitmq-service". Localmente pode ser "localhost".
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq-service";

        cfg.Host(rabbitHost, "/", h => {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ReceiveEndpoint("jogos-indexacao-queue", e =>
        {
            e.ConfigureConsumer<JogoCriadoConsumer>(context);
        });
    });
});
#endregion

var host = builder.Build();
host.Run();