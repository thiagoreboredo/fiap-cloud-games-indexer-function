# FIAP Cloud Games - Indexer Function

Esta Azure Function faz parte de um projeto de microserviços e tem como responsabilidade indexar informações de jogos no Elasticsearch.

## Funcionalidade

A função é acionada por uma mensagem no **Azure Service Bus**.

1.  **Gatilho (Trigger):** A função é inscrita no tópico `jogo-atualizado-topic` através da subscription `elasticsearch-indexer-subscription`.
2.  **Ação:** Ao receber uma mensagem, ela desserializa o conteúdo para um objeto `JogoDocument` e o indexa (ou atualiza, caso já exista) no índice `jogos-index` do Elasticsearch.

## Tecnologias Utilizadas

* **Azure Functions** (.NET 8)
* **Azure Service Bus** (Trigger)
* **Elasticsearch** (Cliente .NET)
* **Azure DevOps** (CI/CD)

## Configuração

Para executar a função localmente ou no Azure, as seguintes configurações de ambiente são necessárias (no `local.settings.json` ou nas configurações do Function App):

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ElasticsearchUri": "URL_DO_SEU_CLUSTER_ELASTIC",
    "ElasticsearchApiKey": "API_KEY_DO_SEU_CLUSTER_ELASTIC",
    "ServiceBusConnection": "CONNECTION_STRING_DO_SEU_SERVICE_BUS"
  }
}
```

## Pipeline de CI/CD

O deploy deste projeto é automatizado através de um pipeline no Azure DevOps (`azure-pipelines.yml`). Qualquer commit na branch `main` irá disparar o processo de build e implantação automática no ambiente do Azure.