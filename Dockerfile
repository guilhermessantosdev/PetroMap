# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copiar o arquivo de solução (.sln)
COPY PetroMap.sln .

# Copiar o arquivo .csproj corretamente da subpasta
COPY PetroMap/PetroMap.csproj ./PetroMap/

# Restaurar as dependências
RUN dotnet restore

# Copiar todo o restante do código
COPY . .

# Publicar o projeto em modo Release
RUN dotnet publish ./PetroMap -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copiar os arquivos publicados da etapa de build
COPY --from=build /app/publish .

# Definir a URL do ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Ponto de entrada
ENTRYPOINT ["dotnet", "PetroMap.dll"]
