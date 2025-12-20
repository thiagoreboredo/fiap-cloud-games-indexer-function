# FIAP Cloud Games - Indexer Worker ⚙️

Este componente é o motor de processamento assíncrono da plataforma **FIAP Cloud Games (FCG)**. Sua função principal é consumir eventos de mudança de estado provenientes do catálogo de jogos e garantir a sincronização em tempo real com o **Elasticsearch**.

Na **Fase 4**, este projeto passou por uma reengenharia completa, deixando de ser uma Azure Function para se tornar um **Worker Service** nativo, otimizado para orquestração em **Kubernetes (AKS)**.

## 🚀 Evoluções Técnicas (Fase 4)

A modernização deste serviço incluiu requisitos fundamentais de arquitetura orientada a eventos e escalabilidade:

- **Mudança de Paradigma**: Transição de *Serverless* (Azure Function) para *Worker Service* (.NET Generic Host), permitindo maior controle sobre o ciclo de vida do processo e consumo de recursos no Kubernetes.
- **Mensageria com RabbitMQ**: Implementação de consumo assíncrono utilizando **MassTransit** integrado ao **RabbitMQ**, substituindo o Azure Service Bus para uma arquitetura multi-cloud e on-premises.
- **Docker de Alta Performance**: Uso da imagem base `aspnet:8.0-alpine`, resultando em um dos menores artefatos do ecossistema, ideal para escalonamento rápido (Rapid Scaling).
- **Segurança de Execução**: Configuração de privilégios mínimos com usuário não-root (`USER $APP_UID`), protegendo o ambiente contra vulnerabilidades de runtime.
- **Pronto para Kubernetes**: Preparado para deploy no **AKS** com suporte a **HPA (Horizontal Pod Autoscaler)**, garantindo que a indexação não se torne um gargalo durante grandes volumes de cadastros.

## 🛠 Tecnologias Utilizadas

- **Runtime**: .NET 8 (Worker Service)
- **Mensageria**: RabbitMQ com MassTransit
- **Motor de Busca**: Elasticsearch (Elastic Cloud)
- **Conteinerização**: Docker (Multi-stage build / Alpine)
- **Orquestração**: Kubernetes (AKS)

## 🐳 Execução via Docker (Local)

Certifique-se de possuir uma instância do RabbitMQ e do Elasticsearch disponíveis:

```bash
# Build da imagem
docker build -t fiap-cloud-games-indexer-worker .

# Execução do container (exemplo de variáveis)
docker run \
  -e ElasticsearchUri="Sua-Uri" \
  -e ElasticsearchApiKey="Sua-Key" \
  -e RabbitMQ__Host="localhost" \
  fiap-cloud-games-indexer-worker
```

## ⚓ Kubernetes e Resiliência

O Worker opera no cluster AKS com foco em processamento garantido:
- **Consumo Resiliente**: Utiliza políticas de Retry do MassTransit para garantir que falhas temporárias no Elasticsearch não causem perda de mensagens.
- **Escalabilidade por Demanda**: O HPA pode ser configurado para criar novas instâncias do Worker conforme a fila de mensagens no RabbitMQ cresce, mantendo o índice sempre atualizado.

## 📈 Monitoramento (APM)

Integrado ao **New Relic**, o Worker fornece métricas de:
- Tempo médio de processamento de cada mensagem.
- Taxa de sucesso/erro de indexação.
- Monitoramento de saúde da conexão com o RabbitMQ.

---
**FIAP - Arquitetura de Sistemas .NET com Azure**
*Grupo 142*