FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-deps
WORKDIR /App
COPY ./ .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY --from=build-deps /App/bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "authServer.dll"]
