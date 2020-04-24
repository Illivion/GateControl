#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 20080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GateControl.Web/GateControl.Web.csproj", "GateControl.Web/"]
RUN dotnet restore "GateControl.Web/GateControl.Web.csproj"
COPY . .
WORKDIR "/src/GateControl.Web"
RUN dotnet build "GateControl.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GateControl.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GateControl.Web.dll"]