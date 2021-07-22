FROM mcr.microsoft.com/dotnet/aspnet:5.0
RUN dotnet public -c Release
COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "authServer.dll"]
