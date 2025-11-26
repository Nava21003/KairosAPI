# 📋 INSTRUCCIONES PARA DEPLOYMENT EN AZURE

## ⚠️ IMPORTANTE: Migración de Base de Datos

Antes de usar la API en producción, **DEBES ejecutar el script de migración** en la BD de Azure.

### Paso 1: Crear la Base de Datos en Azure

1. Abre el **Portal de Azure** (portal.azure.com)
2. Crea una **Azure SQL Database** (o usa la existente)
3. Anota la **cadena de conexión**

### Paso 2: Ejecutar el Script de Migración

**Opción A - Desde el Portal de Azure:**
1. Ve a tu SQL Database en el portal
2. Click en **Query Editor** (lado izquierdo)
3. Inicia sesión con las credenciales de la BD
4. Copia y pega el contenido de `MIGRACION-FIX-UNIQUE-CONSTRAINT.sql`
5. Click en **Run** (Ejecutar)
6. Verifica que diga "✓ Migración completada exitosamente"

**Opción B - Desde SQL Server Management Studio (SSMS):**
1. Abre SSMS
2. Conecta a tu servidor Azure SQL:
   - Server: `tu-servidor.database.windows.net`
   - Auth: SQL Server Authentication
   - Usuario/Password: (de Azure)
3. Abre el archivo `MIGRACION-FIX-UNIQUE-CONSTRAINT.sql`
4. Presiona F5 o click en "Execute"

**Opción C - Desde Azure Data Studio:**
1. Abre Azure Data Studio
2. Conecta al servidor Azure (igual que SSMS)
3. Abre el script SQL
4. Ejecuta

### Paso 3: Actualizar appsettings.json

En el servidor de Azure (App Service), configura la **cadena de conexión**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tu-servidor.database.windows.net;Database=Kairos;User Id=tu-usuario;Password=tu-password;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

O mejor, usa **Variables de Entorno** en App Service:
- `ConnectionStrings__DefaultConnection` = (tu cadena completa)

### Paso 4: Desplegar la API

1. **Publicar desde Visual Studio:**
   - Right-click en proyecto KairosAPI
   - "Publish"
   - Selecciona tu App Service
   - Click "Publish"

2. **O usando GitHub Actions** (si ya lo configuraron)

### Paso 5: Actualizar la App Android

En `RetrofitClient.kt`, cambia la URL:

```kotlin
// Producción (Azure)
private const val BASE_URL = "https://tu-app.azurewebsites.net/api/"
```

---

## ✅ Checklist de Deployment

- [ ] Script de migración ejecutado en Azure SQL
- [ ] Cadena de conexión configurada en App Service
- [ ] API desplegada en Azure
- [ ] URL de Azure agregada a RetrofitClient.kt
- [ ] APK generado con nueva URL
- [ ] Probado registro y login con BD de Azure

---

## 🐛 Troubleshooting

### Error: "Cannot insert duplicate key in object 'dbo.UsoDigital'"
**Causa**: No ejecutaste el script de migración  
**Solución**: Ejecuta `MIGRACION-FIX-UNIQUE-CONSTRAINT.sql` en Azure

### Error: "Constraint does not exist"
**Causa**: Ya está arreglado o la BD es nueva  
**Solución**: Todo OK, continúa

### Error de conexión desde la API
**Causa**: Firewall de Azure SQL  
**Solución**: En Azure Portal → SQL Server → Firewalls → Agrega la IP del App Service

---

## 📞 Contacto

Si tienes problemas con el deployment:
1. Revisa los logs en Azure App Service (Log Stream)
2. Verifica que la BD esté accesible
3. Confirma que el script SQL se ejecutó correctamente
