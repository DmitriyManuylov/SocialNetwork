# syntax=docker/dockerfile:1

# Comments are provided throughout this file to help you get started.
# If you need more help, visit the Dockerfile reference guide at
# https://docs.docker.com/engine/reference/builder/

################################################################################

# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md



# Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:6.0.400 AS build
WORKDIR /src
#COPY ["SocialNetwork/SocialNetwork.csproj", "SocialNetwork/"]
#RUN dotnet restore "SocialNetwork/SocialNetwork.csproj"
COPY . .
#WORKDIR "/src/SocialNetwork"

# Build the application.
# Leverage a cache mount to /root/.nuget/packages so that subsequent builds don't have to re-download packages.
# If TARGETARCH is "amd64", replace it with "x64" - "x64" is .NET's canonical name for this and "amd64" doesn't
#   work in .NET 6.0.
RUN dotnet build "SocialNetwork.csproj" -c Release -o /app/build
RUN dotnet publish "SocialNetwork.csproj" -c Release -o /app/publish 
#RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    #dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app/publish
##
RUN dotnet tool install --global dotnet-ef --version 5.0
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool restore
# RUN apt-get update
# RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
# RUN dpkg -i packages-microsoft-prod.deb
# RUN rm packages-microsoft-prod.deb
# RUN apt update
# RUN apt install apt-transport-https
# RUN apt update
# RUN apt-get install -y aspnetcore-runtime-5.0
#RUN dotnet ef database update
#
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 430
#
## Copy everything needed to run the app from the "build" stage.
COPY --from=build /app/publish .
COPY --from=build /root/.dotnet/tools/ /opt/bin
# RUN dotnet ef database update


# Create a non-privileged user that the app will run under.
# See https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#user

ENTRYPOINT ["dotnet", "SocialNetwork.dll"]
#ENTRYPOINT ["bash"]