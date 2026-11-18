param([string]$HostName='proyectobd.local')
$ErrorActionPreference='Stop'
$dir=Join-Path $PSScriptRoot '../docker/secrets'
New-Item -ItemType Directory $dir -Force | Out-Null
$rsa=[Security.Cryptography.RSA]::Create(3072)
$req=[Security.Cryptography.X509Certificates.CertificateRequest]::new("CN=$HostName",$rsa,[Security.Cryptography.HashAlgorithmName]::SHA256,[Security.Cryptography.RSASignaturePadding]::Pkcs1)
$san=[Security.Cryptography.X509Certificates.SubjectAlternativeNameBuilder]::new();$san.AddDnsName($HostName);$req.CertificateExtensions.Add($san.Build())
$req.CertificateExtensions.Add([Security.Cryptography.X509Certificates.X509BasicConstraintsExtension]::new($true,$false,0,$true))
$cert=$req.CreateSelfSigned([DateTimeOffset]::UtcNow.AddDays(-1),[DateTimeOffset]::UtcNow.AddDays(7))
[IO.File]::WriteAllText((Join-Path $dir 'tls.crt'),$cert.ExportCertificatePem())
[IO.File]::WriteAllText((Join-Path $dir 'tls.key'),$rsa.ExportPkcs8PrivateKeyPem())
& kubectl create secret tls proyectobd-tls -n proyectobd --cert="$dir/tls.crt" --key="$dir/tls.key" --dry-run=client -o yaml | & kubectl apply -f -
if($LASTEXITCODE){throw 'No se pudo crear Secret TLS'}
$patch=@{spec=@{tls=@(@{hosts=@($HostName);secretName='proyectobd-tls'})}}|ConvertTo-Json -Depth 8 -Compress
& kubectl patch ingress proyectobd-web -n proyectobd --type=merge -p $patch
if($LASTEXITCODE){throw 'No se pudo configurar Ingress TLS'}
$redes=((kubectl get nodes -o json | ConvertFrom-Json).items.spec.podCIDR) -join ','
if(!$redes){throw 'Revise manualmente la red de proxies antes de habilitar HTTPS'}
& kubectl set env deployment/proyectobd-web -n proyectobd "TRUSTED_PROXY_NETWORKS=$redes"
& kubectl annotate ingress proyectobd-web -n proyectobd nginx.ingress.kubernetes.io/ssl-redirect=true --overwrite
$rsa.Dispose();$cert.Dispose()
Write-Host 'TLS local preparado. Certificado autofirmado por 7 días; no se añadió confianza al sistema.'
