ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:9.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:9.0

FROM ${DOTNET_RUNTIME} AS base


WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${DOTNET_SDK} AS build
WORKDIR /src


COPY . .
RUN dotnet restore "CashSmart.API/CashSmart.API.csproj"

ENV ASPNETCORE_URLS="http://+:80"

WORKDIR /src/CashSmart.API
RUN dotnet publish "CashSmart.API.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT [ "dotnet", "CashSmart.API.dll" ]