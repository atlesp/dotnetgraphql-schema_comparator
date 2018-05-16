Clear-Host
$ErrorActionPreference="Stop"
$env:BUILD_BUILDURI = "vstfs:///Build/Build/1808"
$env:BUILD_BUILDID  = "999"
$env:BUILD_BUILDNUMBER  = "10.10.0"
$env:SYSTEM_TEAMFOUNDATIONSERVERURI = "htps://fdfdsf.com/"
$env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI = "https://fdfdsf.com/"
$env:SYSTEM_COLLECTIONID = "CUID"

$env:SYSTEM_CULTURE = "en-US"

$env:INPUT_GRAPHQLAPINAME   = "MyApi";
$env:INPUT_FAILBUILDIFAPIBROKEN   = $false;

$env:INPUT_OLDSCHEMA    = "C:\temp\graphql-schemas\graphql-schema-diagnostics.txt";
$env:INPUT_NEWSCHEMA    = "C:\temp\graphql-schemas\graphql-schema-diagnostics-new.txt";
$env:INPUT_SCHEMADIFF   = "C:\temp\graphql-schemas\graphql-schema-diff.json";
try {
    $debugSlackToken  = Get-Content -Path "$PSScriptRoot/debug-slacktoken.txt"    
}
catch {
    write-host "Create a file named debug-slacktoken.txt that contains the slack token to debug the slack integration" -foregroundcolor red
    $debugSlackToken  = " "
}

$env:INPUT_SLACKTOKEN   = $debugSlackToken;
$env:INPUT_SLACKCHANNEL = "#testslackintegrations";
$env:INPUT_SLACKBOOTNAME   = "GraphqlDiff test";

Import-Module .\GraphqlSchemaComperator\ps_modules\VstsTaskSdk
Invoke-VstsTaskScript -ScriptBlock { . .\GraphqlSchemaComperator\task.ps1 } 
