FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LibraryApi.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish LibraryApi.csproj -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000
USER $APP_UID

ENTRYPOINT ["dotnet", "LibraryApi.dll"]
