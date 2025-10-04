using Azure.Messaging.ServiceBus;
using Elastic.Clients.Elasticsearch;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IndexerFunction
{
    public class IndexerFunction
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly ILogger<IndexerFunction> _logger;
        private readonly ElasticsearchClient _elasticClient;

        public IndexerFunction(ILogger<IndexerFunction> logger, ElasticsearchClient elasticClient)
        {
            _logger = logger;
            _elasticClient = elasticClient;
        }

        [Function(nameof(IndexerFunction))]
        public async Task Run(
            [ServiceBusTrigger("jogo-atualizado-topic", "elasticsearch-indexer-subscription", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Mensagem recebida: {MessageId}", message.MessageId);

            try
            {
                // Desserialização robusta a partir do BinaryData
                var jogo = message.Body.ToObjectFromJson<JogoDocument>(JsonOptions);

                if (jogo is null)
                {
                    _logger.LogError("Não foi possível deserializar a mensagem.");
                    return;
                }

                // Indexa com Id explícito para evitar duplicidade (update se existir)
                var response = await _elasticClient.IndexAsync(
                    jogo,
                    i => i.Index("jogos-index").Id(jogo.Id)
                );

                if (!response.IsValidResponse)
                {
                    var reason = response.ElasticsearchServerError?.Error?.Reason ?? "erro desconhecido";
                    _logger.LogError("Falha ao indexar documento {Id}: {Reason}", jogo.Id, reason);
                    throw new Exception("Falha na indexação: " + reason);
                }

                _logger.LogInformation("Documento {Id} indexado/atualizado com sucesso. Resultado: {Result}", jogo.Id, response.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado ao processar a mensagem {MessageId}.", message.MessageId);
                throw; // permite retry/DLQ
            }
        }
    }

    public class JogoDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Company { get; set; } = default!;
        public double Price { get; set; }
        public string Genre { get; set; } = default!;
        public string Rating { get; set; } = default!;
    }
}