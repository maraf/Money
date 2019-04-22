# Build sources.
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/src/Money.Api
RUN dotnet publish -c Release -r linux-arm -o /app

# Final image.
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim-arm32v7
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Money.Api.dll"]