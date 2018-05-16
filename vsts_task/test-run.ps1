Clear-Host

$env:BUILD_BUILDURI = "mybuild uri"
$env:BUILD_BUILDID  = "999"
$env:BUILD_BUILDNUMBER  = "10.10.0"
$env:SYSTEM_TEAMFOUNDATIONSERVERURI = "htps://fdfdsf.com/"
$env:SYSTEM_CULTURE = "EN"


$env:INPUT_GRAPHQLAPINAME   = "MyApi";
$env:INPUT_FAILBUILDIFAPIBROKEN   = $false;

$env:INPUT_OLDSCHEMA    = "C:\temp\graphql-schemas\graphql-schema-diagnostics.txdt";
$env:INPUT_NEWSCHEMA    = "C:\temp\graphql-schemas\graphql-schema-diagnostics-new.txt";
$env:INPUT_SCHEMADIFF   = "C:\temp\graphql-schemas\graphql-schema-diff.json";

$env:INPUT_SLACKTOKEN   = "";
$env:INPUT_SLACKCHANNEL = "#testslackintegrations";
$env:INPUT_SLACKBOOTNAME   = "GraphqlDiff test";

Import-Module .\GraphqlSchemaComperator\ps_modules\VstsTaskSdk
Invoke-VstsTaskScript -ScriptBlock { . .\GraphqlSchemaComperator\task.ps1 } 
