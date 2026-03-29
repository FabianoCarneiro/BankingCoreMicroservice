FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Customer.Service/Customer.Domain/Customer.Domain.csproj", "src/Customer.Service/Customer.Domain/"]
COPY ["src/Customer.Service/Customer.Application/Customer.Application.csproj", "src/Customer.Service/Customer.Application/"]
COPY ["src/Customer.Service/Customer.Infrastructure/Customer.Infrastructure.csproj", "src/Customer.Service/Customer.Infrastructure/"]
COPY ["src/Customer.Service/Customer.API/Customer.API.csproj", "src/Customer.Service/Customer.API/"]
COPY ["src/Shared/Shared.DTOs/Shared.DTOs.csproj", "src/Shared/Shared.DTOs/"]

# Restore and build
RUN dotnet restore "src/Customer.Service/Customer.API/Customer.API.csproj"
COPY . .
RUN dotnet build "src/Customer.Service/Customer.API/Customer.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "src/Customer.Service/Customer.API/Customer.API.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Customer.API.dll"]
