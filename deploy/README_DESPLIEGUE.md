# Despliegue de ProyectoBD.Web

Ejecutar PowerShell **7** desde la raíz de `ProyectoBD2`. La misma imagen se usa en Compose, Dokploy y Kubernetes. SQL Server Developer es para desarrollo/demostración; producción requiere licencia apropiada. No usar LocalDB fuera de Windows.

## A. Docker Compose local

```powershell
docker desktop start
./deploy/scripts/prepare-local.ps1 # sólo primera vez; NO sobrescribe secretos existentes
docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml config --quiet
docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml build
docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml up -d
docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml ps
Invoke-RestMethod http://localhost:8085/health
Start-Process http://localhost:8085
```

El SQL aprobado es `Desktop/Proyecto Bases de Datos 2/ProyectoBD.sql`; se copió **sin cambios** a `deploy/docker/sql/Proyecto_BD_Final.sql`. SHA256: `9346E60FFFE799CC35C74170BCE214792BAA58FEAA5F51F73356C374E6498F63`.

SQL Server inicia con volumen `sql-data` y no expone 1433 al host. Tras estar saludable, el servicio `sql-init` ejecuta el SQL final con `sqlcmd -b`. Sólo importa si **Proyecto_BD no existe**. Después añade una tabla técnica `__DeploymentInitialization` como marca de finalización. Si la base existe sin esa marca, se detiene para revisión manual: no borra, reimporta ni intenta reparar automáticamente. Así una importación interrumpida no se oculta como exitosa. Bases instaladas manualmente necesitan verificar esquema/datos antes de adoptar esta marca; no crearla sin revisión.

La Web espera el fin satisfactorio del inicializador y se conecta a `Server=sqlserver,1433`. Puerto interno 8080; host **8085**, porque 8080 ya estaba ocupado. Se puede cambiar `WEB_PORT` en `.env`. `/health` comprueba vida del proceso; no consulta SQL. Verificar Login para probar también la conexión real.

Los secretos se generan aleatoriamente en `.env` y `secrets/dp.pfx`, ignorados por Git y por el contexto Docker. No imprimir `docker compose config` sin `--quiet`, pues expandiría secretos. Conservar copias protegidas de `.env`, PFX y volúmenes. No regenerar el certificado al recrear los contenedores: protege las claves antiguas y permite conservar cookies.

## B. Construir imagen

```powershell
docker build -f deploy/docker/Dockerfile -t proyectobd-web:latest .
docker image inspect proyectobd-web:latest --format '{{.Id}}'
```

Multi-stage oficial SDK .NET 8 → ASP.NET 8, puerto 8080, usuario `app` no root. Las fuentes Noto están incrustadas en la DLL y su licencia se publica. `curl` sólo se usa para healthcheck. No se incluyen bin/obj, secretos, .env ni WinForms.

## C. Registry

```powershell
$RegistryUsuario = Read-Host 'Usuario Docker Hub'
docker tag proyectobd-web:latest "$RegistryUsuario/proyectobd-web:latest"
docker login # introducir credenciales interactivamente
docker push "$RegistryUsuario/proyectobd-web:latest"
```

No hay credenciales de Registry configuradas en el repositorio. Usar tags por SHA para despliegues reproducibles. Para imagen privada crear `imagePullSecret` de Kubernetes mediante credenciales propias y referenciarlo en `spec.template.spec.imagePullSecrets`.

## D. Kubernetes / Minikube

```powershell
minikube start --driver=docker --cpus=3 --memory=4608
minikube addons enable ingress
minikube image load proyectobd-web:latest
minikube image load mcr.microsoft.com/mssql/server:2022-latest
./deploy/scripts/deploy-k8s.ps1 -Image proyectobd-web:latest
```

Si el perfil ya existe, Minikube puede conservar sus recursos previos; no eliminarlo para cambiarlos. El script aplica únicamente el namespace `proyectobd`, crea Secrets desde archivos locales ignorados, monta el SQL original como ConfigMap, espera el Job de importación y despliega dos réplicas Web. Se detiene si falla un paso. No aplicar `secret.example.yaml` tal cual. Para otro clúster comprobar antes `kubectl config current-context` y utilizar `-Image "$RegistryUsuario/proyectobd-web:latest"`; allí no se usa `minikube image load`.

