ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder AS deps

RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Fanex.CommonAPI.Domain/Fanex.CommonAPI.Domain.csproj", "./Fanex.CommonAPI.Domain/"]
RUN dotnet restore "Fanex.CommonAPI.Domain/Fanex.CommonAPI.Domain.csproj" -v n -r linux-x64

COPY ["src/Fanex.CommonAPI.Application/Fanex.CommonAPI.Application.csproj", "./Fanex.CommonAPI.Application/"]
RUN dotnet restore "Fanex.CommonAPI.Application/Fanex.CommonAPI.Application.csproj" -v n -r linux-x64

COPY ["src/Fanex.CommonAPI.Infrastructure/Fanex.CommonAPI.Infrastructure.csproj", "./Fanex.CommonAPI.Infrastructure/"]
RUN dotnet restore "Fanex.CommonAPI.Infrastructure/Fanex.CommonAPI.Infrastructure.csproj" -v n -r linux-x64

COPY ["src/Fanex.CommonAPI.Presentation/Fanex.CommonAPI.Presentation.csproj", "./Fanex.CommonAPI.Presentation/"]
RUN dotnet restore "Fanex.CommonAPI.Presentation/Fanex.CommonAPI.Presentation.csproj" -v n -r linux-x64

COPY ["src/Fanex.CommonAPI.WebApi/Fanex.CommonAPI.WebApi.csproj", "./Fanex.CommonAPI.WebApi/"]
RUN dotnet restore "Fanex.CommonAPI.WebApi/Fanex.CommonAPI.WebApi.csproj" -v n -r linux-x64

COPY src .

RUN dotnet publish "Fanex.CommonAPI.WebApi/Fanex.CommonAPI.WebApi.csproj" -c Release -o /publish -r linux-x64 \
    --use-current-runtime \
    --self-contained false

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine-25h1 AS final

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

# Dont run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app

COPY --from=builder /publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.CommonAPI.WebApi.dll"]
