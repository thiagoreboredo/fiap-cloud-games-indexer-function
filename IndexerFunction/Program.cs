using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Application Insights para Functions Isolated
        services.ConfigureFunctionsApplicationInsights();

        // --- Configuração do Cliente Elasticsearch (cliente moderno) ---
        var uri = Environment.GetEnvironmentVariable("ElasticsearchUri")
                 ?? throw new InvalidOperationException("Variável de ambiente 'ElasticsearchUri' não definida.");
        var apiKey = Environment.GetEnvironmentVariable("ElasticsearchApiKey")
                   ?? throw new InvalidOperationException("Variável de ambiente 'ElasticsearchApiKey' não definida.");

        var settings = new ElasticsearchClientSettings(new Uri(uri))
            .Authentication(new ApiKey(apiKey))
            .DefaultIndex("jogos-index");

        services.AddSingleton(new ElasticsearchClient(settings));
        // --- Fim da Configuração ---
    })
    .Build();

host.Run();