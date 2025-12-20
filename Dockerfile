FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["IndexerFunction.csproj", "./"]
RUN dotnet restore "IndexerFunction.csproj"
COPY . .
RUN dotnet publish "IndexerFunction.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .
# Como não é uma API, não precisamos de EXPOSE
ENTRYPOINT ["dotnet", "IndexerFunction.dll"]