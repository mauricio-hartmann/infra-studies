FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
RUN apk add --no-cache curl
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/IS.Ticket.API/IS.Ticket.API.csproj", "src/IS.Ticket.API/"]
COPY ["src/shared/IS.Core.API/IS.Core.API.csproj", "src/shared/IS.Core.API/"]
COPY ["src/shared/IS.Core/IS.Core.csproj", "src/shared/IS.Core/"]
RUN dotnet restore "./src/IS.Ticket.API/IS.Ticket.API.csproj"
COPY . .
WORKDIR "/src/src/IS.Ticket.API"
RUN dotnet build "./IS.Ticket.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IS.Ticket.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IS.Ticket.API.dll"]