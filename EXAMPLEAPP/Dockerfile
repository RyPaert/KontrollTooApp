# Use official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /publish --no-restore

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

# Expose the port your API runs on
EXPOSE 5000

# Set the entrypoint
CMD ["dotnet", "ITB2203Application.dll"]
