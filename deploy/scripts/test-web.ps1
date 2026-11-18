param([string]$BaseUrl='http://localhost:8085', [string]$Usuario='gerente', [Security.SecureString]$Password, [string]$SegundaReplica='', [switch]$Pdf)
$ErrorActionPreference='Stop'
if(!$Password){$Password=Read-Host "Contraseña de $Usuario" -AsSecureString}
$clave=[Net.NetworkCredential]::new('', $Password).Password
$handler=[Net.Http.HttpClientHandler]::new();$handler.AllowAutoRedirect=$false;$handler.CookieContainer=[Net.CookieContainer]::new()
$http=[Net.Http.HttpClient]::new($handler)
try {
  $login=$http.GetStringAsync("$BaseUrl/Account/Login").GetAwaiter().GetResult()
  $input=[regex]::Match($login,'<input[^>]*name="__RequestVerificationToken"[^>]*>').Value
  $token=[Net.WebUtility]::HtmlDecode([regex]::Match($input,'value="([^"]+)"').Groups[1].Value)
  if(!$token){throw 'No se obtuvo antiforgery'}
  $campos=[Collections.Generic.Dictionary[string,string]]::new();$campos.Add('Usuario',$Usuario);$campos.Add('Password',$clave);$campos.Add('__RequestVerificationToken',$token)
  $r=$http.PostAsync("$BaseUrl/Account/Login",[Net.Http.FormUrlEncodedContent]::new($campos)).GetAwaiter().GetResult()
  if([int]$r.StatusCode -ne 302){throw 'Falló login contra SQL del contenedor'}
  $r=$http.GetAsync("$BaseUrl/Home/Index").GetAwaiter().GetResult()
  if([int]$r.StatusCode -ne 200){throw 'Cookie no válida'}
  Write-Host 'Login y cookie comprobados contra SQL del despliegue.'
  if($Pdf){
    $html=$http.GetStringAsync("$BaseUrl/Reportes").GetAwaiter().GetResult()
    $input=[regex]::Match($html,'<input[^>]*name="__RequestVerificationToken"[^>]*>').Value
    $token=[Net.WebUtility]::HtmlDecode([regex]::Match($input,'value="([^"]+)"').Groups[1].Value)
    $datos=[Collections.Generic.Dictionary[string,string]]::new()
    $datos.Add('Tipo','1');$datos.Add('Inicial','2026-01-01');$datos.Add('Final',(Get-Date -Format yyyy-MM-dd));$datos.Add('__RequestVerificationToken',$token)
    $r=$http.PostAsync("$BaseUrl/Reportes/Generar",[Net.Http.FormUrlEncodedContent]::new($datos)).GetAwaiter().GetResult()
    if([int]$r.StatusCode -ne 302){throw 'No genera previsualización'}
    $html=$http.GetStringAsync("$BaseUrl$($r.Headers.Location)").GetAwaiter().GetResult()
    $url=[Net.WebUtility]::HtmlDecode([regex]::Match($html,'href="([^"]*/Reportes/Pdf[^"]+)"').Groups[1].Value)
    $bytes=$http.GetByteArrayAsync("$BaseUrl$url").GetAwaiter().GetResult()
    if([Text.Encoding]::ASCII.GetString($bytes,0,5) -ne '%PDF-'){throw 'PDF inválido'}
    [IO.File]::WriteAllBytes((Join-Path ([IO.Path]::GetTempPath()) 'ProyectoBD-Container.pdf'),$bytes)
    Write-Host "PDF Linux descargado: $($bytes.Length) bytes."
  }
  if($SegundaReplica){
    $r=$http.GetAsync("$SegundaReplica/Home/Index").GetAwaiter().GetResult()
    if([int]$r.StatusCode -ne 200){throw 'La segunda réplica no aceptó la cookie de la primera'}
    Write-Host 'La segunda réplica aceptó la misma cookie.'
  }
} finally {$clave=$null;$http.Dispose();$handler.Dispose()}
