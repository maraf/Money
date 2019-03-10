# Build final image.
FROM microsoft/dotnet:2.1.8-aspnetcore-runtime-stretch-slim-arm32v7
WORKDIR /app
COPY /bin/Release/netcoreapp2.1/linux-arm/publish .
ENTRYPOINT ["dotnet", "Money.UI.Backend.dll"]