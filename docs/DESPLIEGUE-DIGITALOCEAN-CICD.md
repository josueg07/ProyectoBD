# ProyectoBD: HTTPS y CI/CD reproducible en DigitalOcean

Revisión e implementación local: **18/11/2026, America/Guatemala**. Rama de trabajo: `fix/dokploy-https-cicd`.

**Estado: CI/CD productivo validado y corrección de auditoría publicada automáticamente.** Los resultados de preparación se conservan abajo y los resultados reales están en las secciones 13 y 14. No se tocó SQL, contraseñas, volúmenes existentes ni lógica de ventas. La revisión visual autenticada de la auditoría nueva y la interacción manual de WinForms quedan NO VERIFICADO hasta completar el acceso manual.

## 1. Problema original y causa comprobada

Kestrel recibe HTTP desde Traefik, aunque el navegador use HTTPS. Debe aceptar `X-Forwarded-Proto` sólo de un proxy confiable antes de crear redirecciones, cookies y HSTS. Sin esa información, el desafío de autenticación podía generar `Location: http://proyectobd.duckdns.org/Account/Login?...`.

**La pérdida de la variable tiene una causa comprobada en la UI de Dokploy:** en Patches existe un parche **update, habilitado**, para `deploy/docker/compose.dokploy.yaml`. Su contenido es una copia completa antigua del Compose; el servicio web contiene DATA_PROTECTION_KEYS_PATH pero no TRUSTED_PROXY_NETWORKS. Se inspeccionó el editor hasta el final y se cerró con Cancel, sin guardar. No es necesario suponer que Traefik elimina variables: el parche sustituye el archivo descargado antes del procesamiento de dominios.

El código de Dokploy v0.31.4 también confirma ese orden: clonar → aplicar parches → preparar dominios/Compose → ejecutar Docker. Una modificación manual en `/etc/dokploy/compose/proyectobd-proyectobd-jsdy5d/code/…` no es fuente de verdad y puede perderse.

En esta revisión la corrección manual existente aún funciona: raíz devuelve 302 hacia **HTTPS** y /health responde Healthy. Esto corresponde a la versión anterior, no al código nuevo de esta rama.

## 2. Configuración real observada

| Dato | Valor |
|---|---|
| Servidor | 167.172.121.38; Dokploy v0.31.4 |
| Proyecto / Environment | ProyectoBD / production |
| Compose ID | p62O-zj8TR52M7xPQzKeH |
| App name / proyecto Compose | proyectobd-proyectobd-jsdy5d |
| Repositorio actual / rama actual | josueg07/ProyectoBD / main |
| Compose Path | deploy/docker/compose.dokploy.yaml |
| Trigger actual | On Push, **Autodeploy desactivado** |
| Comando actual Advanced | Vacío; opción Pull latest images habilitada |
| Web actual | proyectobd-proyectobd-jsdy5d-web-1; josueg07/proyectobd-web:latest; healthy |
| SQL actual | proyectobd-proyectobd-jsdy5d-sqlserver-1; healthy |
| Inicializador actual | proyectobd-proyectobd-jsdy5d-sql-init-1; Exited 0 |
| Dominio | https://proyectobd.duckdns.org; web:8080, Let's Encrypt |

Redes de Web observadas en UI: dokploy-network **10.0.1.16/24**, datos **172.20.0.3/16**, default **172.21.0.2/16**, frente **172.19.0.2/16**. No se modificaron.

El propietario ejecutó y devolvió las consultas de red solicitadas:

```text
10.0.1.0/24
dokploy-postgres.1.e6dqs5d719ddtdkle1j9xg30g=10.0.1.3/24
dokploy-traefik=10.0.1.7/24
proyectobd-proyectobd-jsdy5d-web-1=10.0.1.16/24
dokploy.1.udqrdfsq7jnyg9qh42p7m5atq=10.0.1.6/24
dokploy-network-endpoint=10.0.1.4/24
```

Desde el contenedor Web, `getent ahostsv4 dokploy-traefik` resolvió **10.0.1.7**, según la salida proporcionada por el propietario. Por tanto, la red no es exclusiva de Traefik/Web. **No se concede confianza a toda esa subred.** SSH directo desde la estación siguió rechazando autenticación; estas dos consultas remotas se realizaron por el propietario.

