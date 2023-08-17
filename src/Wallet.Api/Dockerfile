FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY . .
WORKDIR "/src/Wallet.Api/"
RUN dotnet restore "Wallet.Api.csproj"
RUN dotnet build "Wallet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wallet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.Api.dll"]