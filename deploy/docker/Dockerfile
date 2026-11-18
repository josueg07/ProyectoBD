FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ProyectoBD.Web/ProyectoBD.Web.csproj ProyectoBD.Web/
COPY ProyectoBD.Negocio/ProyectoBD.Negocio.csproj ProyectoBD.Negocio/
COPY ProyectoBD.Datos/ProyectoBD.Datos.csproj ProyectoBD.Datos/
COPY ProyectoBD.Entidades/ProyectoBD.Entidades.csproj ProyectoBD.Entidades/
RUN dotnet restore ProyectoBD.Web/ProyectoBD.Web.csproj
COPY ProyectoBD.Web/ ProyectoBD.Web/
COPY ProyectoBD.Negocio/ ProyectoBD.Negocio/
COPY ProyectoBD.Datos/ ProyectoBD.Datos/
COPY ProyectoBD.Entidades/ ProyectoBD.Entidades/
RUN dotnet publish ProyectoBD.Web/ProyectoBD.Web.csproj -c Release --no-restore -o /out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /var/dpkeys && chown app:app /var/dpkeys
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080 ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
USER app
HEALTHCHECK --interval=20s --timeout=5s --start-period=30s CMD curl -fsS http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "ProyectoBD.Web.dll"]