## 3. Archivos modificados

- `ProyectoBD.Web/Program.cs`: infraestructura de encabezados antes de HSTS/autenticación; health interno accesible sin redirección TLS.
- `ProyectoBD.Web/Infrastructure/TrustedProxyForwardingMiddleware.cs`: resolución del proxy y aceptación restringida de encabezados.
- `deploy/docker/compose.dokploy.yaml`: nombre DNS del proxy, restricción CIDR y puerto público de redirección HTTPS.
- `deploy/docker/Dockerfile`: marcador público `/deployment.json` generado desde SOURCE_REVISION.
- `.github/workflows/docker.yml`: verificaciones, build/push, publicación posterior de rama de release y verificación pública.
- `deploy/scripts/publish-release.py`: genera snapshot con imagen por digest, preservando historial sin force push.
- `deploy/scripts/verify-public.py`: HTTPS, health, Login, encabezados falsificados y revisión servida.
- `Pruebas/Infraestructura/Infraestructura.csproj` y `Program.cs`: pruebas de confianza del proxy sin SQL.
- `Pruebas/Despliegue/test_release.py`: pruebas Git con repositorio remoto temporal.
- `Pruebas/Despliegue/docker-proxy-smoke.py`: pruebas reales con imagen y contenedores aislados.
- `.gitignore`: excluye cache Python.
- Este documento.

Para la prueba visible de activación se añadió ` - Redes II` al pie existente de `ProyectoBD.Web/Views/Shared/_Layout.cshtml`, sin cambiar flujos.

`deploy/docker/compose.yaml`, SQL, Datos, Negocio, Entidades, Controllers y funcionalidades de negocio se conservan. La corrección visual adicional de WinForms solicitada posteriormente se describe en la sección 14; no cambia sus procesos. Program.cs mantiene su codificación original; no se convirtió todo el archivo.

## 4. Solución permanente de HTTPS

La solución consta de **dos partes inseparables**: retirar el parche antiguo de Dokploy y usar la configuración del repositorio; restringir la confianza al proxy real sin fijar su IP.

Compose define:

```yaml
TRUSTED_PROXY_HOSTS: ${DOKPLOY_PROXY_HOSTS:-dokploy-traefik}
TRUSTED_PROXY_NETWORKS: ${DOKPLOY_PROXY_NETWORKS:-10.0.1.0/24}
ASPNETCORE_HTTPS_PORT: "443"
```

Con HOSTS configurado, NETWORKS **sólo restringe las respuestas DNS**: el middleware resuelve dokploy-traefik, filtra direcciones dentro del CIDR y utiliza únicamente esas direcciones exactas. No agrega el CIDR completo a KnownNetworks en este modo. No llama a KnownNetworks.Clear(), no acepta cualquier proxy y no habilita ASPNETCORE_FORWARDEDHEADERS_ENABLED.

La lista se renueva cada **30 segundos**. Se procesa **un salto**, X-Forwarded-For y X-Forwarded-Proto; no X-Forwarded-Host. Un contenedor distinto, incluso en 10.0.1.0/24, no puede declarar que es Traefik mediante un encabezado. Si DNS falla o no produce una IP válida, se devuelve 503; un peer no autorizado recibe 400. Así no se genera un Login HTTP durante ese fallo. Puede haber una ventana de indisponibilidad de hasta el refresco cuando cambia la IP del proxy.

Las sondas GET/HEAD `/health` internas siguen accesibles sin confiar en encabezados y sin redirigir a un puerto TLS de Kestrel inexistente. No exponen datos de negocio. El resto de la aplicación conserva redirección HTTPS. HSTS se evalúa después de restaurar el esquema del proxy. Autenticación y autorización no se sustituyeron.

Compatibilidad local/Minikube: cuando HOSTS no está configurado se conserva el modo CIDR explícito anterior, con defaults restrictivos de ASP.NET Core. No configurar HOSTS en Kubernetes sin definir su DNS y CIDR reales. El modo Dokploy debe tener ambas variables; no eliminar HOSTS dejando una subred compartida confiable.

**Límite de seguridad:** se confía en Docker DNS y en el control administrativo del daemon/red. Un atacante con acceso al socket Docker o permiso para reemplazar el contenedor/alias Traefik ya controla infraestructura crítica. No montar el socket en Web, no permitir contenedores de terceros sin revisar y no duplicar el alias dokploy-traefik. La caché puede retener una IP anterior hasta 30 s; la protección no equivale a mTLS entre proxy y Web.

