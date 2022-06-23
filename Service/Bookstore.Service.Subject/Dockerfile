FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Service/Bookstore.Service.Subject/Bookstore.Service.Subject.csproj", "Service/Bookstore.Service.Subject/"]
RUN dotnet restore "Service/Bookstore.Service.Subject/Bookstore.Service.Subject.csproj"
COPY . .
WORKDIR "/src/Service/Bookstore.Service.Subject"
RUN dotnet build "Bookstore.Service.Subject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookstore.Service.Subject.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookstore.Service.Subject.dll"]
