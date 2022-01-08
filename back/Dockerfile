# Build API
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-back
WORKDIR /src
COPY Consensus.sln ./
COPY ./Consensus/Consensus.csproj Consensus/Consensus.csproj
COPY ./Consensus.Quartz/Consensus.Quartz.csproj Consensus.Quartz/Consensus.Quartz.csproj
COPY ./Consensus.Data/Consensus.Data.csproj Consensus.Data/Consensus.Data.csproj
COPY ./Consensus.Common/Consensus.Common.csproj Consensus.Common/Consensus.Common.csproj
COPY ./Consensus.Bl/Consensus.Bl.csproj Consensus.Bl/Consensus.Bl.csproj
COPY ./Consensus.DataSourceHandlers.Vk/Consensus.DataSourceHandlers.Vk.csproj Consensus.DataSourceHandlers.Vk/Consensus.DataSourceHandlers.Vk.csproj
COPY ./Consensus.DataSourceHandlers.Api/Consensus.DataSourceHandlers.Api.csproj Consensus.DataSourceHandlers.Api/Consensus.DataSourceHandlers.Api.csproj
COPY ./Consensus.ApiContracts/Consensus.ApiContracts.csproj Consensus.ApiContracts/Consensus.ApiContracts.csproj
COPY ./Consensus.DataSourceHandlers.Viber/Consensus.DataSourceHandlers.Viber.csproj Consensus.DataSourceHandlers.Viber/Consensus.DataSourceHandlers.Viber.csproj

RUN dotnet restore Consensus/Consensus.csproj

COPY . .
WORKDIR Consensus
RUN dotnet publish --no-restore -c Release -o /app
RUN cp -p $(find . -name '*.pdb') /app

# Build Agent
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-agent
ARG VERSION
WORKDIR /src

COPY ./Consensus.Common/Consensus.Common.csproj Consensus.Common/Consensus.Common.csproj
COPY ./Consensus.DataSourceHandlers.Api/Consensus.DataSourceHandlers.Api.csproj Consensus.DataSourceHandlers.Api/Consensus.DataSourceHandlers.Api.csproj
COPY ./Consensus.ApiContracts/Consensus.ApiContracts.csproj Consensus.ApiContracts/Consensus.ApiContracts.csproj
COPY ./Consensus.Agent/Consensus.Agent.csproj Consensus.Agent/Consensus.Agent.csproj
COPY ./Consensus.DataSourceHandlers.Viber/Consensus.DataSourceHandlers.Viber.csproj Consensus.DataSourceHandlers.Viber/Consensus.DataSourceHandlers.Viber.csproj

RUN dotnet restore Consensus.Agent/Consensus.Agent.csproj
COPY . .
WORKDIR Consensus.Agent
RUN dotnet publish -c Release -o /agent -p:AssemblyVersion=$VERSION.0 -p:Version=$VERSION.0

# The final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000
COPY --from=build-back /app .
COPY --from=build-agent /agent/Consensus.Agent.exe ./agent/Consensus.Agent.exe

CMD dotnet Consensus.dll
