FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY BigBrother.sln ./
COPY Eris/Eris/Eris.csproj ./Eris/Eris/
COPY BigBrother/BigBrother.csproj ./BigBrother/
# Restore dependencies
RUN dotnet restore

# Copy everything
COPY BigBrother/ ./BigBrother/
COPY Eris/ ./Eris/

# Build and publish a release
RUN dotnet publish -c Release --property:PublishDir=out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
# Copy the executable
COPY --from=build /src/BigBrother/out/ ./
ENTRYPOINT [ "dotnet", "BigBrother.dll" ]
