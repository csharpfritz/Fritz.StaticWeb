FROM mcr.microsoft.com/dotnet/sdk:7.0

COPY ./ /app

WORKDIR /app/Test.StaticBlog

RUN dotnet restore

