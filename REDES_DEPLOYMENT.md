# Resultado de infraestructura — ProyectoBD2

Fecha: 18/11/2026. Guía copiable: [deploy/README_DESPLIEGUE.md](deploy/README_DESPLIEGUE.md).

## Resultado real

| Componente | Estado comprobado |
|---|---|
| Solución .NET | Compilación inicial y final: **0 errores, 0 advertencias** |
| Docker | Motor Linux iniciado; imagen multi-stage construida correctamente |
| Compose | SQL Server saludable, inicializador Exited(0), Web saludable en **http://localhost:8085** |
| SQL final | Archivo proporcionado por el usuario importado con esquema, datos y 14 procedimientos |
| Persistencia Compose | Contenedores recreados sin borrar volumen; tres usuarios y una marca de inicialización antes/después |
| Login Compose | Login Gerente y cookie válidos contra SQL del contenedor |
| PDF Linux | Previsualización y PDF real descargado desde contenedor, aproximadamente 40 KB |
| Minikube | Clúster existente iniciado sin eliminar sus cargas anteriores |
| Kubernetes | SQL StatefulSet Ready, Job de importación completado, dos Pods Web Ready; Deployment **2/2** |
| PVC | sql-data 8 Gi y dp-keys 100 Mi, ambas Bound |
| Acceso Web Kubernetes | Service por port-forward **http://localhost:8081**, health Healthy y Login comprobado por Pod |
| Ingress | nginx con host proyectobd.local; health verificado por Host header a través de puerto 8082 |
| HTTPS | Certificado local autofirmado de 7 días; `curl --cacert` a Ingress en 8443 respondió Healthy. No es certificado público ni se instaló confianza en Windows |
| Cookie entre réplicas | Login en puerto 8091 (Pod A), misma cookie aceptada en 8092 (Pod B), antes y después del rollout TLS |
| HA de Pods Web | Se eliminó un Pod de prueba: otro quedó disponible, apareció reemplazo y volvió a **2/2** |
| Persistencia Kubernetes | Se recreó sqlserver-0: permanecieron tres usuarios y una marca de importación |
| Registry remoto | Preparado, **no se hizo push**: faltan cuenta/token |
| Dokploy remoto | Preparado, **no desplegado**: faltan servidor/acceso/dominio |
| Fail2ban | Instrucciones Linux preparadas; **no instalado ni ejecutado en Windows** |
| GitHub Actions | Workflow preparado; **no ejecutado**: faltan repositorio conectado y Secrets |

Las pruebas no modificaron la base LocalDB del escritorio ni datos funcionales de la solución. Se importaron copias separadas del SQL final en volúmenes nuevos de Compose y Kubernetes. No se borró ningún volumen ni PVC. Las cargas previas de otros proyectos no se eliminaron.

## Archivos creados

- `.dockerignore`, `.gitignore`.
- `.github/workflows/docker.yml`.
- `deploy/docker/Dockerfile`, `compose.yaml`, `.env.example`.
- `deploy/docker/sql/Proyecto_BD_Final.sql` — copia exacta del archivo aprobado, no una base inventada ni una mezcla de scripts.
- `deploy/kubernetes/namespace.yaml`, `configmap.yaml`, `secret.example.yaml`.
- `deploy/kubernetes/sqlserver-pvc.yaml`, `sqlserver-service.yaml`, `sqlserver-statefulset.yaml`, `sql-init-job.yaml`.
- `deploy/kubernetes/web-deployment.yaml`, `web-service.yaml`, `ingress.yaml`.
- `deploy/scripts/prepare-local.ps1`, `init-sql.sh`, `deploy-k8s.ps1`, `prepare-tls-demo.ps1`, `test-web.ps1`.
- `deploy/README_DESPLIEGUE.md`, este informe y evidencias bajo `deploy/evidence/`.

Se generaron localmente `.env` y certificados en `deploy/docker/secrets/`; están ignorados por Git y excluidos del build. No se incluyen sus valores en este informe ni en los manifests. `secret.example.yaml` sólo contiene marcadores. El script de prueba solicita la contraseña mediante SecureString; no contiene contraseñas reales.

## Archivo existente modificado

Únicamente **`ProyectoBD.Web/Program.cs`**, para infraestructura:

1. Si existe `DATA_PROTECTION_KEYS_PATH`, persistir claves y usar ApplicationName estable `ProyectoBD.Web`.
2. Si se proporciona certificado/PFX mediante variables, proteger las claves en reposo con él. Compose y Kubernetes comparten certificado y almacenamiento de claves entre sus respectivas réplicas.
3. Middleware de forwarded headers con redes confiables configurables mediante `TRUSTED_PROXY_NETWORKS`, antes de HTTPS. No se habilitó confianza universal en proxies.

Cuando no se define ruta de claves, se conserva el comportamiento local anterior. No se modificaron Controllers, vistas, autenticación por cookies/claims, BCrypt, reportes, PEPS, cobros, anulación, Datos, Negocio, Entidades, WinForms ni SP.

