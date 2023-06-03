FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 7979
EXPOSE 8989

ENV ASPNETCORE_URLS=https://+:7979;http://+:8989
ENV ASPNETCORE_ENVIRONMENT=Docker

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["../src/Gateway/Gateway.csproj", "./"]
RUN dotnet restore "Gateway.csproj"
COPY ../src/Gateway/. .
RUN dotnet build "Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YARP.Gateway.dll"]
