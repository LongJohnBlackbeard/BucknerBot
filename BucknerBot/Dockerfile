FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

# Install FFmpeg & unversioned libopus.so
USER root
RUN apt-get update && \
    apt-get install -y ffmpeg libopus-dev && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BucknerBot/BucknerBot.csproj", "BucknerBot/"]
RUN dotnet restore "BucknerBot/BucknerBot.csproj"
COPY . .
WORKDIR "/src/BucknerBot"
RUN dotnet build "BucknerBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BucknerBot.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "BucknerBot.dll"]
