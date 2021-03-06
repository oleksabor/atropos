
$path = "..\src"
$configuration="Release"

$version = Get-Content "$path\\versionFile"

if ([string]::IsNullOrWhitespace($configuration)) { Throw New-Object System.ArgumentException("no configuration parameter")}
if ([string]::IsNullOrWhitespace($version)) { Throw New-Object System.ArgumentException("no version parameter")}

if ($configuration -ne "Release" -and $configuration -ne "Debug") { Throw New-Object System.ArgumentException([string]::Format("unsupported configuration parameter value {0}", $configuration))}

if ($version -notmatch "^\d+\.\d+\.\d+$") { Throw New-Object System.ArgumentException([string]::Format("unsupported version parameter value {0}", $version))} 
$platformDefault = 'Any CPU'
if ($platform -ne 'x86' -and $platform -ne 'x64') { 
'unknown platorm, using default ' + $platformDefault
$platform = $platformDefault 
}

& .\setAssemblyInfo.ps1 $version $path

$solution = "$path\\accountTimer.sln"
#$msb = ${env:programfiles(x86)}+'\MSBuild\14.0\Bin\msbuild.exe'

$msb = ${env:programfiles(x86)}+'\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe'

'building ' + $solution + ' configuration:'+$configuration+ ' platform:'+$platform
& $msb $solution /t:build /p:configuration=$configuration $platformSwitch > build.log

if (-Not $?) { Throw New-Object System.ArgumentException([string]::Format("failed to build {0}", $lastexitcode))}
'build was finished'

$platformPath = ""
if ($platform -ne $platformDefault) { $platformPath = $platform }

$a7z = ${env:programfiles}+'\7-zip\7z.exe'
$archName = "..\bin\atropos_$configuration.$version.7z"

Class Target {
[String] $bin # binary path
[String] $arcPrefix
[String] $arcName
}

$targetList=@(
[Target]@{bin="$path\server\bin\$platformPath\$configuration\*";arcPrefix='..\bin\atroposService'}
,[Target]@{bin="$path\client\bin\$platformPath\$configuration\*";arcPrefix='..\bin\atroposClient'}
)

For ($i=0; $i -lt $targetList.Length; $i++) {
	'========================================'
	'archive '+$targetList[$i].bin+ ' started'

 	$archName = $targetList[$i].arcPrefix+$configuration+'.'+$version+'.7z'

	if (Test-Path $archName) { 
		'removing '+$archName 
		Remove-Item $archName 
	}

	& $a7z a $archName $targetList[$i].bin -m0=LZMA '-xr!*.log' '-xr!*.tmp'

	$targetList[$i].arcName=[System.IO.Path]::GetFileName($archName)
	'done '+$archName
}