Traefik debe sobrescribir/normalizar los encabezados de clientes y no tener `forwardedHeaders.insecure=true`. Las pruebas externas actuales con X-Forwarded-Proto=http y host falsificado conservaron HTTPS. El archivo estático de Traefik se inspeccionó durante la activación; ver sección 13. No se modificó ni se abrió su almacenamiento de certificados. No confiar en encabezados externos mediante una regla global de Traefik.

Si cambia la infraestructura: consultar nombre/IPAM de la red y resolución desde Web; actualizar HOSTS y el CIDR restringido en configuración versionada/Environment; revisar miembros; desplegar sólo Web y ejecutar las pruebas. No actualizar una /32 después de cada recreación. **Parámetros de la plantilla:** DOKPLOY_PROXY_HOSTS y DOKPLOY_PROXY_NETWORKS, opcionales con defaults versionados dokploy-traefik y 10.0.1.0/24. Las variables que recibe .NET siguen llamándose TRUSTED_PROXY_HOSTS y TRUSTED_PROXY_NETWORKS. Separar ambos nombres evita que la antigua /32 guardada en Dokploy prevalezca accidentalmente; se puede retirar ese valor obsoleto sin afectar la plantilla nueva. Si cambia la red, ajustar DOKPLOY_PROXY_NETWORKS al CIDR real después de revisar DNS y miembros.

## 5. CI/CD sin la carrera del push inicial

Flujo diseñado:

```text
push a main
  → pruebas de infraestructura y compilación
  → construir y publicar en Docker Hub (latest y SHA)
  → obtener digest publicado
  → crear commit en deploy/production con snapshot del código
    y web.image=josueg07/proyectobd-web@sha256:...
  → GitHub App notifica el push de deploy/production a Dokploy
  → Dokploy descarga esa rama y actualiza exclusivamente web
  → CI comprueba /deployment.json, HTTPS, Login y /health públicos
```

**main continúa siendo la rama de desarrollo principal.** Dokploy seguirá la rama generada **deploy/production**, no main. Esa rama sólo se mueve después de que Docker Hub confirma el push. Es el disparador posterior a publicación; no se necesita guardar un token nuevo de Dokploy ni su webhook secreto en Git.

El script crea un árbol completo de la revisión construida, cambia únicamente la imagen Web a digest y añade `deploy/release.json` con revisión/imagen. Usa índice Git temporal; no altera el checkout. Los commits conservan el padre de la release anterior y el commit fuente; push normal, sin force. Si la revisión construida ya no es HEAD de main, omite su release para no publicar una versión atrasada. Los workflows de la misma referencia se serializan sin cancelar uno que está publicando.

La imagen productiva se obtiene por **digest**, no por el valor mutable de latest. Si otro push cambia latest antes de que Dokploy termine, el Compose de release sigue solicitando el artefacto exacto. El snapshot de SQL/script también corresponde a ese código, aunque las actualizaciones normales no ejecutarán sql-init.

La integración GitHub App instalada recibió el push de la rama de release y produjo el despliegue automático en la validación autorizada de la sección 13. Un push con GITHUB_TOKEN no inicia otros workflows de Actions; aquí el receptor es **Dokploy/GitHub App**, no un segundo workflow. Si la política de la organización impide el push o la entrega a la App, no dar el flujo por aceptado: revisar permisos/entregas y usar una App de CI autorizada o API postpublicación bajo aprobación, sin añadir un deploy temprano sobre main.

Secrets existentes: **REGISTRY_USERNAME**, **REGISTRY_PASSWORD**, únicamente para Docker Hub. El usuario necesita permisos de escritura sobre josueg07/proyectobd-web. GITHUB_TOKEN se emite por ejecución; `contents: write` se concede sólo al job release. La variable no secreta **DOKPLOY_CD_ENABLED=true** habilita release; sin ella sólo se construye/publica. No se configuró ni habilitó durante esta preparación.

## 6. Activación única en Dokploy y GitHub — requiere autorización

No activar Autodeploy todavía sobre main. No presionar Deploy antes de preparar y revisar todo este procedimiento.

