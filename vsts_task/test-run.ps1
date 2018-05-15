cls

$env:BUILD_BUILDURI = "mybuild uri"
$env:BUILD_BUILDID = "999"
$env:SYSTEM_TEAMFOUNDATIONSERVERURI = "htps://fdfdsf.com/"


$env:INPUT_OLDSCHEMA =  "C:\temp\graphql-schemas\graphql-schema-diagnostics.txt";
$env:INPUT_NEWSCHEMA  = "C:\temp\graphql-schemas\graphql-schema-diagnostics-new.txt";
$env:INPUT_SCHEMADIFF  ="C:\temp\graphql-schemas\graphql-schema-diff.json";

$env:INPUT_SLACKTOKEN  ="";
$env:INPUT_SLACKCHANNEL  ="testslackintegrations";



Import-Module .\GraphqlSchemaComperator\ps_modules\VstsTaskSdk
Invoke-VstsTaskScript -ScriptBlock { . .\GraphqlSchemaComperator\task.ps1 } 