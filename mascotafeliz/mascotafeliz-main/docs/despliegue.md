# Guía de despliegue — Mascota Feliz

Esta guía explica cómo desplegar la aplicación en Azure App Service usando
el pipeline de GitHub Actions.

## Recursos de Azure necesarios

Crear los siguientes recursos en Azure Portal (nombres sugeridos por Giraldo):

| Recurso | Nombre | Plan |
|---------|--------|------|
| Resource Group | `mascotafeliz-prod-rg` | — |
| App Service (Linux) | `mascotafeliz-prod` | B1 (Basic) |
| Azure SQL Database | `mascotafeliz-prod-sql` | Basic (DTU) |
| Storage Account | `mascotafelizprod` | Standard LRS |

## Configuración de Azure SQL Database

1. Crear el servidor SQL y la base de datos `MascotaFelizProd`.
2. Obtener el connection string (ADO.NET) desde Azure Portal → Connection strings.
3. La app aplicará las migraciones automáticamente al iniciar (`Database.Migrate()`).

## Configuración de App Service

En Azure Portal → App Service → Settings → Configuration → Application settings:

| Nombre | Valor |
|--------|-------|
| `ConnectionStrings__MascotaFeliz` | Connection string de Azure SQL |
| `Storage__ConnectionString` | Connection string de Azure Blob Storage |

> **Importante:** Estas credenciales se configuran en Azure Portal, **nunca** en el código
> ni en los archivos del workflow de GitHub Actions.

## Configuración del blob container

1. En el Storage Account, crear el contenedor `mascotas-fotos`.
2. Configurar el nivel de acceso público del contenedor a **Blob** (permite leer las fotos por URL).

## Configuración de GitHub Secrets

En el repositorio de GitHub → Settings → Secrets and variables → Actions:

| Secret | Valor |
|--------|-------|
| `AZURE_PUBLISH_PROFILE` | Publish profile del App Service (descargar desde Azure Portal → App Service → Overview → Get publish profile) |

## Pipeline de CI/CD

El workflow `.github/workflows/ci-cd.yml` hace lo siguiente en cada push a `main`:

1. **Build**: `dotnet restore` + `dotnet build --configuration Release`
2. **Test**: `dotnet test --configuration Release` (60 pruebas)
3. **Publish**: `dotnet publish` a carpeta `./publish`
4. **Deploy**: Sube el paquete a Azure App Service con `azure/webapps-deploy@v3`

Los pull requests ejecutan solo build + test (sin deploy).

## Verificación del primer despliegue

Después del primer deploy:

1. Visitar la URL del App Service — debería mostrar el sitio público.
2. Ir a `/Cuenta/Login` — iniciar sesión con las credenciales del Admin inicial
   (la contraseña temporal se mostró en los logs de App Service al primer inicio).
3. **Cambiar la contraseña del Admin** inmediatamente.
4. Verificar que las migraciones se aplicaron (revisar tablas en Azure SQL).

## Notas

- El App Service debe tener .NET 10 runtime (se configura automáticamente con `dotnet publish`).
- Las migraciones EF se aplican automáticamente al iniciar la app (`Database.Migrate()`).
- En producción no hay datos de ejemplo (solo en Development).
- El rate limiting en `/Agendar` es en memoria — con múltiples instancias, migrar a Redis.