El Job completado se conserva: reaplicar no vuelve a importar. Para revalidar la inicialización puede eliminarse **sólo el Job sql-init**, nunca sus PVC, y reaplicarse el manifiesto; comprobará la marca existente.

La PVC SQL persiste datos. La PVC `dp-keys` comparte las claves de cookies entre ambos Pods. Para demostración en un nodo usa RWO con afinidad de Pods al mismo nodo. Esto tolera caída de un **Pod**, no caída del nodo ni del único SQL. Producción multinodo necesita almacenamiento RWX o servicio externo para claves y una estrategia real de HA de SQL; no basta cambiar replicas.

## E. Verificar 2 réplicas

```powershell
kubectl get pods -n proyectobd -o wide
kubectl get deployments -n proyectobd
kubectl get svc,ingress,pvc -n proyectobd
kubectl rollout status deployment/proyectobd-web -n proyectobd
kubectl port-forward -n proyectobd svc/proyectobd-web 8081:80
# En otra terminal:
Invoke-RestMethod http://localhost:8081/health
Start-Process http://localhost:8081
```

Esperado: Deployment READY **2/2**. `port-forward` llega a un Pod; demuestra acceso, no balanceo. El Service tiene `sessionAffinity: ClientIP` y el Ingress también afinidad por cookie, porque el Ingress puede seleccionar endpoints directamente. Las previsualizaciones permanecen en memoria del Pod: si éste desaparece, generar otra; producción debe usar caché distribuida (por ejemplo Redis). Las cookies de autenticación usan claves y certificado compartidos, independientes de la memoria de reportes.

## F. Probar alta disponibilidad

```powershell
kubectl get pods -n proyectobd -l app=proyectobd-web
$Pod = kubectl get pods -n proyectobd -l app=proyectobd-web -o jsonpath='{.items[0].metadata.name}'
kubectl delete pod $Pod -n proyectobd
kubectl get pods -n proyectobd -l app=proyectobd-web -w
# Ctrl+C cuando se vea el reemplazo Ready
kubectl rollout status deployment/proyectobd-web -n proyectobd
```

Mantener la Web abierta por Ingress: debe quedar otro Pod disponible y aparecer un nuevo nombre. Una petición en curso al Pod eliminado puede fallar; la siguiente podrá usar el superviviente. Volver a generar reportes cuya copia estaba en el Pod eliminado.

## G. Persistencia SQL

Antes y después comparar un cliente o el número de usuarios del SQL final. No hace falta crear ni alterar datos operativos.

```powershell
docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml exec sqlserver bash -c 'SQLCMDPASSWORD="$MSSQL_SA_PASSWORD" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -d Proyecto_BD -Q "SELECT COUNT(*) Usuarios FROM dbo.tbl_Usuario"'
docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml up -d --force-recreate
# Repetir SELECT y comprobar el mismo resultado.
docker volume ls --filter label=com.docker.compose.project=proyectobd-redes
```

Para detener: `docker compose --env-file deploy/docker/.env -f deploy/docker/compose.yaml down`, **sin `-v`**. No borrar PVC ni namespace en Kubernetes para demostrar persistencia; reiniciar el StatefulSet (`kubectl rollout restart statefulset/sqlserver -n proyectobd`) conserva su PVC. Esto implica una breve interrupción de SQL, pues no es un clúster SQL redundante.

## H. Ingress

```powershell
minikube ip
kubectl get ingress -n proyectobd
minikube tunnel # puede requerir terminal elevada, dejarla abierta
```

Host inicial `proyectobd.local`, configurable editando `spec.rules[0].host`. Resolver ese nombre hacia la dirección alcanzable de tu controlador. En Docker Desktop/Windows, la IP interna de Minikube puede no ser accesible; usar tunnel o, para demostración inequívoca:

