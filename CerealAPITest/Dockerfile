# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the project files and restore dependencies
COPY CerealAPITest/CerealAPITest.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY CerealAPITest . ./

# Build the project
RUN dotnet build CerealAPITest/CerealAPITest.csproj -c Debug

# Run the tests
HEALTHCHECK --timeout=20s --retries=3 CMD dotnet test bin/Debug/net8.0/CerealAPITest.dll --no-build --verbosity normal