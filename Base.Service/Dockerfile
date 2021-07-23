FROM mcr.microsoft.com/dotnet/aspnet:3.1-focal AS base
WORKDIR /app
EXPOSE 4001

FROM mcr.microsoft.com/dotnet/sdk:3.1-focal AS build
WORKDIR /src
COPY ["Base.Service.csproj", "./"]
RUN dotnet restore "Base.Service.csproj"
COPY . .
RUN dotnet build "Base.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Base.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Base.Service.dll"]