```powershell
kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8082:80 8443:443
# Otra terminal, sin editar hosts:
curl.exe -H 'Host: proyectobd.local' http://localhost:8082/health
```

Para abrir el dominio en el navegador configurar hosts/DNS según el túnel usado. No fijar una IP dentro de los manifests.

## I. HTTPS

Alternativa rápida en **PowerShell 7 Windows**, ya comprobada en esta ejecución:

```powershell
./deploy/scripts/prepare-tls-demo.ps1
kubectl rollout status deployment/proyectobd-web -n proyectobd
# Mantener abierto el port-forward 8082:80 8443:443 indicado en H.
curl.exe --noproxy '*' --cacert deploy/docker/secrets/tls.crt --resolve proyectobd.local:8443:127.0.0.1 https://proyectobd.local:8443/health
```

El script confía sólo en los CIDR de Pods obtenidos del clúster; revisar ese alcance en producción. No importa el certificado en el almacén de confianza Windows. Su clave privada queda bajo `secrets/`, ignorada. Esta alternativa y los comandos Linux siguientes son opciones equivalentes: no hace falta ejecutar ambas.

Para demo, generar en una terminal Linux con OpenSSL un certificado local (no subir la clave):

```bash
openssl req -x509 -newkey rsa:3072 -sha256 -nodes -days 7 -keyout /tmp/proyectobd.key -out /tmp/proyectobd.crt -subj '/CN=proyectobd.local' -addext 'subjectAltName=DNS:proyectobd.local'
kubectl create secret tls proyectobd-tls -n proyectobd --cert=/tmp/proyectobd.crt --key=/tmp/proyectobd.key
kubectl patch ingress proyectobd-web -n proyectobd --type=merge -p '{"spec":{"tls":[{"hosts":["proyectobd.local"],"secretName":"proyectobd-tls"}]}}'
kubectl annotate ingress proyectobd-web -n proyectobd nginx.ingress.kubernetes.io/ssl-redirect=true --overwrite
```

Configurar la red de proxies de confianza antes de HTTPS. Obtener `kubectl get nodes -o jsonpath='{.items[*].spec.podCIDR}'`, revisar la red real del controlador y:

```powershell
$RedProxy = Read-Host 'CIDR de la red de ingress/proxy confiable'
kubectl set env deployment/proyectobd-web -n proyectobd "TRUSTED_PROXY_NETWORKS=$RedProxy"
```

No confiar en todo Internet ni borrar listas de proxies conocidos. `Program.cs` interpreta X-Forwarded-Proto sólo desde proxies confiables; esto permite cookies/redirects correctos tras terminación TLS. Certificado autofirmado no es certificado público confiable: el usuario debe confiar expresamente en él para la demo. Para producción usar dominio real y certificado válido; cert-manager es opcional si ya está instalado, con Issuer/ClusterIssuer propio. No se instaló cert-manager.

## J. Dokploy

1. Crear proyecto y servicio Compose, conectar repositorio Git privado.
2. Indicar archivo `deploy/docker/compose.yaml`; las rutas relativas deben resolverse desde esa carpeta.
3. Cargar variables de `.env` desde almacenamiento seguro, sin publicarlas en Git.
4. Proveer mediante los montajes/archivos seguros de Dokploy el PFX en `deploy/docker/secrets/dp.pfx` y preservar volúmenes entre despliegues. No generar un PFX distinto en cada deploy. El SQL y el bash de inicialización sí están versionados.
5. Usar la misma imagen `USUARIO/proyectobd-web:latest`. Para consumir exclusivamente la imagen publicada configurar en Dokploy Compose el servicio web sin su bloque `build`; mantener el resto del mismo Compose. Si se deja `build`, se compila el mismo Dockerfile.
6. En Domains seleccionar servicio **web**, puerto **8080**, asignar dominio con DNS al servidor y habilitar SSL de Dokploy. No enrutar hacia sqlserver o sql-init.
7. Configurar `TRUSTED_PROXY_NETWORKS` con la red real de Traefik y añadirla al environment del servicio Web; no introducir CIDR universal. Mantener puerto host libre o dejar el acceso sólo por Traefik en el servidor según la configuración de Dokploy.
8. Desplegar, revisar logs y comprobar `https://DOMINIO/health` y Login.

