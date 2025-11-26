# 1. Usar imagen del SDK para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 2. Copiar los archivos de proyecto (.csproj) y restaurar dependencias
# Esto ayuda a aprovechar la caché de Docker
COPY *.csproj ./
RUN dotnet restore

# 3. Copiar todo el resto del código y publicar la aplicación
COPY . .
RUN dotnet publish -c Release -o out

# 4. Crear la imagen final ligera para ejecutar la API
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configurar el puerto (Render usa la variable PORT automáticamente, pero esto es buena práctica)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 5. Ejecutar la API
# ¡IMPORTANTE!: Asegúrate de que "KairosAPI.dll" coincida con el nombre de tu proyecto
ENTRYPOINT ["dotnet", "KairosAPI.dll"]