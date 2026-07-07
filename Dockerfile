FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN apt-get update && \
    apt-get install -y wget ca-certificates && \
    wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-8.0

WORKDIR /src

COPY . .

RUN dotnet publish WashZone.csproj -c Release -o /app/publish

COPY start.sh /start.sh
RUN chmod +x /start.sh

EXPOSE 8080 1433

ENTRYPOINT ["/start.sh"]