No se necesita tocar lógica MVC/SQL. El despliegue remoto requiere acceso a tu Dokploy, dominio y credenciales propios. [Dominios Compose oficiales](https://docs.dokploy.com/docs/core/docker-compose/domains).

## K. Fail2ban

**Sólo en el servidor Linux**, no en este Windows. Ubuntu/Debian:

```bash
sudo apt-get update
sudo apt-get install -y fail2ban
sudo tee /etc/fail2ban/jail.d/sshd.local >/dev/null <<'EOF'
[sshd]
enabled = true
backend = systemd
maxretry = 5
findtime = 10m
bantime = 1h
EOF
sudo systemctl enable --now fail2ban
sudo systemctl restart fail2ban
sudo fail2ban-client status
sudo fail2ban-client status sshd
```

Verificar puerto SSH real y permitir una vía de administración antes de cambiar reglas. Esta protección cubre SSH, no implementa automáticamente bloqueo de Login MVC ni tráfico reenviado por Docker.

## L. CI/CD

Workflow `.github/workflows/docker.yml`: push a main o ejecución manual → checkout → Docker Hub con Secrets → build y push de latest y SHA. Crear Secrets `REGISTRY_USERNAME` y `REGISTRY_PASSWORD` (token del Registry). No se creó repositorio ni se hizo push sin tus credenciales. En Dokploy habilitar Auto Deploy o su webhook desde la interfaz del servicio; guardar URL/token como Secret si se invoca desde Actions, nunca en YAML público. No inventar una URL: copiar la generada por tu instalación.

Monitoreo mínimo:

```powershell
kubectl get pods -n proyectobd
kubectl describe deployment proyectobd-web -n proyectobd
kubectl logs -n proyectobd -l app=proyectobd-web --tail=100
kubectl logs -n proyectobd job/sql-init
kubectl get events -n proyectobd --sort-by=.lastTimestamp
```

Helm está disponible y se detectó monitoreo previo en otro namespace, que no se modificó. Si se necesita una instalación adicional y hay RAM suficiente, comandos **opcionales**, no ejecutados:

```powershell
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm upgrade --install redes-monitor prometheus-community/kube-prometheus-stack -n proyectobd-monitor --create-namespace
```

No es requisito ni bloquea la demo principal. No duplicar monitoreo si el existente ya cumple la necesidad.

## M. Capturas que debo tomar

1. Login y una pantalla funcional Web en Compose.
2. `docker compose ... ps`, con web/sqlserver saludables e inicializador Exited(0).
3. `http://localhost:8085/health` mostrando Healthy.
4. `docker volume ls` con sql-data y dp-keys.
5. `kubectl get pods -n proyectobd -o wide`: dos Web Running/Ready.
6. `kubectl get deployments -n proyectobd`: READY 2/2.
7. `kubectl get svc -n proyectobd`.
8. `kubectl get ingress -n proyectobd`.
9. `kubectl get pvc -n proyectobd`: Bound.
10. Web y health a través de Ingress o port-forward, indicando cuál.
11. Nombres de Pods antes/eliminación/reemplazo y regreso a 2/2.
12. Mismo dato SQL antes y después de recrear contenedores sin borrar volumen.
13. HTTPS/certificado si se configura realmente; distinguir autofirmado de público.
14. Dashboard y despliegue Dokploy, sólo cuando se ejecute en tu servidor.
15. `fail2ban-client status sshd`, sólo en Linux.
16. Imagen local/Registry con su tag o digest; Registry remoto sólo tras push real.
17. Ejecución verde de Actions, sólo tras configurar Secrets y ejecutar el pipeline.

Referencias: [Data Protection compartida](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-8.0), [SQL Server Docker](https://learn.microsoft.com/sql/linux/quickstart-install-connect-docker), [afinidad ingress-nginx](https://kubernetes.github.io/ingress-nginx/examples/affinity/cookie/).
