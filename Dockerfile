#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
# install System.Drawing native dependencies 
#RUN apt-get update && apt-get install -y --allow-unauthenticated \ libc6-dev \ libgdiplus \ libx11-dev \ && rm -rf /var/lib/apt/lists/*
RUN apt-get update && apt-get install -y libc6-dev libgdiplus

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["ImageLinuxOS.csproj", "."]
RUN dotnet restore "./ImageLinuxOS.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ImageLinuxOS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageLinuxOS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageLinuxOS.dll"]