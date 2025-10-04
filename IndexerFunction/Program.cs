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

        // --- Configura��o do Cliente Elasticsearch (cliente moderno) ---
        var uri = Environment.GetEnvironmentVariable("ElasticsearchUri")
                 ?? throw new InvalidOperationException("Vari�vel de ambiente 'ElasticsearchUri' n�o definida.");
        var apiKey = Environment.GetEnvironmentVariable("ElasticsearchApiKey")
                   ?? throw new InvalidOperationException("Vari�vel de ambiente 'ElasticsearchApiKey' n�o definida.");

        var settings = new ElasticsearchClientSettings(new Uri(uri))
            .Authentication(new ApiKey(apiKey))
            .DefaultIndex("jogos-index");

        services.AddSingleton(new ElasticsearchClient(settings));
        // --- Fim da Configura��o ---
    })
    .Build();

host.Run();