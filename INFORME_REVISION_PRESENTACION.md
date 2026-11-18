# Revisión de ProyectoBD para la presentación

Fecha: 18 de noviembre de 2026. Revisión de sólo lectura del código, archivos de infraestructura, entorno local y panel remoto. Los estados son una fotografía del momento de la revisión, no una garantía futura. No se modificó código ni configuración, no se hizo deploy y no se borraron ni reiniciaron componentes. Único archivo nuevo: este informe.

**Cómo leer la evidencia:** «actual» significa observado durante esta revisión; «histórica» significa que existe un archivo de evidencia anterior; «NO VERIFICADO» identifica lo que no se pudo comprobar. No se ejecutaron nuevamente pruebas que crean, anulan o eliminan datos. No se mostraron secretos.

## A. Resumen general del proyecto.

ProyectoBD es un sistema de compras, inventario y ventas con dos interfaces: Windows Forms y Web ASP.NET Core MVC. Ambas reutilizan Entidades, Negocio y Datos. SQL Server conserva catálogos, usuarios, documentos, lotes, movimientos de inventario y pagos.

Los módulos incluyen autenticación, Usuarios/Roles, cambio de contraseña, Clientes, Proveedores, Marcas, Categorías, Presentaciones, Colores, Productos, Variantes, Cotizaciones, Órdenes de compra, Ingreso a bodega completo/parcial, Facturación PEPS, Cobros, medios de pago, consultas, anulación y reportes/PDF.

Existen dos demostraciones de infraestructura separadas:

- **Local:** Docker Compose y Kubernetes sobre Minikube. Kubernetes tiene dos réplicas Web y un SQL Server.
- **Remota:** servidor 167.172.121.38 con Dokploy y Docker Compose; una Web y un SQL Server. No es el clúster Minikube.