1. Revisar/mergear la rama fix a main por el mecanismo habitual. Con DOKPLOY_CD_ENABLED ausente/false, se ejecutan pruebas y publicación, sin actualización productiva automática.
2. Registrar imagen/digest actual, ID/estado de SQL y nombres de volúmenes. El respaldo `Proyecto_BD_pre_despliegue.bak`, fuera del contenedor y con copia local, fue confirmado por el propietario; no se abrió, copió ni restauró aquí.
3. En Patches, **deshabilitar** el update de compose.dokploy.yaml, conservándolo como referencia de reversión. Es configuración persistente de Dokploy, no editar su copia interna. No deshabilitar parches de otros servicios.
4. En Environment conservar MSSQL_SA_PASSWORD y todos los valores existentes. La plantilla usa los defaults verificados; sólo si cambia la infraestructura establecer DOKPLOY_PROXY_NETWORKS y DOKPLOY_PROXY_HOSTS con sus valores reales. Los antiguos TRUSTED_PROXY_* en Environment ya no son parámetros de interpolación del Compose nuevo. No sobrescribir todo Environment ni mostrar contraseñas.
5. En Advanced → Run Command usar exactamente este comando, **sin el prefijo docker**, porque Dokploy lo agrega:

```text
compose -p proyectobd-proyectobd-jsdy5d --env-file deploy/docker/.env -f deploy/docker/compose.dokploy.yaml up -d --no-deps --pull always --wait --wait-timeout 180 web
```

Es el mismo nombre de proyecto existente, para conservar el volumen de claves y la red. `--no-deps` y el objetivo `web` evitan recrear, actualizar o ejecutar sqlserver/sql-init. No utilizar --remove-orphans, --force-recreate global, down, down -v ni Fresh Volumes. La opción Pull latest no se aplica a comandos personalizados; por eso el comando incluye --pull always para Web. La imagen por digest no cambia aunque latest avance.

6. Obtener una primera rama deploy/production mediante workflow_dispatch sobre main, con **prepare_only=true**, manteniendo Autodeploy apagado y DOKPLOY_CD_ENABLED ausente/false. Este modo explícito de bootstrap construye/publica y prepara la rama, pero omite esperar un despliegue público todavía no configurado. No presentarlo como una actualización productiva completada. Usar prepare_only=false para todas las verificaciones finales y posteriores.
7. Cambiar Branch de Dokploy a **deploy/production**, conservar repositorio/Compose Path y On Push. Mantener Watch Paths vacío salvo que se hayan revisado para incluir los commits de release. Guardar.
8. Habilitar Autodeploy y provocar el siguiente workflow de main (workflow_dispatch o push autorizado). Ahora el push de release ocurre después del build y dispara Dokploy automáticamente. Evitar iniciar además un Deploy manual duplicado.
9. Revisar Deployments, imagen por digest, revisión pública, SQL y mounts. La primera actualización Web puede interrumpir solicitudes durante su recreación; requiere autorización explícita. Este Compose tiene una sola Web.

Las opciones/etiquetas Traefik del dominio siguen administradas por Domains; no se cambian DNS ni certificados. Si la integración no permite seleccionar una rama recién creada, refrescar sus ramas antes de seleccionarla.

## 7. Actualización normal segura

Después de la configuración inicial autorizada: editar código visible → commit/push main → Actions verifica y publica → release por digest → Dokploy automático. No editar `/etc/dokploy/…/code` ni el Compose a mano en cada deploy. El resultado debe verse en la UI y en `/deployment.json` con el SHA esperado.

Si el build falla, no se publica release. Si falla el push de release, no forzar: revisar conflicto/permisos. Si falla la verificación pública, Actions queda rojo; no se declara éxito porque el webhook haya aceptado el evento. El marcador confirma revisión, pero no reemplaza revisar que Dokploy usó el digest fijado y que SQL sigue saludable.

## 8. Reversión

Antes de la primera actualización capturar una referencia inmutable de la imagen vigente y su configuración no secreta. Guardar valores secretos sólo en su gestor/Environment, nunca en logs o commits.

Si falla una release: deshabilitar temporalmente el job release mediante DOKPLOY_CD_ENABLED=false y Autodeploy para no competir con la recuperación. Revisar logs Web sin imprimir conexión/contraseñas. SQL no se restaura ni se reinicializa por un fallo de Web.

