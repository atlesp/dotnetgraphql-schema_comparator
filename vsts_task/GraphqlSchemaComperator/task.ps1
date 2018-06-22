
[CmdletBinding()]
param()

Import-Module $PSScriptRoot\PSSlack -WarningAction Ignore
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
                $changePretext = "Safe change"
                $color = '#00FF00'
                if($change.IsBreaking) {
                    $changePretext = "Breaking changes"
                    $color = '#FF0000'
                    
                } elseif ($change.IsDangerous) {
                    $changePretext = "Dangerous changes"
                    $color = '#FFa500'
                }
                Write-Host "Adding attachment $changePretext"
                $attachment = New-SlackMessageAttachment -Color $color `
                    -Title "changePretext $($change.name): $($change.path)" `
                    -Text "$($change.message)" `
                    -Pretext  "" `
                    -Fallback "$($change.message)" `
                    -ExistingAttachment $attachment `
                    -TitleLink $buildurl 
                
                
            }

            $mainMessage = "Build $buildNumber for GraphQL API '$apiName' has some changes."
            if($result.IsBreaking){
                    $mainMessage = "At least one of the GraphQL API '$apiName' changes in build $buildNumber are breaking previous version."
            } elseif($result.IsDangerous) {
                $mainMessage = "At least one of the GraphQL API '$apiName' changes in build $buildNumber are dangerous compared to previous version."
            }
            
    
            try{
                $ignore = $attachment |
                    New-SlackMessage -Channel "$slackChannel" -Text "$mainMessage" -IconUrl "https://www.graphqlbin.com/static/media/logo.57ee3b60.png" -Username "$bootname" |
                    Send-SlackMessage -Token "$slackToken" -Verbose
                        
                Write-Host "Slack integration finished"
            }catch {
                $attachment = $null
                $message = "FAILED TO SEND DETAILS OF CHANGES TO SLACK"
                Write-Host "$message"

                $ignore = New-SlackMessageAttachment -Color "#FF0000" `
                                -Title "Error" `
                                -Text "$message" `
                                -Pretext  "" `
                                -Fallback "$message" `
                                -ExistingAttachment $attachment `
                                -TitleLink $buildurl |
                         New-SlackMessage -Channel "$slackChannel" -Text "$mainMessage" -IconUrl "https://www.graphqlbin.com/static/media/logo.57ee3b60.png" -Username "$bootname" |
                         Send-SlackMessage -Token "$slackToken"  -Verbose

            }
        }else {
            Write-Host "Slack integration not configured"
        }

        if($result.IsBreaking -and $failBuildIfBroken) {
            Write-Error "Build failed since the API schemas has breaking changes and task is configured to fail when that happens."
        }
    }


} finally {
    Trace-VstsLeavingInvocation $MyInvocation
}
 