URL oficial: [https://proyectobd.duckdns.org](https://proyectobd.duckdns.org). `/health` devuelve actualmente **200, Healthy**. La página de Login abierta directamente por HTTPS devuelve 200. Atención: la raíz HTTPS redirige actualmente hacia Login por **HTTP**; se detalla en P. No se corrigió por el alcance de sólo lectura.

Para defender la aplicación conviene mostrar un ingreso a bodega, una factura que utiliza lotes PEPS, un cobro parcial/completo y un reporte PDF. Para Redes: contenedores saludables, persistencia, dos Pods Web, Service/Ingress y HTTPS público.

## B. Arquitectura de la aplicación.

La solución `ProyectoBD2.sln` contiene estos cinco proyectos:

| Proyecto | Tecnología y responsabilidad | Referencias |
|---|---|---|
| ProyectoBD.Entidades | .NET 8; objetos compartidos y UsuarioSesion | Sin referencia a las otras capas |
| ProyectoBD.Datos | .NET 8; conexión y acceso ADO.NET | Entidades |
| ProyectoBD.Negocio | .NET 8; reglas y permisos | Datos, Entidades |
| ProyectoBD.Presentacion | .NET 8 Windows Forms; `net8.0-windows` | Negocio, Entidades |
| ProyectoBD.Web | ASP.NET Core MVC .NET 8 | Negocio, Entidades |

Flujo: **navegador → Controller MVC → Negocio → Datos → SQL Server**, y respuesta hacia una vista Razor. La Web no utiliza un DbContext. El acceso es ADO.NET con `Microsoft.Data.SqlClient` **7.1.0**, comandos parametrizados y procedimientos almacenados; algunos detalles se transmiten como JSON. No se encontró uso de Entity Framework ni Dapper.

`ProyectoBD.Datos/ConexionBD.cs` busca primero `PROYECTOBD_CONNECTION_STRING`. Cuando no existe utiliza LocalDB de Windows, `(localdb)\MSSQLLocalDB`, base `Proyecto_BD`, con seguridad integrada. Docker/Kubernetes suministran la variable para usar SQL Server de red; Linux no depende de LocalDB. En Compose el servidor es `sqlserver,1433`.

La autenticación utiliza cookies y Claims. `AccountController` llama a Negocio; `BCrypt.Net-Next` **4.2.0** verifica contraseñas. El helper `User.ObtenerSesion()` entrega `UsuarioSesion` a los procesos. Los Controllers aplican autorización por rol y los POST usan validación antifalsificación; Negocio vuelve a validar permisos. Ocultar una opción del menú no sustituye esas comprobaciones.

Roles Web observados: **Digitador**, catálogos y procesos de cotización/compra/bodega/facturación; **Cajero**, cobros y medios de pago; **Gerente**, Usuarios/Roles, reportes, consultas y anulación. Algunas reglas compartidas permiten operaciones de escritorio a más de un rol; la autorización Web puede ser más restrictiva.

UI: Razor, Bootstrap **5.1.0** y jQuery. PDF: `PDFsharp-MigraDoc` **6.2.4**, variante Core, con fuentes Noto incluidas. `Services/ReportesMemoria.cs` conserva reportes temporalmente en memoria por usuario y token, durante **20 minutos**: esa memoria no se comparte entre réplicas.

`Program.cs` configura cookies, autorización, Data Protection, forwarded headers, HTTPS y `/health`. El health check registrado es general de aplicación: **Healthy no prueba por sí solo que se pueda consultar SQL**. Los proxies confiables se configuran mediante `TRUSTED_PROXY_NETWORKS`.

Base de datos: el archivo final desplegable es `deploy/docker/sql/Proyecto_BD_Final.sql`, copia del original `C:\Users\josue\Desktop\Proyecto Bases de Datos 2\ProyectoBD.sql`. SHA-256 comprobado: `9346E60FFFE799CC35C74170BCE214792BAA58FEAA5F51F73356C374E6498F63`. Contiene **27 tablas y 14 procedimientos**. No contiene definiciones `CREATE FUNCTION` ni `CREATE TRIGGER`; la ausencia de esos objetos en toda la instancia SQL es **NO VERIFICADO**.

Procedimientos importantes:

- `usp_GuardarCotizacion`, `usp_EstadoCotizacion`, `usp_GuardarOrdenCompra`, `usp_RegistrarIngresoBodega`.
- `usp_FacturarPEPS`, `usp_RegistrarPagos`, `usp_AnularFactura`.
- `usp_ReporteFacturacionFormasPago`, `usp_ReporteProductosMayoresIngresos`, `usp_ReporteProductosMasVendidos`, `usp_ReporteInventarioActual`, `usp_ReporteProductosMenosVendidos`, `usp_ReporteProductosSinExistencia`, `usp_ReporteDetalleFactura`.

**PEPS/FIFO real:** la facturación toma lotes con existencia, ordenados por `FechaIngreso ASC, ID_Lote ASC`. Usa transacción, bloqueos `UPDLOCK/HOLDLOCK` y `sp_getapplock` con recurso `ProyectoBD_FacturacionPEPS`. Registra la asignación en `DetalleFacturaLote`, descuenta existencias y crea movimientos `SALIDA_VENTA`. Si falta inventario, revierte la transacción. La anulación devuelve cantidades a los lotes asignados, registra `ENTRADA_ANULACION` y auditoría, preservando el historial de pagos.

## C. Arquitectura Docker.

Dockerfile: `deploy/docker/Dockerfile`. Etapa de compilación con `mcr.microsoft.com/dotnet/sdk:8.0`: restaura y publica Release con `UseAppHost=false`. Etapa final con `mcr.microsoft.com/dotnet/aspnet:8.0`: copia la publicación y fuentes, instala curl y prepara `/var/dpkeys`. La aplicación corre como usuario **app**, UID **1654**, no como root. Escucha HTTP interno **8080**, ambiente Production. El health check ejecuta curl contra `/health`, intervalo 20 s, timeout 5 s e inicio de gracia 30 s.

`.dockerignore` evita enviar al build secretos, `.env`, bin/obj, `.git`, `.vs`, la presentación WinForms y material de pruebas excluido. Una imagen puede contener varias capas; la etapa final no necesita conservar el SDK.

| Archivo | Uso |
|---|---|
| `deploy/docker/compose.yaml` | Local; construye Web, permite `WEB_IMAGE`, publica el puerto Web; monta PFX local para Data Protection |
| `deploy/docker/compose.dokploy.yaml` | Remoto; usa imagen del registry, sin build; `expose: 8080`, sin puerto host Web/SQL; no depende de PFX local |

Servicios: **sqlserver**, **sql-init**, **web**. SQL usa `mcr.microsoft.com/mssql/server:2022-latest`, edición Developer. La dependencia es **SQL saludable → sql-init finalizado correctamente → Web**. `sql-init` ejecuta `deploy/scripts/init-sql.sh` con el SQL montado desde `./sql/Proyecto_BD_Final.sql` y el script `../scripts/init-sql.sh`.

El script espera una conexión SQL mediante sqlcmd. Si la base no existe, importa esquema/datos y registra el marcador técnico `__DeploymentInitialization`. Si ya existe con marcador, omite la importación. Si existe sin marcador, falla para evitar inicializar encima de una base desconocida. **Exited (0) es correcto:** es un trabajo finito que terminó bien, no un servicio que deba quedar ejecutándose.

Redes lógicas: **datos**, interna, y **frente**. SQL no publica 1433 al host. Volúmenes: **sql-data** en `/var/opt/mssql` y **dp-keys** en `/var/dpkeys`. El Compose local añade certificado de Data Protection; el remoto no monta `./secrets/dp.pfx`.

Estado local actual: Web saludable en puerto **8085 → 8080**, SQL saludable y sql-init Exited 0. `docker inspect` de SQL devolvió `PortBindings: {}`. Los nombres completos aparecen en K.

## D. Arquitectura Kubernetes.

Contexto y nodo **minikube**; host, kubelet y apiserver en ejecución. Namespace real: **proyectobd**. Es un clúster local de un nodo, no el servidor de DigitalOcean.

| Recurso | Configuración/estado actual |
|---|---|
| Deployment `proyectobd-web` | Dos réplicas, **2/2 disponibles**, imagen efectiva `proyectobd-web:latest` |
| StatefulSet `sqlserver` | Una réplica, **1/1**, SQL Server 2022 |
| Job `sql-init` | **Complete 1/1**, duración registrada 6 m 4 s |
| Service `proyectobd-web` | ClusterIP **10.105.181.8**, puerto 80 → 8080 |
| Service `sqlserver` | Headless, ClusterIP None, puerto 1433 |
| Ingress `proyectobd-web` | Clase nginx, host **proyectobd.local**, dirección **192.168.49.2**, puertos 80/443 |
| PVC `sql-data` | **Bound**, 8 Gi, ReadWriteOnce, storageClass standard |
| PVC `dp-keys` | **Bound**, 100 Mi, ReadWriteOnce, storageClass standard |

Los YAML están en `deploy/kubernetes`. El archivo Web contiene un nombre de imagen sustituible; `deploy/scripts/deploy-k8s.ps1` prepara el despliegue. La imagen efectiva consultada en el Deployment es la indicada arriba.

ConfigMaps: **web-config**, **sql-init-files**, **kube-root-ca.crt**. Secrets observados sólo por nombre/tipo, sin leer valores: **sql-secret** (Opaque), **web-secret** (Opaque), **dp-certificate** (Opaque) y **proyectobd-tls** (`kubernetes.io/tls`).

Web: readiness `/health` cada 5 s; liveness cada 15 s; startup con hasta 30 intentos cada 5 s. SQL: comprobaciones TCP 1433 de startup/readiness; no tiene liveness SQL configurada. Una prueba TCP demuestra que el puerto acepta conexiones, no que todas las consultas funcionan.

Web solicita 100 millicores/128 MiB y limita 1 CPU/512 MiB. Corre con UID/fsGroup 1654, sin elevación de privilegios, capacidades eliminadas y sin automontar token de ServiceAccount. SQL solicita 250 millicores/2 GiB y limita 2 CPU/3 GiB; fsGroup 10001.

**Balanceo y sesión:** el Service distribuye conexiones entre endpoints listos y utiliza `sessionAffinity: ClientIP`. Ingress tiene afinidad de cookie persistente **ProyectoBDRoute**. Data Protection comparte claves mediante PVC y certificado para poder validar cookies entre réplicas. Los reportes en memoria siguen dependiendo del proceso: la afinidad ayuda, pero perder un Pod puede obligar a generar nuevamente un PDF pendiente.

El PVC de claves es RWO; hay afinidad de Pods para ubicarlos en el mismo nodo. Esto funciona en este Minikube de un nodo, pero no demuestra almacenamiento compartido entre nodos ni alta disponibilidad multinodo.

TLS local: Ingress activo tiene redirección SSL y Secret **proyectobd-tls**. `prepare-tls-demo.ps1` prepara TLS de demostración; la evidencia existente utiliza certificado autofirmado local. El archivo base de Ingress y el recurso efectivo pueden diferir por los ajustes del script. No confundirlo con Let's Encrypt del dominio público.

Si desaparece un Pod Web, el Deployment crea otro y el Service utiliza endpoints listos. Puede haber fallos transitorios de solicitudes al Pod perdido. Si desaparece `sqlserver-0`, el StatefulSet lo recrea usando el mismo PVC; SQL tarda en recuperar servicio. **SQL tiene una única réplica y existe un único nodo: no hay alta disponibilidad completa.** No se eliminó ningún Pod durante esta revisión.

## E. DigitalOcean + Dokploy.

Servidor público **167.172.121.38**. Ubuntu 24.04 x64 fue indicado por el propietario; verificación directa del sistema operativo: **NO VERIFICADO**. El acceso SSH de consulta falló por autenticación (`Permission denied`); no se introdujeron contraseñas. `free -h` y `uptime` remotos: **NO VERIFICADO**. La conexión DigitalOcean disponible no devolvió droplets de esta cuenta, por lo que no se utilizó como evidencia de hardware.

En Dokploy se verificó: versión **v0.31.4**, Docker Engine **28.5.0**, nodo **proyectobd-dokploy**, estado Leader/Active, 1/1 online, **2 CPU**, **3.82 GiB RAM**. Monitorización aproximada: CPU **2.50 %**, RAM **2.13/3.82 GiB**, disco **8.71/76.45 GB**, uso Docker **6.20 GB**. Son valores del panel, no una salida SSH de `free`/`df`.

Configuración del proyecto:

| Campo | Valor |
|---|---|
| Proyecto | ProyectoBD |
| Environment | production |
| Compose | ProyectoBD |
| Identificador | proyectobd-proyectobd-jsdy5d |
| Repositorio | https://github.com/josueg07/ProyectoBD.git |
| Rama | main |
| Compose Path | deploy/docker/compose.dokploy.yaml |
| Servicio del dominio | web |
| Puerto interno | 8080 |
| Dominio oficial | proyectobd.duckdns.org, HTTPS, certificado letsencrypt |

Dokploy obtiene el repositorio, carga el Compose y aplica su configuración y parche persistente. El parche de imagen corrige el usuario del registry a **josueg07**. El archivo versionado todavía dice **josueg077**: no son el mismo nombre. Los contenedores observados usan **josueg07/proyectobd-web:latest**.

Variable obligatoria del Compose: **MSSQL_SA_PASSWORD**. El Compose genera `PROYECTOBD_CONNECTION_STRING` hacia `sqlserver,1433` y conserva `DATA_PROTECTION_KEYS_PATH=/var/dpkeys`. No se abrieron ni reprodujeron valores secretos de Environment. Valor efectivo de variables sensibles: **NO VERIFICADO**.

Traefik enruta por dominio hacia `web:8080`; es quien termina el TLS público. SQL y sql-init no tienen dominio público. Existe también la asociación temporal HTTP `proyectobd-proyectobd-jsdy5d-31a649-167-172-121-38.sslip.io`, sin certificado. No se eliminó.

Flujo remoto: **Internet → resolución DuckDNS → IP DigitalOcean → Traefik:443 → Web:8080 → SQL:1433 por red Docker**. DuckDNS proporciona DNS, no aloja la aplicación ni procesa las consultas SQL.

El nodo pertenece a Swarm para la plataforma Dokploy; eso no significa que esta aplicación Compose tenga dos réplicas ni que use Kubernetes. Se observaron los tres contenedores de aplicación y volúmenes de Dokploy; inventario completo de contenedores internos Traefik/Dokploy/Postgres/Redis: **NO VERIFICADO**.

## F. GitHub + Docker Hub + CI/CD.

Repositorio **josueg07/ProyectoBD**, rama **main**, HEAD local observado **d04a7a1**. Árbol de trabajo limpio antes de crear este informe. No se hizo fetch, commit ni push en esta revisión.

Workflow `.github/workflows/docker.yml`:

1. Se activa con push a main o `workflow_dispatch`.
2. Ejecuta un runner `ubuntu-latest` y obtiene el código con checkout v4.
3. Prepara Buildx v3.
4. Inicia sesión en Docker Hub con login-action v3 usando Secrets **REGISTRY_USERNAME** y **REGISTRY_PASSWORD**. Se verificaron los nombres en el YAML, no sus valores.
5. build-push-action v6 construye desde contexto raíz y `deploy/docker/Dockerfile`.
6. Publica tags del usuario configurado: **latest** y el **SHA completo del commit**.

Última ejecución consultada por API pública: **Docker Web**, main, commit d04a7a1…, **completed / success**, 18/11/2026 15:02:04 UTC (09:02:04 Guatemala). [Ejecución 44330906876](https://github.com/josueg07/ProyectoBD/actions/runs/44330906876). La ejecución anterior de a12215d también fue exitosa; existe una más antigua fallida de 8039d3e, superada por las siguientes.

Repositorio público Docker Hub observado: **josueg07/proyectobd-web**, tag **latest**. Metadatos actuales: actualizado 18/11/2026 15:02:42 UTC; digest del tag `sha256:fe759a40e10112e79b3394fae2c1747fdef665ee9f108605396bca32824f5073`. El digest exacto del contenedor remoto en ejecución es **NO VERIFICADO**: que use latest no demuestra por sí solo que esté ejecutando la publicación más reciente.

Docker Hub almacena y distribuye la imagen para que el servidor no tenga que compilarla. **CI de construcción/publicación confirmado** por la última ejecución. Autodeploy había quedado habilitado en la configuración de Dokploy, pero un ciclo nuevo push → build → publicación → actualización remota es **NO VERIFICADO** en esta revisión; no se provocó un despliegue. Un webhook al push puede adelantarse a la publicación de latest, por lo que no debe afirmarse sincronización garantizada sin probarla.

## G. Dominio + HTTPS + seguridad.

| Comprobación actual | Resultado |
|---|---|
| DNS A proyectobd.duckdns.org | 167.172.121.38 |
| HTTPS `/health` | 200 OK, Healthy |
| HTTPS `/Account/Login` directo | 200 OK |
| Certificado, validación sin omitir errores | Válido para CN proyectobd.duckdns.org |
| Emisor | CN=YR1, O=Let's Encrypt, C=US |
| Vigencia observada | 18/11/2026 a 16/02/2027 |
| HTTPS `/` | 302 hacia **http://proyectobd.duckdns.org/Account/Login?ReturnUrl=%2F** |
| TCP público 1433 desde este equipo | No accesible |

En Domains de Dokploy el dominio oficial tiene HTTPS con `letsencrypt`. Traefik gestiona la terminación TLS y el mecanismo ACME de certificados. Una renovación futura efectivamente completada es **NO VERIFICADO**. El certificado público no es el autofirmado de Minikube.

Protecciones reales: cookies/Claims y BCrypt; autorización en Web y Negocio; POST antifalsificación; consultas parametrizadas; SQL sin puerto publicado; red Docker interna; secretos fuera de Git; usuario app sin root en Web; HTTPS público. Los flags efectivos de una cookie autenticada en producción son **NO VERIFICADO**: no se capturó una sesión nueva con contraseña.

Puertos: 443 responde HTTPS; 3000 corresponde al panel Dokploy accesible por HTTP; SSH 22 respondió con rechazo de autenticación; política completa de firewall y redirección en 80: **NO VERIFICADO**. El resultado externo de 1433 es compatible con Compose y UI sin publicación, pero no equivale a una auditoría completa de firewall.

**Fail2ban y jail sshd: NO VERIFICADO.** La documentación contiene recomendaciones (`backend=systemd`, maxretry 5, findtime 10 min, bantime 1 h); no demuestra que estén instaladas/activas. Fail2ban limita intentos repetidos de SSH, no reemplaza firewall, claves SSH o autorización Web. No afirmar que está protegiendo el servidor sin consultar su estado.

En remoto las Data Protection Keys persisten, pero no se configura el PFX local: no debe afirmarse cifrado de esas claves mediante certificado en producción. No se inspeccionó su contenido.

## H. Persistencia.

| Ambiente | Recurso | Conserva |
|---|---|---|
| Compose local | proyectobd-redes_sql-data → /var/opt/mssql | Archivos de SQL, esquema y datos |
| Compose local | proyectobd-redes_dp-keys → /var/dpkeys | Claves Data Protection |
| Dokploy | proyectobd-proyectobd-jsdy5d_sql-data → /var/opt/mssql | Base de datos remota |
| Dokploy | proyectobd-proyectobd-jsdy5d_dp-keys → /var/dpkeys | Claves de cookies de la Web remota |
| Minikube | PVC sql-data, 8 Gi | Archivos SQL del StatefulSet |
| Minikube | PVC dp-keys, 100 Mi | Claves compartidas por Web |

Recrear un contenedor/Pod sin borrar su volumen/PVC conserva esos archivos. Las claves permiten validar cookies tras recrear el proceso, junto con la misma configuración y nombre de aplicación Data Protection **ProyectoBD.Web**. Los PDF/reportes temporales guardados en memoria no sobreviven al proceso.

La ruta de los volúmenes remotos es `/var/lib/docker/volumes/<nombre>/_data`. El panel mostró aproximadamente 175.7 MB para SQL y 1 KB para dp-keys. Son cifras de interfaz, no tamaño lógico exacto de la base.

Los PV de Minikube usan almacenamiento hostpath, clase standard, política **Delete**. Borrar PVC/namespace puede destruir almacenamiento; borrar el nodo o su disco tampoco está cubierto. Un volumen persistente **no es una copia de seguridad** ni una réplica SQL.

## I. Estado actual de todos los componentes.

| Componente | Estado y alcance de verificación |
|---|---|
| Código y arquitectura | Inspeccionados; sin modificaciones |
| Compilación nueva de la solución | **NO VERIFICADO**; no ejecutada en auditoría de sólo lectura |
| Compilación anterior | `deploy/evidence/build.txt`: 0 errores, 0 advertencias, evidencia histórica |
| WinForms | Código presente y compartiendo capas; ejecución manual actual **NO VERIFICADO** |
| Compose local | Web/SQL healthy; inicializador Exited 0 |
| Kubernetes | Dos Web Ready, SQL Ready, Job Complete |
| Dokploy remoto | Web/SQL healthy; inicializador Exited 0 |
| Dominio y certificado público | DNS correcto, HTTPS /health y Login directo 200 |
| Raíz del sitio | Redirección insegura a HTTP detectada |
| Login con credenciales en esta revisión | **NO VERIFICADO**; el propietario lo confirmó antes; no se manejaron contraseñas |
| PEPS, pagos, PDF | Código y evidencias históricas revisados; nueva transacción de prueba **NO VERIFICADO** |
| GitHub Actions / Hub | Último build success y latest existente comprobados |
| Renovación de certificado | Mecanismo configurado; renovación futura **NO VERIFICADO** |
| Fail2ban, firewall completo, backups | **NO VERIFICADO** |

Evidencia histórica localizada: `deploy/evidence/compose.txt`, `health-compose.txt`, `kubernetes.txt`, `health-kubernetes.txt`, `ha-antes.txt`, `ha-despues.txt`, `sql-k8s-antes.txt`, `sql-k8s-despues.txt`, `sesion-pdf.txt`, `tls.txt`, `build.txt` e `integridad.txt`.

`ha-antes/despues` muestra sustitución de un Pod conservando el otro; no se repitió hoy. `sql-k8s-antes/despues` conserva 3 usuarios y 1 marcador. `sesion-pdf.txt` registra PDF de 40128 bytes y aceptación de cookie en la segunda réplica. `integridad.txt` registra comparación anterior de 121 archivos protegidos sin diferencias; no es una nueva certificación de integridad de esta revisión.

`Documentacion/ResultadoWebFinal.txt` documenta pruebas HTTP/SQL anteriores: PEPS consumiendo primero lote 71 y después 72; factura 1053 por Q40; otra 1055 por Q70; cobros Q10 efectivo + Q20 tarjeta + Q10 cheque; restauración de cantidades tras anulación. Usa datos temporales de aquella prueba, no un inventario actual. Hay artefactos PDF de respaldo en `Documentacion/EvidenciaWebFinal`. No se ejecutaron nuevamente scripts de integración porque alteran documentos/datos.

## J. Ubicación de los archivos importantes.

Raíz absoluta: `C:\Users\josue\source\repos\ProyectoBD2`. Todas las rutas siguientes son relativas a esa raíz.

| Ruta | Para qué consultarla |
|---|---|
| ProyectoBD2.sln | Solución de cinco proyectos |
| ProyectoBD.Entidades/ | Contratos y entidades compartidas |
| ProyectoBD.Datos/ConexionBD.cs | Variable de conexión y fallback LocalDB |
| ProyectoBD.Datos/ProcesoDatos.cs | Acceso SQL de los procesos |
| ProyectoBD.Negocio/ProcesoNegocio.cs | Reglas de procesos y permisos |
| ProyectoBD.Datos/ReporteDatos.cs | Consultas de reportes |
| ProyectoBD.Negocio/ReporteNegocio.cs | Reglas de reportes |
| ProyectoBD.Presentacion/ | WinForms terminado |
| ProyectoBD.Web/Program.cs | Pipeline, cookies, DP, proxies, health |
| ProyectoBD.Web/Controllers/ | Endpoints MVC y autorización |
| ProyectoBD.Web/Controllers/AccountController.cs | Acceso y salida |
| ProyectoBD.Web/Views/ | Vistas Razor |
| ProyectoBD.Web/Services/ReportesMemoria.cs | Cache de reportes por proceso |
| ProyectoBD.Web/Services/ReportePdf.cs | Construcción del PDF |
| Database/ | Scripts de evolución por fases |
| deploy/docker/sql/Proyecto_BD_Final.sql | SQL final usado por infraestructura |
| deploy/docker/Dockerfile | Build/publicación y runtime Linux |
| deploy/docker/compose.yaml | Compose local |
| deploy/docker/compose.dokploy.yaml | Compose remoto versionado |
| .dockerignore, .gitignore | Exclusiones de build y Git |
| .github/workflows/docker.yml | CI de imagen |
| deploy/kubernetes/ | Todos los manifiestos K8s |
| deploy/scripts/init-sql.sh | Inicialización segura de la base |
| deploy/scripts/prepare-local.ps1 | Preparación local |
| deploy/scripts/deploy-k8s.ps1 | Preparación/aplicación K8s |
| deploy/scripts/prepare-tls-demo.ps1 | TLS local |
| deploy/scripts/test-web.ps1 | Pruebas Web; no ejecutar sin revisar sus escrituras |
| deploy/README_DESPLIEGUE.md | Guía de despliegue |
| deploy/evidence/ | Evidencias anteriores de Redes |
| Documentacion/ResultadoWebFinal.txt | Pruebas funcionales anteriores |
| WEB_FINAL.md, REDES_DEPLOYMENT.md | Documentación anterior; algunas descripciones del estado remoto quedaron desactualizadas |

También existen herramientas `Verificacion`, `VerificacionWeb` y `Herramientas/DatosEjemplo`; no forman parte de los cinco proyectos de la solución principal.

## K. Lista de contenedores, Pods, Services, volúmenes y redes.

**Docker local de este proyecto:**

| Contenedor | Imagen/estado actual | Puertos |
|---|---|---|
| proyectobd-redes-web-1 | Config.Image proyectobd-web:latest; healthy, aprox. 7 h | 8085 host → 8080 |
| proyectobd-redes-sqlserver-1 | mssql/server:2022-latest; healthy, aprox. 7 h | 1433 interno |
| proyectobd-redes-sql-init-1 | Inicializador; Exited 0 | Ninguno público |
| minikube | kicbase v0.0.51; en ejecución | Nodo local de Kubernetes |

`docker ps` muestra para Web local el identificador de imagen anterior `5d4382d8a951`; su Config.Image sigue siendo proyectobd-web:latest. Un tag local que cambie no reemplaza por sí solo un contenedor existente.

Consumo instantáneo local observado: Web ~124.1 MiB/0.01 % CPU; SQL ~370.2 MiB/1.71 %; Minikube ~5.026 GiB de 6 GiB/40 %. Son lecturas puntuales y no deben compararse como mediciones de carga. Otros contenedores locales ajenos al proyecto no se manipularon.

**Remoto:**

| Contenedor | ID corto | Estado observado |
|---|---|---|
| proyectobd-proyectobd-jsdy5d-web-1 | 22b0621f417f | healthy, aprox. 3 h; josueg07/proyectobd-web:latest; 8080 interno |
| proyectobd-proyectobd-jsdy5d-sqlserver-1 | b51d255e6c6e | healthy, aprox. 4 h; SQL2022; sin puerto host |
| proyectobd-proyectobd-jsdy5d-sql-init-1 | d54070dba278 | Exited 0, aprox. 3 h |

**Pods actuales en namespace proyectobd:**

| Pod | Estado | IP |
|---|---|---|
| proyectobd-web-76b578855f-4zzrj | Running, 1/1, 0 reinicios | 10.244.0.24 |
| proyectobd-web-76b578855f-zjrzv | Running, 1/1, 0 reinicios | 10.244.0.25 |
| sqlserver-0 | Running, 1/1, 0 reinicios | 10.244.0.26 |
| sql-init-hds9w | Completed | 10.244.0.17 |

Service Web: proyectobd-web, 10.105.181.8:80 → 8080. Service SQL: sqlserver, headless:1433. Ingress: proyectobd-web, nginx, proyectobd.local, 192.168.49.2.

PV de SQL: `pvc-94361636-7c87-4fe7-97a2-b5231a733b6f`. PV de claves: `pvc-42fddb29-47bf-46fe-a2f4-ba3151baa278`. Ambos Bound, RWO, standard, política Delete.

Redes Docker locales relevantes: **proyectobd-redes_datos**, **proyectobd-redes_frente**, **minikube**, además de bridge/host/none. Volúmenes locales: **proyectobd-redes_sql-data**, **proyectobd-redes_dp-keys**, **minikube**.

Volúmenes remotos observados: **proyectobd-proyectobd-jsdy5d_sql-data**, **proyectobd-proyectobd-jsdy5d_dp-keys**, además de **dokploy** y **dokploy-postgres** para plataforma. Ese Postgres no sustituye SQL Server de la aplicación. El panel Networks mostró «No networks yet» en su catálogo de redes administradas: no demuestra ausencia de redes del motor. Nombres físicos de las redes remotas: **NO VERIFICADO**; datos/frente son los nombres lógicos del Compose.

## L. Comandos importantes.

Ejecutar desde la raíz del repositorio. Estos comandos son de consulta; adaptar nombres si la presentación ocurre después de recrear recursos.

```powershell
# Contexto y Docker local
git status --short
git log -1 --oneline
docker ps -a
docker stats --no-stream
docker compose -f deploy/docker/compose.yaml ps -a
docker logs --tail 50 proyectobd-redes-web-1
docker logs --tail 50 proyectobd-redes-sql-init-1
docker network ls
docker volume ls
docker inspect proyectobd-redes-web-1 --format '{{.Config.Image}} {{.Config.User}} {{.State.Health.Status}}'
docker inspect proyectobd-redes-sqlserver-1 --format '{{json .HostConfig.PortBindings}}'
docker inspect proyectobd-redes-sqlserver-1 --format '{{json .Mounts}}'

# Kubernetes
kubectl config current-context
minikube status
kubectl get pods -n proyectobd -o wide
kubectl get deployments,statefulsets,jobs -n proyectobd
kubectl get services,ingress,pvc -n proyectobd
kubectl get pv
kubectl get configmaps -n proyectobd
kubectl get secrets -n proyectobd
kubectl describe deployment proyectobd-web -n proyectobd
kubectl describe ingress proyectobd-web -n proyectobd
kubectl logs job/sql-init -n proyectobd --tail=50
kubectl get endpointslices -n proyectobd -l kubernetes.io/service-name=proyectobd-web

# DNS y sitio público: curl.exe evita el alias de PowerShell
Resolve-DnsName proyectobd.duckdns.org -Type A
curl.exe -i https://proyectobd.duckdns.org/health
curl.exe -I https://proyectobd.duckdns.org/
curl.exe -I https://proyectobd.duckdns.org/Account/Login
Test-NetConnection 167.172.121.38 -Port 1433
```

No mostrar `docker inspect` completo, `printenv`, `kubectl get secret -o yaml` ni renderizar Compose completo delante del público: pueden revelar valores. Revisar logs antes de proyectarlos; no mostrar cadenas de conexión ni cookies. Listar nombres/tipos de Secrets no decodifica sus contenidos.

Sólo si el propietario proporciona acceso SSH, consultas remotas útiles: `free -h`, `uptime`, `df -h`, `docker ps -a`, `docker stats --no-stream`, `sudo fail2ban-client status`, `sudo fail2ban-client status sshd`. No fueron ejecutadas satisfactoriamente en el servidor durante esta revisión.

## M. Pruebas recomendadas para hacer EN VIVO frente al ingeniero.

Las siguientes son propuestas, no acciones nuevas ejecutadas por esta auditoría. Priorizar las consultas. Para pruebas funcionales usar datos de demostración y escribir personalmente las credenciales.

| Prueba / comando | Qué observar antes | Qué debe ocurrir | Qué explicar oralmente |
|---|---|---|---|
| `docker compose -f deploy/docker/compose.yaml ps -a` | Tres servicios locales | Web/SQL healthy, init Exited 0 | «El inicializador termina; no debe quedar como daemon». |
| `docker stats --no-stream` | Identificar contenedores del proyecto | CPU/RAM por contenedor | «Los recursos se miden; un pico no demuestra una falla». |
| `kubectl get deployment proyectobd-web -n proyectobd` y `kubectl get pods -n proyectobd -o wide` | Contexto minikube | Dos Web 1/1 Ready | «Deployment mantiene dos instancias de la aplicación». |
| `kubectl get services -n proyectobd` y comando EndpointSlices de L | Dos Pods Web listos | IP estable del Service y endpoints de Pods | «Los clientes usan Service, no dependen de la IP efímera de un Pod». |
| `kubectl describe ingress proyectobd-web -n proyectobd` | Host proyectobd.local | Reglas host/ruta/servicio y TLS | «Ingress NGINX enruta HTTP/HTTPS dentro de Minikube». |
| `kubectl get pvc -n proyectobd` y `kubectl get pv` | SQL y claves Bound | Volúmenes separados de la vida del Pod | «Persistencia no equivale a backup ni réplica SQL». |
| `kubectl describe deployment proyectobd-web -n proyectobd` | Deployment disponible | Probes y recursos configurados | «Readiness controla tráfico; liveness reinicia contenedor; startup da tiempo al arranque». |
| `Resolve-DnsName proyectobd.duckdns.org -Type A` y `curl.exe -i https://proyectobd.duckdns.org/health` | Conectividad Internet | IP correcta y Healthy por HTTPS | «DuckDNS resuelve; Traefik termina TLS; Web atiende». |
| Abrir `https://proyectobd.duckdns.org/Account/Login` | Certificado válido | Formulario HTTPS; propietario inicia sesión | «La Web reutiliza Negocio/Datos y aplica roles». Evitar la raíz hasta corregir la redirección. |
| Ver factura, asignación PEPS, cobro y PDF desde la UI | Documentos conocidos y rol adecuado | Importes/lotes/pagos coherentes y PDF | «La transacción SQL conserva integridad; PEPS consume primero lo más antiguo». Una operación nueva escribe datos. |
| Abrir Actions y Dokploy | Último workflow y tres servicios visibles | Build success; contenedores healthy | «Actions publica imagen; Dokploy despliega; son etapas distintas». |

**Recuperación Web opcional, altera sólo el entorno local y requiere decidirlo antes de la presentación:**

```powershell
kubectl get pods -n proyectobd -l app=proyectobd-web -o wide
# Sustituir por UN nombre real obtenido arriba; no copiar nombres antiguos.
kubectl delete pod NOMBRE_REAL_DE_UN_POD_WEB -n proyectobd
kubectl get pods -n proyectobd -w
```

Antes: dos Web Ready. Después: el Deployment crea otro Pod con nombre/IP nuevos y vuelve a 2/2. Explicación: «El estado deseado mantiene réplicas; el Service retira endpoints no listos». Puede interrumpirse una petición o un reporte temporal del Pod borrado. **No se ejecutó durante la auditoría. No hacerlo contra producción.**

**Persistencia SQL opcional:** antes, anotar un registro existente desde la UI y `kubectl get pvc -n proyectobd`. Sólo en Minikube y aceptando interrupción, `kubectl delete pod sqlserver-0 -n proyectobd`; observar su recreación y consultar el mismo registro después. Explicar StatefulSet + mismo PVC. Produce indisponibilidad temporal porque SQL tiene una réplica. No borrar PVC, volúmenes o namespace. La evidencia histórica permite explicarlo sin repetir el riesgo en vivo.

## N. Orden recomendado para realizar la demostración.

1. En un minuto, presentar el problema y las dos interfaces con capas compartidas.
2. Mostrar Login directo HTTPS y un flujo de aplicación conocido; no empezar creando datos desde cero si el tiempo es corto.
3. Explicar PEPS con un documento ya preparado; mostrar detalle, cobro y PDF.
4. Mostrar el Dockerfile y resumir build/runtime, app sin root, 8080 y health check.
5. Mostrar Compose local: tres servicios, init terminado, volumen SQL y ausencia de publicación 1433.
6. Pasar a Minikube: dos réplicas, Deployment, Service, Ingress, PVC y probes.
7. Si fue preparado, demostrar recuperación de un Pod Web local; esperar nuevamente 2/2 antes de seguir. O utilizar evidencia anterior.
8. Mostrar GitHub Actions exitoso y la imagen correcta de Docker Hub.
9. Mostrar Dokploy remoto, servicio Web, dominio/puerto, DNS y `/health` HTTPS.
10. Cerrar con límites reales: un nodo, un SQL, persistencia sin backup, Fail2ban pendiente de verificación y redirección HTTPS pendiente de corregir.

Mantener diferenciados los terminales local/remoto y Compose/Minikube. No afirmar que las dos Web de Minikube son las del dominio público: ese dominio tiene una Web en Dokploy.

## O. Posibles preguntas del ingeniero y respuestas.

1. **¿Cómo se organiza el programa?** Web o WinForms llama a Negocio; Negocio llama a Datos y Datos a SQL Server; Entidades se comparte.
2. **¿Usa Entity Framework?** No. Usa ADO.NET, Microsoft.Data.SqlClient y procedimientos almacenados.
3. **¿Cómo funciona PEPS?** Ordena lotes por fecha de ingreso e ID, consume los más antiguos y registra las cantidades por lote dentro de una transacción.
4. **¿Qué diferencia hay entre imagen y contenedor?** La imagen es el artefacto distribuido; el contenedor es una instancia ejecutándose con esa imagen.
5. **¿Para qué Docker Compose?** Define conjuntamente SQL, inicialización, Web, redes, volúmenes y dependencias.
6. **¿Por qué sql-init está Exited 0?** Terminó correctamente su trabajo de inicialización; no necesita seguir ejecutándose.
7. **¿Cómo evita importar SQL repetidamente?** Comprueba existencia de la base y el marcador de inicialización; si ya está inicializada, omite la carga.
8. **¿Dónde se guarda la base?** En /var/opt/mssql respaldado por sql-data; en Kubernetes por un PVC de 8 Gi.
9. **¿Qué sucede si elimino el contenedor SQL?** Al recrearlo con el mismo volumen conserva archivos; habrá interrupción mientras vuelve a arrancar.
10. **¿Qué es un Pod aquí?** La unidad Kubernetes que ejecuta la Web o SQL; sus IP y nombre pueden cambiar al recrearse.
11. **¿Por qué dos réplicas Web?** Permiten distribuir conexiones y mantener otra instancia mientras se recupera una, dentro del único nodo disponible.
12. **¿Qué hace un Deployment?** Mantiene el número deseado de Pods Web y administra sus reemplazos/actualizaciones.
13. **¿Qué hace el StatefulSet?** Mantiene identidad estable de sqlserver-0 y su asociación con almacenamiento persistente.
14. **¿Qué hace el Service?** Da un punto de acceso estable a los Pods listos; Web publica 80 hacia el puerto 8080.
15. **¿Qué hace Ingress?** En Minikube enruta el host proyectobd.local hacia el Service Web usando NGINX y TLS local.
16. **¿Qué es un PVC?** Una solicitud de almacenamiento persistente; aquí hay una para SQL y otra para las claves Data Protection.
17. **¿Qué pasa al eliminar un Pod Web?** Deployment crea otro; Service deja de enviar tráfico al que ya no está listo. La memoria del proceso se pierde.
18. **¿La aplicación tiene alta disponibilidad completa?** No. Hay dos Web locales, pero un nodo y un SQL. El servidor remoto también es único.
19. **¿Para qué readiness y liveness?** Readiness decide si recibe tráfico; liveness detecta un proceso atascado para reiniciarlo; startup protege el arranque.
20. **¿Cómo funcionan cookies con dos Web?** Comparten claves Data Protection y nombre de aplicación; además existe afinidad para reportes guardados en memoria.
21. **¿Para qué GitHub Actions y Docker Hub?** Actions construye/publica; Hub almacena la imagen que Dokploy descarga. El último build fue exitoso.
22. **¿Qué hace Dokploy y qué hace Traefik?** Dokploy administra el despliegue; Traefik recibe tráfico por dominio y termina HTTPS hacia Web:8080.
23. **¿Cómo intervienen DuckDNS y Let's Encrypt?** DuckDNS resuelve el dominio a la IP; Let's Encrypt emite el certificado gestionado por Traefik.
24. **¿SQL está expuesto a Internet?** Compose no publica 1433 y la prueba externa falló; la aplicación lo usa internamente. No se auditó todo el firewall.
25. **¿Está todo automatizado y protegido?** El build/publicación está comprobado. Un nuevo ciclo completo de CD, backups y Fail2ban activo son NO VERIFICADO; además hay una redirección HTTP pendiente.

## P. Problemas, pendientes o limitaciones reales que detectes.

1. **Prioridad: la raíz HTTPS redirige a HTTP.** Respuesta observada: 302, `Location: http://proyectobd.duckdns.org/Account/Login?ReturnUrl=%2F`. `/health` y Login directo por HTTPS funcionan. El reconocimiento de proxy/esquema reenviado es una causa probable, pero la causa exacta y las variables efectivas son **NO VERIFICADO**. No se modificó Program.cs, Traefik ni Environment. Requiere autorización para corregir y volver a verificar.
2. **Imagen versionada distinta de la real:** `compose.dokploy.yaml` contiene josueg077; Dokploy funciona con parche josueg07. Un despliegue limpio sin parche puede volver a fallar en el pull. No se cambió el archivo.
3. **Fail2ban y jail sshd NO VERIFICADO.** Hay documentación, no prueba actual de servicio activo. SSH no permitió ejecutar consultas.
4. **Backups y restauración remota NO VERIFICADO.** Los volúmenes conservan datos, pero no cubren pérdida del servidor/disco ni borrado de volumen.
5. **Alta disponibilidad limitada:** Minikube de un nodo, SQL único; en remoto una Web/SQL en un servidor. No hay SQL replicado ni tolerancia a pérdida del servidor.
6. **Reportes en memoria:** afinidad reduce cambios de réplica, pero no replica el cache; perder un Pod puede requerir generar el reporte otra vez.
7. **Health check básico:** Healthy no confirma operaciones SQL ni login. SQL TCP readiness tampoco valida una consulta de negocio.
8. **Claves remotas sin el PFX local:** persistentes, pero protección por certificado en reposo no configurada en el Compose remoto. No se leyeron claves.
9. **latest mutable:** digest ejecutado en producción NO VERIFICADO. Dos despliegues de latest pueden resolver a imágenes distintas; existe tag por SHA para identificar versiones.
10. **CD completo NO VERIFICADO:** CI exitoso, pero no se probó ahora un push nuevo y actualización automática; hay posible carrera entre webhook Git y publicación de la imagen.
11. **Exposición administrativa:** Dokploy disponible por HTTP :3000 y dominio temporal HTTP conservado. No se evaluaron restricciones de acceso, reglas completas de firewall o endurecimiento SSH.
12. **Documentación anterior parcialmente desactualizada:** WEB_FINAL.md y REDES_DEPLOYMENT.md incluyen estados previos al despliegue remoto; usar el estado actual de este informe y distinguir evidencia histórica.
13. **Alcance de verificación:** sin compilación nueva, sin transacciones nuevas de facturación/pagos y sin prueba nueva de recuperación. Las evidencias anteriores se identificaron como históricas. No se verificó renovación futura TLS, SO por SSH, inventario completo de servicios del host ni redes físicas remotas.

La presentación puede apoyarse en los servicios saludables, DNS/HTTPS comprobados, dos réplicas locales y CI exitoso, explicando con claridad estas limitaciones. Este informe no realizó ninguna corrección ni cambio de infraestructura.