Comparación con hashes conservados: **121 archivos protegidos fuera de Web, cero diferencias** (`deploy/evidence/integridad.txt`). El SQL copiado tiene SHA256 `9346E60FFFE799CC35C74170BCE214792BAA58FEAA5F51F73356C374E6498F63`, igual al original aprobado.

## Misma imagen

Se construyó `proyectobd-web:latest` y se cargó esa imagen en Minikube, sin reconstruir una variante de Kubernetes. La DLL dentro de Compose y dentro del Pod tiene el mismo SHA256:

```text
26c5525ba690daed8b926c564d0ef85c80a806082148cf58d7ff5b76e573b1a4
```

La publicación remota reutilizará esta imagen con el nombre parametrizado `USUARIO/proyectobd-web:latest`; Docker Hub no se contactó con credenciales del usuario. El workflow además publica tag por SHA de Git.

## Incidencias resueltas

- Docker Desktop estaba detenido: se inició el motor existente; no se instalaron herramientas.
- 8080 ocupado por otro proyecto: se utilizó **8085**, conservando 8080 dentro de la imagen.
- Minikube tenía cargas de otros proyectos y límite de 4 Gi. `minikube start` no aplicó los nuevos recursos a ese perfil existente. Se amplió el contenedor existente a 6 Gi, con swap máximo total de 7 Gi, sin eliminar el clúster ni sus volúmenes. No se retiraron aplicaciones existentes.
- Primer arranque SQL lento por descarga/extracción y preparación de bases del sistema: se esperó hasta que el Job completó.
- El montaje readonly en `/run/secrets` chocaba con el token Kubernetes bajo `/var/run/secrets`: se desactivó el automontaje del token en la Web, que no usa la API Kubernetes. Ambos Pods arrancaron correctamente después.
- Durante la adaptación se corrigió el namespace de `ForwardedHeadersOptions`; la compilación final pasó con 0 errores/advertencias.
- Health en HTTP sin TLS configurado puede registrar la advertencia de puerto HTTPS no determinado. No impide la demo HTTP. Con Ingress TLS y forwarded headers confiables, se verificó HTTPS correctamente.

## Alcance y límites

Dos réplicas Web demuestran recuperación de un **Pod**, no tolerancia a caída del nodo. SQL sigue siendo instancia única. La PVC de claves RWO y la afinidad de Pods son adecuadas para esta demo de un nodo; multinodo requiere RWX o almacenamiento de claves externo. Los reportes siguen en memoria, con ClientIP en Service y cookie affinity en Ingress; si muere su Pod, se regenera la previsualización. Redis/caché distribuida queda recomendado para producción, sin reescribir módulos en esta entrega.

SA, Developer Edition y `TrustServerCertificate=True` son configuración de demostración interna. Producción requiere cuenta SQL de privilegios mínimos, licencia apropiada, TLS SQL validado, backups, Secrets gestionados y controles de red según el entorno. El certificado público de dominio y despliegue remoto no se simularon como resultados ya logrados.

## Comandos exactos para continuar

Desde la raíz, ver estado y abrir Compose:

```powershell
docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml ps -a
Invoke-RestMethod http://localhost:8085/health
Start-Process http://localhost:8085
kubectl get pods,deployments,svc,ingress,pvc -n proyectobd
```

Si se cerraron las terminales de acceso Kubernetes:

```powershell
kubectl port-forward -n proyectobd svc/proyectobd-web 8081:80
# Otra terminal:
kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8082:80 8443:443
# Otra terminal:
Invoke-RestMethod http://localhost:8081/health
curl.exe --noproxy '*' --cacert deploy/docker/secrets/tls.crt --resolve proyectobd.local:8443:127.0.0.1 https://proyectobd.local:8443/health
```

Si el clúster vuelve a mostrar presión de memoria al reiniciarse y el equipo tiene recursos suficientes, el ajuste usado fue `docker update --memory 6g --memory-swap 7g minikube`. No eliminar el perfil para cambiar recursos.

Para pruebas manuales de Login/PDF (sin contraseña en archivo):

```powershell
./deploy/scripts/test-web.ps1 -BaseUrl http://localhost:8085 -Usuario gerente -Pdf
```

Registry, Dokploy, HTTPS de dominio real, Fail2ban y pipeline: seguir **C, I, J, K y L** de la guía, con credenciales propias. No se hicieron cambios remotos ni se enviaron mensajes/publicaciones externas.

## Evidencias y capturas

`deploy/evidence/`: compilación, Compose, Kubernetes 2/2, persistencia SQL antes/después, nombres de Pods antes/después de HA, sesión/PDF entre réplicas, health TLS e integridad. La lista completa de **17 capturas** que debe tomar el usuario está en **M** de `deploy/README_DESPLIEGUE.md`. Prioridad inmediata: Web Compose, health, contenedores saludables, volúmenes/PVC, Deployment 2/2, Service/Ingress, reemplazo del Pod, datos SQL persistentes y HTTPS con certificado de demostración. Capturar Dokploy/Registry/Fail2ban/Actions sólo cuando se ejecuten realmente.