Una vez existan releases buenas, crear un **nuevo commit** en deploy/production con el árbol de una release buena (incluido Compose por digest y release.json), manteniendo como padre la release actual; push normal, sin reset/force. Después reactivar Autodeploy y comprobar la recuperación. También puede utilizarse el rollback de Dokploy únicamente tras verificar que conserva mounts y actualiza sólo Web. No se probó rollback remoto en esta preparación.

En el primer despliegue todavía no hay una release anterior de este mecanismo: volver al digest y configuración capturados previamente. La imagen anterior sólo entiende TRUSTED_PROXY_NETWORKS; si se usa, restaurar su /32 temporal correcta en Environment bajo revisión, en lugar de asumir que implementa el nuevo modo DNS. Ese rollback es recuperación excepcional, no la solución permanente. Nunca restaurar un .bak automáticamente ni tocar volúmenes para revertir código.

## 9. Comprobaciones de aceptación

Desde Windows en la raíz del repositorio:

```powershell
curl.exe -I https://proyectobd.duckdns.org/
curl.exe -i https://proyectobd.duckdns.org/health
python deploy/scripts/verify-public.py --base-url https://proyectobd.duckdns.org --revision SHA_COMPLETO_ESPERADO --attempts 60
```

La raíz debe responder 302 con Login **https://** y host oficial. Login directo y /health deben responder 200; TLS se valida sin -k. verify-public envía también encabezados Proto/Host/For falsificados: no deben cambiar el destino a HTTP ni evil.invalid.

En la consola Linux del propietario, sin mostrar secretos:

```sh
docker ps -a --filter name=proyectobd-proyectobd-jsdy5d
docker inspect proyectobd-proyectobd-jsdy5d-sqlserver-1 --format '{{.Id}} {{.State.Health.Status}} {{range .Mounts}}{{.Name}}:{{.Destination}} {{end}}'
docker inspect proyectobd-proyectobd-jsdy5d-web-1 --format '{{.Config.Image}} {{range .Mounts}}{{.Name}}:{{.Destination}} {{end}}'
docker exec proyectobd-proyectobd-jsdy5d-web-1 getent ahostsv4 dokploy-traefik
docker exec proyectobd-proyectobd-jsdy5d-web-1 printenv TRUSTED_PROXY_HOSTS TRUSTED_PROXY_NETWORKS ASPNETCORE_HTTPS_PORT
docker inspect proyectobd-proyectobd-jsdy5d-sqlserver-1 --format '{{json .HostConfig.PortBindings}}'
```

El ID SQL debe permanecer igual durante una actualización sólo Web; SQL healthy, init Exited 0, mismos volúmenes SQL/DP, sin puerto host 1433. Inspeccionar sólo las tres variables no secretas indicadas, no todo Env. Acceder con credenciales personalmente y consultar un registro existente para verificar SQL, sin transacciones de prueba innecesarias.

La prueba remota directa de un peer no confiable devolvió 400 con encabezado HTTPS falsificado; la recreación Web ocurrió durante el despliegue automático. Los resultados de activación se registran en la sección 13. Las pruebas aisladas también se ejecutaron. Revisar también el archivo estático de Traefik para confirmar que no confía indiscriminadamente en encabezados recibidos de Internet.

## 10. Secuencia corta para el video

1. Mostrar la página y `deployment.json` antes del cambio. Estar autenticado sin grabar la contraseña.
2. En `ProyectoBD.Web/Views/Home/Index.cshtml`, cambiar un texto visible existente, por ejemplo `Redes II` por `Redes II - entrega CI/CD`. No tocar reglas de negocio.
3. En main actualizado, ejecutar:

```powershell
git add ProyectoBD.Web/Views/Home/Index.cshtml
git commit -m "Mostrar actualización de Redes II por CI/CD"
git push origin main
git rev-parse HEAD
```

4. Mostrar Actions: verify → publish → release. En Hub enseñar el tag SHA/digest, sin abrir Secrets.
5. Mostrar la actualización automática de deploy/production y el despliegue de Dokploy, sin presionar Deploy.
6. Recargar la página para ver el texto; comparar `/deployment.json` con el SHA de main.
7. Ejecutar verify-public con ese SHA y enseñar SQL healthy y los mismos mounts. Mostrar que Location sigue siendo HTTPS.

