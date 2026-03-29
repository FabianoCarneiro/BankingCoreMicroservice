FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Core.Domain/Core.Domain.csproj", "src/Core.Domain/"]
COPY ["src/Core.Application/Core.Application.csproj", "src/Core.Application/"]
COPY ["src/Core.Infrastructure/Core.Infrastructure.csproj", "src/Core.Infrastructure/"]
COPY ["src/Core.API/Core.API.csproj", "src/Core.API/"]
COPY ["src/Shared/Shared.DTOs/Shared.DTOs.csproj", "src/Shared/Shared.DTOs/"]
COPY ["src/Shared/Shared.HttpClients/Shared.HttpClients.csproj", "src/Shared/Shared.HttpClients/"]

# Restore and build
RUN dotnet restore "src/Core.API/Core.API.csproj"
COPY . .
RUN dotnet build "src/Core.API/Core.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "src/Core.API/Core.API.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Core.API.dll"]
