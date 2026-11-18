param([string]$Image='proyectobd-web:latest')
$ErrorActionPreference='Stop'
function K { & kubectl @args; if($LASTEXITCODE -ne 0){throw 'kubectl falló; despliegue detenido.'} }
$root=Split-Path (Split-Path $PSScriptRoot)
$man=Join-Path $root 'deploy/kubernetes'
$envFile=Join-Path $root 'deploy/docker/.env'
if(!(Test-Path $envFile)){throw 'Ejecute primero prepare-local.ps1.'}
$variables=@{}
Get-Content $envFile | Where-Object {$_ -match '^[A-Z_]+='} | ForEach-Object {$k,$v=$_ -split '=',2; $variables[$k]=$v}
K apply -f "$man/namespace.yaml"
# YAML temporal sólo en memoria; nunca imprimir secretos.
$secreto=@{apiVersion='v1';kind='Secret';metadata=@{name='sql-secret';namespace='proyectobd'};type='Opaque';stringData=@{MSSQL_SA_PASSWORD=$variables.MSSQL_SA_PASSWORD}} | ConvertTo-Json -Depth 8
$secreto | & kubectl apply -f - | Out-Null; if($LASTEXITCODE){throw 'Error creando Secret SQL'}
$secreto=@{apiVersion='v1';kind='Secret';metadata=@{name='web-secret';namespace='proyectobd'};type='Opaque';stringData=@{PROYECTOBD_CONNECTION_STRING="Server=sqlserver,1433;Database=Proyecto_BD;User Id=sa;Password=$($variables.MSSQL_SA_PASSWORD);Encrypt=True;TrustServerCertificate=True;";DATA_PROTECTION_CERTIFICATE_PASSWORD=$variables.DP_CERT_PASSWORD}} | ConvertTo-Json -Depth 8
$secreto | & kubectl apply -f - | Out-Null; if($LASTEXITCODE){throw 'Error creando Secret Web'}
& kubectl create secret generic dp-certificate -n proyectobd --from-file="dp.pfx=$root/deploy/docker/secrets/dp.pfx" --dry-run=client -o yaml | & kubectl apply -f -
if($LASTEXITCODE){throw 'Error certificado'}
& kubectl create configmap sql-init-files -n proyectobd --from-file="init-sql.sh=$root/deploy/scripts/init-sql.sh" --from-file="Proyecto_BD_Final.sql=$root/deploy/docker/sql/Proyecto_BD_Final.sql" --dry-run=client -o yaml | & kubectl apply -f -
if($LASTEXITCODE){throw 'Error ConfigMap SQL'}
foreach($f in 'configmap.yaml','sqlserver-pvc.yaml','sqlserver-service.yaml','sqlserver-statefulset.yaml','sql-init-job.yaml'){K apply -f "$man/$f"}
K wait -n proyectobd --for=condition=complete job/sql-init --timeout=600s
$web=(Get-Content "$man/web-deployment.yaml" -Raw).Replace('REGISTRY_USUARIO/proyectobd-web:latest',$Image)
$web | & kubectl apply -f -; if($LASTEXITCODE){throw 'Error Deployment Web'}
K apply -f "$man/web-service.yaml"
K apply -f "$man/ingress.yaml"
K rollout status -n proyectobd deployment/proyectobd-web --timeout=300s
K get pods,deployments,svc,ingress,pvc -n proyectobd