Grabar después de completar el bootstrap y una validación autorizada, no durante la primera configuración de secretos o variables.

## 11. Pruebas de preparación realmente ejecutadas

Esta tabla conserva el estado previo a la autorización; los resultados productivos posteriores están en la sección 13.

| Prueba | Resultado |
|---|---|
| Compilación inicial de ProyectoBD2.sln | 0 errores / 0 advertencias |
| Pruebas de middleware .NET | 15 comprobaciones aprobadas: proxy exacto, peers externos/misma red/loopback, IPv4 mapeada, un salto, renovación/revocación IP, DNS fuera de rango/fallido/recuperado, CIDR inválidos |
| Python unittest de release | 3 pruebas aprobadas; dos releases fast-forward en remoto Git temporal, snapshot correcto y rechazo de release antigua/imagen mutable |
| Docker Compose config --quiet | Correcto con contraseña ficticia de validación; servicios sqlserver/sql-init/web, volúmenes dp-keys/sql-data |
| Docker build de imagen local | Correcto, proyectobd-web:https-cicd-test, revision=cicd-local-test |
| Prueba real de imagen en Docker aislado | Cookie challenge HTTPS, HSTS, rechazo de otro contenedor, health interno, marcador y recreación sólo Web aprobados |
| actionlint del workflow | Sin errores |
| Verificación pública de versión anterior | HTTPS raíz/Login/health y resistencia a encabezados falsificados aprobadas; **no demuestra la release nueva** |
| Estado remoto en UI | Web/SQL healthy; init Exited 0; Autodeploy off; parche antiguo identificado |
| Nuevo build/publicación GitHub/Hub, entrega automática, revisión pública nueva | **PENDIENTE DE AUTORIZACIÓN / NO VERIFICADO** |
| Nuevo login manual, persistencia y rollback remoto tras actualización | **NO VERIFICADO** |

La prueba Docker inicial detectó que configurar HTTPS_PORT hacía redirigir el health interno. Se corrigió la excepción restringida de /health y se reconstruyó la imagen; la repetición completa pasó. Los contenedores/redes efímeros creados por la prueba se eliminaron comprobando su etiqueta propia; no hubo SQL ni volúmenes existentes involucrados.

## 12. Referencias y pendientes para cerrar la aceptación

Referencias primarias consultadas:

