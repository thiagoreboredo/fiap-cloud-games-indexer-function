# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Ajuste: Copia o arquivo de projeto de dentro da subpasta para a pasta correspondente no container
COPY ["IndexerFunction/IndexerFunction.csproj", "IndexerFunction/"]

# Restaura as dependências apontando para o caminho correto
RUN dotnet restore "IndexerFunction/IndexerFunction.csproj"

# Copia todo o resto do código
COPY . .

# Muda o diretório de trabalho para a pasta do projeto antes de publicar
WORKDIR "/src/IndexerFunction"
RUN dotnet publish "IndexerFunction.csproj" -c Release -o /app/publish

# Estágio Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Segurança: Executar como usuário não-root (Boa prática para K8s)
USER $APP_UID

COPY --from=build /app/publish .

# Como não é uma API, não precisamos de EXPOSE
ENTRYPOINT ["dotnet", "IndexerFunction.dll"]