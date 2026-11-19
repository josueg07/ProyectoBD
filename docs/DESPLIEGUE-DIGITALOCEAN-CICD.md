# ProyectoBD: HTTPS y CI/CD reproducible en DigitalOcean

Revisión e implementación local: **18/11/2026, America/Guatemala**. Rama de trabajo: `fix/dokploy-https-cicd`.

**Estado: preparado y probado localmente; activación productiva PENDIENTE DE AUTORIZACIÓN.** No se cambió Dokploy, no se publicó una imagen productiva y no se modificó main durante la preparación. No se tocó SQL, contraseñas, volúmenes existentes ni lógica de ventas. No afirmar que el flujo productivo nuevo ya fue probado.

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

`deploy/docker/compose.yaml`, SQL, Datos, Negocio, Entidades, WinForms, Controllers y funcionalidades de negocio se conservan. Program.cs mantiene su codificación original; no se convirtió todo el archivo.

## 4. Solución permanente de HTTPS

La solución consta de **dos partes inseparables**: retirar el parche antiguo de Dokploy y usar la configuración del repositorio; restringir la confianza al proxy real sin fijar su IP.

Compose define:

```yaml
TRUSTED_PROXY_HOSTS: ${TRUSTED_PROXY_HOSTS:-dokploy-traefik}
TRUSTED_PROXY_NETWORKS: ${TRUSTED_PROXY_NETWORKS:-10.0.1.0/24}
ASPNETCORE_HTTPS_PORT: "443"
```

Con HOSTS configurado, NETWORKS **sólo restringe las respuestas DNS**: el middleware resuelve dokploy-traefik, filtra direcciones dentro del CIDR y utiliza únicamente esas direcciones exactas. No agrega el CIDR completo a KnownNetworks en este modo. No llama a KnownNetworks.Clear(), no acepta cualquier proxy y no habilita ASPNETCORE_FORWARDEDHEADERS_ENABLED.

La lista se renueva cada **30 segundos**. Se procesa **un salto**, X-Forwarded-For y X-Forwarded-Proto; no X-Forwarded-Host. Un contenedor distinto, incluso en 10.0.1.0/24, no puede declarar que es Traefik mediante un encabezado. Si DNS falla o no produce una IP válida, se devuelve 503; un peer no autorizado recibe 400. Así no se genera un Login HTTP durante ese fallo. Puede haber una ventana de indisponibilidad de hasta el refresco cuando cambia la IP del proxy.

Las sondas GET/HEAD `/health` internas siguen accesibles sin confiar en encabezados y sin redirigir a un puerto TLS de Kestrel inexistente. No exponen datos de negocio. El resto de la aplicación conserva redirección HTTPS. HSTS se evalúa después de restaurar el esquema del proxy. Autenticación y autorización no se sustituyeron.

Compatibilidad local/Minikube: cuando HOSTS no está configurado se conserva el modo CIDR explícito anterior, con defaults restrictivos de ASP.NET Core. No configurar HOSTS en Kubernetes sin definir su DNS y CIDR reales. El modo Dokploy debe tener ambas variables; no eliminar HOSTS dejando una subred compartida confiable.

**Límite de seguridad:** se confía en Docker DNS y en el control administrativo del daemon/red. Un atacante con acceso al socket Docker o permiso para reemplazar el contenedor/alias Traefik ya controla infraestructura crítica. No montar el socket en Web, no permitir contenedores de terceros sin revisar y no duplicar el alias dokploy-traefik. La caché puede retener una IP anterior hasta 30 s; la protección no equivale a mTLS entre proxy y Web.

Traefik debe sobrescribir/normalizar los encabezados de clientes y no tener `forwardedHeaders.insecure=true`. Las pruebas externas actuales con X-Forwarded-Proto=http y host falsificado conservaron HTTPS. La inspección completa del archivo estático efectivo de Traefik es **NO VERIFICADO**; comprobarla antes de aceptar el despliegue nuevo. No confiar en encabezados externos mediante una regla global de Traefik.

Si cambia la infraestructura: consultar nombre/IPAM de la red y resolución desde Web; actualizar HOSTS y el CIDR restringido en configuración versionada/Environment; revisar miembros; desplegar sólo Web y ejecutar las pruebas. No actualizar una /32 después de cada recreación. **Eliminar o cambiar el valor temporal 10.0.1.7/32 a 10.0.1.0/24 en Environment una sola vez**: de lo contrario ese valor prevalece sobre el default y restringirá las nuevas IP legítimas.

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

La integración GitHub App instalada debe recibir eventos push de la rama de release. Esto se comprobará con la primera release autorizada. Un push con GITHUB_TOKEN no inicia otros workflows de Actions; aquí el receptor es **Dokploy/GitHub App**, no un segundo workflow. Si la política de la organización impide el push o la entrega a la App, no dar el flujo por aceptado: revisar permisos/entregas y usar una App de CI autorizada o API postpublicación bajo aprobación, sin añadir un deploy temprano sobre main.

Secrets existentes: **REGISTRY_USERNAME**, **REGISTRY_PASSWORD**, únicamente para Docker Hub. El usuario necesita permisos de escritura sobre josueg07/proyectobd-web. GITHUB_TOKEN se emite por ejecución; `contents: write` se concede sólo al job release. La variable no secreta **DOKPLOY_CD_ENABLED=true** habilita release; sin ella sólo se construye/publica. No se configuró ni habilitó durante esta preparación.

## 6. Activación única en Dokploy y GitHub — requiere autorización

No activar Autodeploy todavía sobre main. No presionar Deploy antes de preparar y revisar todo este procedimiento.

1. Revisar/mergear la rama fix a main por el mecanismo habitual. Con DOKPLOY_CD_ENABLED ausente/false, se ejecutan pruebas y publicación, sin actualización productiva automática.
2. Registrar imagen/digest actual, ID/estado de SQL y nombres de volúmenes. El respaldo `Proyecto_BD_pre_despliegue.bak`, fuera del contenedor y con copia local, fue confirmado por el propietario; no se abrió, copió ni restauró aquí.
3. En Patches, **deshabilitar** el update de compose.dokploy.yaml, conservándolo como referencia de reversión. Es configuración persistente de Dokploy, no editar su copia interna. No deshabilitar parches de otros servicios.
4. En Environment conservar MSSQL_SA_PASSWORD y todos los valores existentes. Cambiar únicamente TRUSTED_PROXY_NETWORKS a `10.0.1.0/24` y establecer TRUSTED_PROXY_HOSTS=`dokploy-traefik`. El propietario puede editarlo para no mostrar su contraseña SQL. No sobrescribir todo Environment con un ejemplo.
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

Prueba remota directa de peer no confiable y recreación productiva controlada: **PENDIENTES**. Las equivalentes en contenedores locales aislados sí se ejecutaron. Revisar también el archivo estático de Traefik para confirmar que no confía indiscriminadamente en encabezados recibidos de Internet.

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

## 11. Pruebas realmente ejecutadas

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

Falta aprobar y ejecutar: integración de la rama a main; retiro del parche antiguo; ajuste de variables no secretas y comando sólo Web; rama de release y Autodeploy; primera actualización/validación pública; comprobación del digest ejecutado, SQL/mounts y Traefik efectivo. No se necesita un token nuevo de Dokploy para el diseño elegido. Si GitHub requiere iniciar sesión o modificar Secrets, debe hacerlo el propietario, sin compartirlos en el chat.

**No declarar cerrado el criterio de aceptación hasta que el cambio visible llegue automáticamente a la URL pública y HTTPS, SQL y los volúmenes se verifiquen después.**
