
[CmdletBinding()]
param()

Import-Module $PSScriptRoot\PSSlack 
$ErrorActionPreference="Stop"

# For more information on the VSTS Task SDK:
# https://github.com/Microsoft/vsts-task-lib
Trace-VstsEnteringInvocation $MyInvocation
try {

    #Get-VstsTaskVariableInfo | Out-Host 
    
    $buildNumber = Get-VstsTaskVariable -name "build.buildNumber" 

    $tfsCollectionUri = Get-VstsTaskVariable -name "System.TeamFoundationCollectionUri"

    $buildUri = Get-VstsTaskVariable -name "build.buildUri"
    $collectionId = Get-VstsTaskVariable -name "System.CollectionId"

    $buildUriEncoded  = [System.Web.HttpUtility]::UrlEncode($buildUri)
    $buildurl = "$($tfsCollectionUri)web/build.aspx?pcguid=$collectionId&builduri=$buildUriEncoded"
    
     
    $apiName = Get-VstsInput -Name graphqlApiName
    $failBuildIfBroken = Get-VstsInput -Name failBuildIfApiBroken -AsBool
    $bootname = Get-VstsInput -Name slackBootName

    $oldSchema = Get-VstsInput -Name oldSchema -Require
    Assert-VstsPath -LiteralPath $oldSchema -PathType Leaf
    $newSchema = Get-VstsInput -Name newSchema -Require
    Assert-VstsPath -LiteralPath $oldSchema -PathType Leaf
    $schemaDiff = Get-VstsInput -Name schemaDiff -Require

    Write-Host "Going to compare the new schema '$newSchema' vs the old schema '$oldSchema'"

    $comperator = "$PSScriptRoot/comperator/schema_comparator_cli.exe" 
    & $comperator -o "$oldSchema" -n "$newSchema" -d "$schemaDiff" -s

    Write-Host "Compared the schemas and the diff is output to '$schemaDiff' "

    if(-not (Test-Path -Path $schemaDiff)) {
        Write-Error "Failed when comparing the schemas";
    }

    $result = Get-Content $schemaDiff | ConvertFrom-Json 
        
    if($result.IsIdentical) {
        Write-Host "The schemas were identical"
    } else {
        Write-Host "The schemas were not identical. The changes are breaking: $($result.IsBreaking) or dangerous: $($result.IsDangerous)"
    
        $slackToken = (Get-VstsInput -Name slackToken)
        if($slackToken -and $slackToken -ne " " ) {
            Write-Host "Slack integration"
            
            $slackChannel = Get-VstsInput -Name slackChannel -Require

            $attachment = $null
            foreach($change in $result.changes){
                if($change.IsBreaking) {
                    $attachment = New-SlackMessageAttachment -Color '#FF0000' `
                        -Title "Breaking changes $($change.name): $($change.path)" `
                        -Text "$($change.message)" `
                        -Pretext  "" `
                        -Fallback "$($change.message)" `
                        -ExistingAttachment $attachment `
                        -TitleLink $buildurl
                    
                } elseif ($change.IsDangerous) {
                    $attachment = New-SlackMessageAttachment -Color '#FFa500' `
                                -Title "Dangerous change $($change.name) : $($change.path)" `
                                -Text "$($change.message)" `
                                -Pretext "" `
                                -Fallback "$($change.message)" `
                                -ExistingAttachment $attachment`
                                -TitleLink $buildurl
                    
                } else {
                    $attachment = New-SlackMessageAttachment -Color '#00FF00' `
                        -Title "Safe change $($change.name): $($change.path)" `
                        -Text "$($change.message)" `
                        -Pretext  "" `
                        -Fallback "$($change.message)" `
                        -ExistingAttachment $attachment `
                        -TitleLink $buildurl 
                    
                }
            }

            $mainMessage = "Build $buildNumber for GraphQL API '$apiName' has some changes."
            if($result.IsBreaking){
                    $mainMessage = "At least one of the GraphQL API '$apiName' changes in build $buildNumber are breaking previous version."
            } elseif($result.IsDangerous) {
                $mainMessage = "At least one of the GraphQL API '$apiName' changes in build $buildNumber are dangerous compared to previous version."
            }
            
    
            $ignore = $attachment |
                New-SlackMessage -Channel "$slackChannel" -Text "$mainMessage" -IconUrl "https://www.graphqlbin.com/static/media/logo.57ee3b60.png" -Username "$bootname" |
                Send-SlackMessage -Token "$slackToken" 
                      
            Write-Host "Slack integration finished"
        }else {
            Write-Host "Slack integration not configured"
        }

        if($result.IsBreaking -and $failBuildIfBroken) {
            Write-Error "Build failed since the API schemas has breaking changes and taks is configured to fail when that happens."
        }
    }


} finally {
    Trace-VstsLeavingInvocation $MyInvocation
}
 