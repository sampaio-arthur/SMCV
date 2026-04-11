# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solution e csproj primeiro (cache de restore)
COPY ["SMCV.sln", "./"]
COPY ["src/SMCV.Domain/SMCV.Domain.csproj", "src/SMCV.Domain/"]
COPY ["src/SMCV.Common/SMCV.Common.csproj", "src/SMCV.Common/"]
COPY ["src/SMCV.Application/SMCV.Application.csproj", "src/SMCV.Application/"]
COPY ["src/SMCV.Infrastructure/SMCV.Infrastructure.csproj", "src/SMCV.Infrastructure/"]
COPY ["src/SMCV.Features/SMCV.Features.csproj", "src/SMCV.Features/"]
COPY ["src/SMCV.Api/SMCV.Api.csproj", "src/SMCV.Api/"]
RUN dotnet restore

COPY . .
RUN dotnet publish src/SMCV.Api/SMCV.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y tzdata
ENV TZ=America/Sao_Paulo

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SMCV.Api.dll"]
