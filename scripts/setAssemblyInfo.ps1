#function Update-AssemblyInfoVersionFiles
#{
#  Param
#  (
#	[Parameter(Mandatory=$true)]
#    [string]$productVersion
#  )

param([string]$version, [string]$sources)

    $buildNumber = $version
    if ($buildNumber -eq $null)
    {
        $buildIncrementalNumber = 0
    }
    else
    {
        $splitted = $buildNumber.Split('.')
        $buildIncrementalNumber = $splitted[$splitted.Length - 1]
    }
      
	$SrcPath = $sources
    Write-Verbose "Executing Update-AssemblyInfoVersionFiles in path $SrcPath for product version Version $buildNumber"  -Verbose
 
    $AllVersionFiles = Get-ChildItem $SrcPath -Include AssemblyInfo.cs,*.AssemblyInfo.cs -recurse

	#calculation Julian Date 
	$year = Get-Date -format yy
	$julianYear = $year.Substring(0)
	$dayOfYear = (Get-Date).DayofYear
	$julianDate = $julianYear + "{0:D3}" -f $dayOfYear
	Write-Verbose "Julian Date: $julianDate" -Verbose

	#split product version in SemVer language
	$versions = $buildNumber.Split('.')
	$major = $versions[0]
	$minor = $versions[1]
	$patch = $versions[2]

    $assemblyVersion = $buildNumber
    $assemblyFileVersion = "$major.$minor.$buildIncrementalNumber.$julianDate"
    $assemblyInformationalVersion = $buildNumber
     
    Write-Verbose "Transformed Assembly Version is $assemblyVersion" -Verbose
    Write-Verbose "Transformed Assembly File Version is $assemblyFileVersion" -Verbose
    Write-Verbose "Transformed Assembly Informational Version is $assemblyInformationalVersion" -Verbose
 
    foreach ($file in $AllVersionFiles) 
    { 
		#version replacements
        (Get-Content $file.FullName) |
        %{$_ -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', "AssemblyVersion(""$assemblyVersion"")" } |
        %{$_ -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', "AssemblyFileVersion(""$assemblyFileVersion"")" } |
		%{$_ -replace 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', "AssemblyInformationalVersion(""$assemblyInformationalVersion"")" } | 
		Set-Content $file.FullName -Force
		$file.FullName+" version was set to " + $assemblyVersion
    }
  
#	return $assemblyFileVersion
#}