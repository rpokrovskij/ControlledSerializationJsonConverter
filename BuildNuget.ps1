# CONFIGURE
$msbuildPath = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe' 
$nugetPath   = 'C:\bin\nuget.exe'
$nugetApiKey = '323a45ba-fba3-4a3d-8429-047c1d3a0f0c'
$assemblyInfoLocation = 'ControlledSerializationJsonConverter\Properties\AssemblyInfo.cs'

# STEP 1. Get Solution Folder
$SolutionFolderPath = $PSScriptRoot #or enter it manually there 

If ($SolutionFolderPath -eq '') {
    $SolutionFolderPath = 'D:\cot\ControlledSerializationJsonConverter'
    #throw "Rut it as script from the VS solution's root folder, this will point the location of the solution."
}

# STEP 2. Get Version
$Version = Get-Content "$SolutionFolderPath\$assemblyInfoLocation" | Select-String 'AssemblyVersion.*\(.*\"(.*)\".*\)' -AllMatches | Foreach-Object {$_.Matches} | 
       Foreach-Object {$_.Groups[1].Value} | select-object

# it is possible to configure /p:TargetFrameworkVersion="v%1"
& $msbuildPath "Vse.Web.Serialization.sln" /p:Configuration="Release-Net40" /t:"ControlledSerializationJsonConverter:Clean","ControlledSerializationJsonConverter:Rebuild" /v:m /nologo
& $msbuildPath "Vse.Web.Serialization.sln" /p:Configuration="Release-Net45" /t:"ControlledSerializationJsonConverter:Clean","ControlledSerializationJsonConverter:Rebuild" /v:m /nologo

& $nugetPath pack -Version "$Version" Vse.Web.Serialization.nuspec

# STEP 3. PUSH
# $package = Get-ChildItem -Path "$SolutionFolderPath" -Filter 'Vse.Web.Serialization.ControlledSerializationJsonConverter.*.nupkg' -File | sort LastWriteTime | select -first 1 | % { "$($_.FullName)" } 
# & $nugetPath push "$package" "$nugetApiKey" -Source https://www.nuget.org/api/v2/package -User "ko" -pass "ko"

# or nuget update ? when to use push or update? there is config file which not possible. How to login ?

