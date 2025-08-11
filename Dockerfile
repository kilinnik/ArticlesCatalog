FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src
COPY ["ArticlesCatalog.Api/ArticlesCatalog.Api.csproj", "ArticlesCatalog.Api/"]
RUN dotnet restore "ArticlesCatalog.Api/ArticlesCatalog.Api.csproj"

FROM restore AS build
COPY . .
WORKDIR /src/ArticlesCatalog.Api
RUN dotnet build "ArticlesCatalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArticlesCatalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
RUN adduser --disabled-password --home /app appuser && chown -R appuser /app
USER appuser
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ArticlesCatalog.Api.dll"]
