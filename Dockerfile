FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-back
WORKDIR /src
COPY Consensus.sln ./
COPY ./Consensus/Consensus.csproj Consensus/Consensus.csproj
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
