FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Text-Captcha.API/Text-Captcha.API.csproj", "Text-Captcha.API/"]
RUN dotnet restore "Text-Captcha.API/Text-Captcha.API.csproj"
COPY . .
WORKDIR "/src/Text-Captcha.API"
RUN dotnet build "./Text-Captcha.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Text-Captcha.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Text-Captcha.API.dll"]
