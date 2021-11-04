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

RUN dotnet restore Consensus/Consensus.csproj

COPY . .
WORKDIR Consensus
RUN dotnet publish --no-restore -c Release -o /app
RUN cp -p $(find . -name '*.pdb') /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000
COPY --from=build-back /app .

CMD dotnet Consensus.dll
