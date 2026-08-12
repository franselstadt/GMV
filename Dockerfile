# linux container, three stages: build the client, publish the api, run on the aspnet runtime

# stage 1: build the react client with node
FROM node:20-alpine AS client
WORKDIR /client
COPY gmvTM.Client/package.json gmvTM.Client/package-lock.json ./
RUN npm ci
COPY gmvTM.Client/ ./
RUN npm run build

# stage 2: publish the api, the PublishClient msbuild target picks up the client dist and drops it into wwwroot
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY global.json Directory.Build.props ./
COPY gmvTM.Domain/ gmvTM.Domain/
COPY gmvTM.Application/ gmvTM.Application/
COPY gmvTM.Server/ gmvTM.Server/
COPY --from=client /client/dist gmvTM.Client/dist/
RUN dotnet publish gmvTM.Server/gmvTM.Server.csproj -c Release -o /app/publish

# stage 3: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ConnectionStrings__Default="Data Source=/data/gmvtm.db"
VOLUME ["/data"]
EXPOSE 8080
ENTRYPOINT ["dotnet", "gmvTM.Server.dll"]