- [Dokploy Compose y variables](https://docs.dokploy.com/docs/core/docker-compose).
- [Dokploy Auto Deploy](https://docs.dokploy.com/docs/core/auto-deploy).
- [Orden de parches en Dokploy v0.31.4](https://github.com/Dokploy/dokploy/blob/v0.31.4/packages/server/src/services/compose.ts).
- [Comando Compose de Dokploy v0.31.4](https://github.com/Dokploy/dokploy/blob/v0.31.4/packages/server/src/utils/builders/compose.ts).
- [ASP.NET Core y proxies](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-8.0).

La integración y activación se autorizaron y ejecutaron; ver la sección 13 para evidencia real y las comprobaciones restantes. La guía de la sección 6 sirve para reproducir la configuración, no para repetirla después de cada push. No se necesita un token nuevo de Dokploy para el diseño elegido. Si GitHub requiere iniciar sesión o modificar Secrets, debe hacerlo el propietario, sin compartirlos en el chat.

**No declarar cerrado el criterio de aceptación hasta que el cambio visible llegue automáticamente a la URL pública y HTTPS, SQL y los volúmenes se verifiquen después.**

## 13. Registro de activación autorizada (18/11/2026 Guatemala)

El propietario autorizó explícitamente integrar main, deshabilitar el parche antiguo, ajustar configuración no secreta, actualizar sólo Web y activar/validar CI/CD. Se integró por fast-forward, conservando el historial.

Referencias anteriores al primer despliegue:

- Contenedor Web: `982eaab22579c30c69f5e756fc409f20903e9f75e4f947b017ff9037cd85d080`.
- Config digest Web: `sha256:ef0055fb31157dd930bf40037981d9293b9ab352741be4ebb5f4f0be04306d8f`.
- Imagen anterior identificada por coincidencia del config digest en el registry: `josueg07/proyectobd-web@sha256:a5010aaf368f68e6bc77bfc1786e8b4b3a469f29f4045470cfe933156a1faa03`, tag fuente f31acb7a641cc6d569cc11274513459100c16ee8.
- Contenedor SQL: `b51d255e6c6e97247f102cf38807e94a9b7701241f49efa82da0b81b40c7f826`.
- Imagen SQL: `sha256:548ecf4a83632020d95dfc744bac431f91e31458c2a785b61275941abae7c2bf`.
- Mounts: `proyectobd-proyectobd-jsdy5d_dp-keys:/var/dpkeys` y `proyectobd-proyectobd-jsdy5d_sql-data:/var/opt/mssql`.

IDs/mounts recibidos de la consulta ejecutada por el propietario. El propietario guardó las variables TRUSTED_PROXY_NETWORKS y TRUSTED_PROXY_HOSTS sin mostrar la contraseña SQL. En UI se deshabilitó el parche, se guardó el comando sólo Web, se seleccionó deploy/production y se habilitó Autodeploy. En GitHub se creó la variable no secreta DOKPLOY_CD_ENABLED=true.

Bootstrap [44463360433](https://github.com/josueg07/ProyectoBD/actions/runs/44463360433) terminó success en verify/publish/release con prepare_only=true. Creó la primera release `84d9066260868780d18a1b0ce2891bb7eb8531b4` para fuente c6232d2; todavía no era prueba de actualización productiva.

Cambio visible enviado por push: `ffbc46b72a26224c6f37e971481da18416e24b8c`. [Workflow de validación completa](https://github.com/josueg07/ProyectoBD/actions/runs/44463842668). Antes del despliegue, el Login público mostraba el pie sin Redes II.

Se inspeccionó `/etc/dokploy/traefik/traefik.yml` desde su editor, sin modificar: docker provider con network=dokploy-network, exposedByDefault=false, entrypoints 80/443 y ACME HTTP challenge/Let's Encrypt. No contiene forwardedHeaders.insecure ni una lista de proxies externos confiables; las pruebas externas también rechazaron el cambio de esquema por encabezados falsificados. `api.insecure=true` corresponde al API/dashboard de Traefik y es distinto de forwardedHeaders.insecure; su exposición administrativa completa queda fuera de esta corrección y no fue auditada aquí. No se abrió acme.json ni se leyeron claves privadas.

**Primera actualización automática completa: aprobada.** El workflow 44463842668 terminó success en verify/publish/release. El commit de release fue `71d5a1b43a59185d5a36f0c78da89e4a329b35c5`; imagen ejecutada `josueg07/proyectobd-web@sha256:954de4dd46ba088f35e78e3dba6cd02b715eea8716eb20d204eb5595136c3561`. `/deployment.json` coincidió con ffbc46b y el Login público pasó de mostrar el pie anterior a mostrar `Sistema de Ventas e Inventario - Redes II`. No se presionó Deploy manualmente.

La consulta posterior del propietario mostró Web healthy con nuevo ID `c75f64f4fd59df070029f0a4443a67c50a224250ab505165ba08edd07585a9b1`, mismo volumen dp-keys; SQL healthy con **exactamente el mismo ID b51d255e… y el mismo volumen sql-data**. El propietario confirmó login y consulta de registros correctos.

Desde la terminal Linux del contenedor se ejecutó únicamente printenv de las tres variables no secretas y curl de consulta contra loopback con X-Forwarded-Proto:https falsificado. Resultado: **UNTRUSTED_HTTP=400**. Se detectó que el runtime aún recibía la antigua /32, pese a la edición reportada de Environment. No se leyó todo Environment para investigar precedencias, por lo que el origen exacto de ese valor residual es NO VERIFICADO.

Para eliminar esa dependencia, el commit **84c492a41f7728a8fa36f498d6f4ee6188d9baf3** separó parámetros de plantilla `DOKPLOY_PROXY_*` de nombres runtime `TRUSTED_PROXY_*`. Se comprobó localmente que incluso con TRUSTED_PROXY_NETWORKS=10.0.1.7/32 residual, Compose genera el CIDR verificado 10.0.1.0/24. [Workflow 44464651781](https://github.com/josueg07/ProyectoBD/actions/runs/44464651781) terminó success y la revisión pública coincidió con 84c492a, manteniendo HTTPS, Login, health y rechazo de encabezados externos falsificados. Imagen de esa release: `josueg07/proyectobd-web@sha256:65d2c046651d00e16b8059118b52c45a5bfb95cb274ba06c4d8e87c093fef95f`.

## 14. Corrección visual adicional de auditoría solicitada durante la validación

El propietario confirmó que la aplicación funcionaba y pidió corregir Datos anteriores/Datos nuevos, mostrados como JSON. Se revisaron todas las referencias de esas columnas en Web, PDF y escritorio. No se cambió su almacenamiento, consultas, permisos, procesos de facturación ni SQL.

- `ProyectoBD.Web/Helpers/FormatoAuditoria.cs`: convierte objetos y listas JSON en etiquetas/valores con registros numerados y sangría; interpreta lotes anidados, booleanos y nulos. Los importes conservan su precisión original. Texto histórico no JSON o JSON malformado se conserva tal cual para no perder información.
- `ProyectoBD.Web/Helpers/ReporteWeb.cs`: aplica el formato sólo a las dos columnas auditadas; los DataTable originales no se modifican. La vista existente muestra texto escapado por Razor, sin Html.Raw. Web, consulta/anulación, previsualización y PDF comparten este adaptador.
- `ProyectoBD.Presentacion/Reportes.cs`: aplica el mismo formato mediante CellFormatting; columnas multilínea y doble clic para abrir lectura completa con desplazamiento y botón Volver. No se altera DataSource ni el JSON guardado.
- `ProyectoBD.Presentacion/ProyectoBD.Presentacion.csproj`: enlaza el archivo del formateador como código de presentación; **no agrega referencia de WinForms al proyecto Web ni otra capa de negocio**.
- `Pruebas/Infraestructura/PruebasAuditoria.cs`: nueve comprobaciones aprobadas de lectura, precisión, nulos/vacíos, JSON anidado, fallback histórico, invariancia de los datos y generación del PDF real. Se mantienen las quince comprobaciones de proxy.

Compilación completa tras la corrección: **0 errores y 0 advertencias**. Se generó un PDF sintético de prueba de dos páginas y se renderizaron/revisaron ambas; no hay JSON crudo ni texto cortado. No se creó ninguna factura ni se anuló ningún documento para probarlo. La interacción manual de la ventana WinForms no se probó; se compiló la solución completa. Commit de la corrección: `e81eef35771636a15894890c1a1badeafa2385b9`; [workflow 44465311005](https://github.com/josueg07/ProyectoBD/actions/runs/44465311005).

**Resultado productivo final de esa corrección:** verify, publish y release terminaron success. La release `ec08a3a` fijó la imagen `josueg07/proyectobd-web@sha256:d5fbffadd7c2268521ff49a48c3fb699db61241ebb1517ba13b280e6323bd7aa`. La revisión servida por /deployment.json coincide con e81eef3; el verificador público pasó HTTPS, Login, Healthy y encabezados falsificados. Las 24 comprobaciones .NET (15 de proxy y 9 de auditoría) y las 3 pruebas Python pasaron también en CI.

En la terminal del contenedor final `a5d251d47779` se verificaron sólo tres variables no secretas: **dokploy-traefik, 10.0.1.0/24 y 443**. Se repitió la petición directa de loopback con esquema falsificado: **400**. Esta vez el runtime ya no depende de la /32 residual. SQL sigue healthy, con identificador corto **b51d255e6c6e**, coincidente con el registro anterior, y sql-init sigue Exited 0 con su antigüedad previa. Las actualizaciones automáticas sólo recrearon Web.

Se solicitó acceso manual como Gerente en el navegador integrado para revisar la auditoría de una factura real sin manejar contraseñas. Esa inspección autenticada nueva queda **NO VERIFICADO** mientras no finalice el acceso manual. La generación y el contenido legible del reporte se comprobaron con DataSet/PDF sintéticos y sin acceso a SQL; no se confunden con una prueba manual de todos los datos históricos.

La actualización final de este documento es sólo registro de resultados, con `[skip ci]` para evitar otra recreación Web por documentación. La revisión de aplicación desplegada es e81eef3; los siguientes cambios de código enviados normalmente a main vuelven a ejecutar todo el flujo automáticamente.
