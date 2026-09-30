FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src
COPY . .
RUN dotnet restore
ENTRYPOINT ["dotnet", "test", "--logger", "console"]
