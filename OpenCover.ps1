$mstestLocation = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\mstest.exe'
$openCoverPath = 'packages\Opencover.4.6.519\Tools\OpenCover.Console.exe'
$reportGeneratorPath = 'packages\ReportGenerator.2.5.6\Tools\ReportGenerator.exe'
$TestsFolder1 = 'ControlledSerializationJsonConverter.Test'

$ErrorActionPreference = 'Stop'

if ($PSScriptRoot -eq ''){
    throw "Rut it as script from solution root, because we need to know the solution location."
}
#cho $PSScriptRoot

# STEP 1. create TestResults folder
$SolutionFolder = $PSScriptRoot

$TestResultsFolder = $SolutionFolder+'\TestResults'
If (!(Test-Path "$TestResultsFolder")){
    New-Item -ItemType Directory -Force -Path $TestResultsFolder
}

# STEP 2. empty TestResults folder content if needed
$MsTestFolder = $TestResultsFolder+'\mstest'
$GeneratedReportsHtmlFolder = $TestResultsFolder+'\report'
$OpenCoverOutput ="$TestResultsFolder\opencover.xml"
If (Test-Path "$MsTestFolder"){
	Remove-Item "$MsTestFolder" -Recurse
}
If (Test-Path "$GeneratedReportsHtmlFolder"){
	Remove-Item "$GeneratedReportsHtmlFolder" -Recurse
}
If (Test-Path "$OpenCoverOutput"){
	Remove-Item "$OpenCoverOutput"
}

#STEP 3. create folder for mstest results
If (!(Test-Path "$MsTestFolder")){
    New-Item -ItemType Directory -Force -Path $MsTestFolder
}

#STEP 4. Execute

$openCoverLocation = $SolutionFolder + '\'+ $openCoverPath
$TrxFile1 = "$MsTestFolder\ControlledSerializationJsonConverterTests.trx"
$TestDll1 = $SolutionFolder+"\"+$TestsFolder1 + '\bin\Debug\Vse.Web.Serialization.Test.dll' 
$targetargs = "/testcontainer:$TestDll1 /resultsfile:$TrxFile1"
$filters='+[Vse.*]* -[Vse.Web.Serialization.Test]*'
$reportGeneratorLocation = $SolutionFolder + '\'+$reportGeneratorPath

& $openCoverLocation "-register:user" "-target:$mstestLocation" "-targetargs:$targetargs" "-filter:$filters" "-mergebyhash"  "-skipautoprops" "-output:$OpenCoverOutput"

& $reportGeneratorLocation "-reports:$OpenCoverOutput" "-targetdir:$GeneratedReportsHtmlFolder"