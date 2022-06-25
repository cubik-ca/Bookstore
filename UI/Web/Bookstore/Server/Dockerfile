#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UI/Web/Bookstore/Bookstore/Server/Bookstore.Server.csproj", "UI/Web/Bookstore/Bookstore/Server/"]
COPY ["UI/Web/Bookstore/Bookstore/Shared/Bookstore.Shared.csproj", "UI/Web/Bookstore/Bookstore/Shared/"]
COPY ["UI/Web/Bookstore/Bookstore/Client/Bookstore.Client.csproj", "UI/Web/Bookstore/Bookstore/Client/"]
RUN dotnet restore "UI/Web/Bookstore/Bookstore/Server/Bookstore.Server.csproj"
COPY . .
WORKDIR "/src/UI/Web/Bookstore/Bookstore/Server"
RUN dotnet build "Bookstore.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookstore.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookstore.Server.dll"]