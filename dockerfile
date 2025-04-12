ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:9.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:9.0


FROM ${DOTNET_RUNTIME} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${DOTNET_SDK} AS build
WORKDIR /src

ENV ASPNETCORE_ENVIRONMENT=Production

COPY ["CashSmart.API/CashSmart.API.csproj", "API/"]
COPY ["CashSmart.Application/CashSmart.Application.csproj", "Application/"]
COPY ["CashSmart.Core/CashSmart.Core.csproj", "Domain/"]

RUN dotnet restore "API/CashSmart.API.csproj"

COPY . .

COPY .env ../.env

WORKDIR /src/API
RUN dotnet build "CashSmart.API.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src/API
RUN dotnet publish "CashSmart.API.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT [ "dotnet", "API.dll" ]