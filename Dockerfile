# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copiar o arquivo de solução (.sln)
COPY *.sln . 

# Copiar o arquivo .csproj para o diretório correto
COPY PetroMap.csproj ./PetroMap/

# Restaurar as dependências (não esquecer de restaurar os pacotes!)
RUN dotnet restore

# Copiar o restante do código
COPY . .

# Publicar o projeto
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copiar a pasta publish do container de build
COPY --from=build /app/publish . 

# Definir a URL do ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Definir o ponto de entrada
ENTRYPOINT ["dotnet", "PetroMap.dll"]