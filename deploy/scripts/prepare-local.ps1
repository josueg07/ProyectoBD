$ErrorActionPreference = 'Stop'
$destino = Join-Path $PSScriptRoot '../docker'
if (Test-Path (Join-Path $destino '.env')) { throw '.env ya existe; no se sobrescribe ni se rota el certificado.' }
New-Item -ItemType Directory (Join-Path $destino 'secrets') -Force | Out-Null
$claveSql = 'Aa1!' + [Convert]::ToHexString([Security.Cryptography.RandomNumberGenerator]::GetBytes(24))
$claveCert = [Convert]::ToHexString([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
$rsa = [Security.Cryptography.RSA]::Create(3072)
$req = [Security.Cryptography.X509Certificates.CertificateRequest]::new('CN=ProyectoBD-DataProtection',$rsa,[Security.Cryptography.HashAlgorithmName]::SHA256,[Security.Cryptography.RSASignaturePadding]::Pkcs1)
$cert = $req.CreateSelfSigned([DateTimeOffset]::UtcNow.AddDays(-1),[DateTimeOffset]::UtcNow.AddYears(3))
[IO.File]::WriteAllBytes((Join-Path $destino 'secrets/dp.pfx'),$cert.Export([Security.Cryptography.X509Certificates.X509ContentType]::Pfx,$claveCert))
@("MSSQL_SA_PASSWORD=$claveSql","DP_CERT_PASSWORD=$claveCert",'WEB_IMAGE=proyectobd-web:latest','WEB_PORT=8085') | Set-Content (Join-Path $destino '.env') -Encoding utf8
$rsa.Dispose(); $cert.Dispose()
Write-Host 'Secretos locales generados en archivos ignorados por Git. No se muestran contraseñas.'
