using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class JogoCriadoConsumer : IConsumer<JogoCriadoEvent>
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly ILogger<JogoCriadoConsumer> _logger;

    public JogoCriadoConsumer(ElasticsearchClient elasticClient, ILogger<JogoCriadoConsumer> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<JogoCriadoEvent> context)
    {
        var jogo = context.Message;
        _logger.LogInformation("Processando indexação do jogo: {Id}", jogo.Id);

        // Mesma lógica que você usava na Azure Function
        var response = await _elasticClient.IndexAsync(
            jogo,
            i => i.Index("jogos-index").Id(jogo.Id)
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Falha ao indexar documento {Id}", jogo.Id);
            throw new Exception("Erro na indexação.");
        }

        _logger.LogInformation("Documento {Id} indexado com sucesso no Elasticsearch.", jogo.Id);
    }
}

public class JogoCriadoEvent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
    public double Price { get; set; }
    public int Genre { get; set; }
    public int Rating { get; set; }
}