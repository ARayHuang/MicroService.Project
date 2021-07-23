FROM mcr.microsoft.com/dotnet/aspnet:3.1-focal AS base
WORKDIR /app
EXPOSE 4002

FROM mcr.microsoft.com/dotnet/sdk:3.1-focal AS build
WORKDIR /src
COPY ["Cache.Service.csproj", "./"]
RUN dotnet restore "Cache.Service.csproj"
COPY . .
RUN dotnet build "Cache.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cache.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cache.Service.dll"]
