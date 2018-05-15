
[CmdletBinding()]
param()

Import-Module $PSScriptRoot\PSSlack 

# For more information on the VSTS Task SDK:
# https://github.com/Microsoft/vsts-task-lib
Trace-VstsEnteringInvocation $MyInvocation
try {

    #Get-VstsTaskVariableInfo | Out-Host 
    # https://domoslabs.visualstudio.com/Domos/_build/index?buildId=1808&_a=summary
    $buildId = Get-VstsTaskVariable -name "build.buildId" 
    $tfsServerUri = Get-VstsTaskVariable -name "system.TeamFoundationServerUri"
    
    $builduri = "$($tfsServerUri)Domos/_build/index?buildId=$buildId&_a=summary"
    #build.buildNumber 
    #system.teamFoundationCollectionUri 
    #build.buildUri    vstfs:///Build/Build/1808 
    
    
    $oldSchema = Get-VstsInput -Name oldSchema -Require
    Assert-VstsPath -LiteralPath $oldSchema -PathType Leaf

    $newSchema = Get-VstsInput -Name newSchema -Require
    $schemaDiff = Get-VstsInput -Name schemaDiff -Require

    Write-Host "Going to compare the new schema '$newSchema' vs the old schema '$oldSchema'"

    $comperator = "$PSScriptRoot/comperator/schema_comparator_cli.exe" 
    & $comperator -o "$oldSchema" -n "$newSchema" -d "$schemaDiff" -s

    Write-Host "Compared the schemas and the diff is output to '$schemaDiff' "

    $result = Get-Content $schemaDiff | ConvertFrom-Json 
        
    if($result.IsIdentical) {
        Write-Host "The schemas were identical"
    } else {
        Write-Host "The schemas were not identical. The changes are breaking: $($result.IsBreaking) or dangerous: $($result.IsDangerous)"
    
        $slackToken = Get-VstsInput -Name slackToken
        if($slackToken ) {
            Write-Host "Slack integration"
            
            $slackChannel = Get-VstsInput -Name slackChannel -Require
            $bootname = "GraphQLApi"
            #-TitleLink https://www.youtube.com/watch?v=TmpRs7xN06Q `
            $attachment = $null
            foreach($change in $result.changes){
                if($change.IsBreaking) {
                    $attachment = New-SlackMessageAttachment -Color '#FF0000' `
                        -Title "Breaking changes $($change.name): $($change.path)" `
                        -Text "$($change.message)" `
                        -Pretext  "" `
                        -Fallback "$($change.message)" `
                        -ExistingAttachment $attachment
                    
                } elseif ($change.IsDangerous) {
                    $attachment = New-SlackMessageAttachment -Color '#FFa500' `
                                -Title "Dangerous change $($change.name) : $($change.path)" `
                                -Text "$($change.message)" `
                                -Pretext "" `
                                -Fallback "$($change.message)" `
                                -ExistingAttachment $attachment
                    
                } else {
                    $attachment = New-SlackMessageAttachment -Color '#00FF00' `
                        -Title "Safe change $($change.name): $($change.path)" `
                        -Text "$($change.message)" `
                        -Pretext  "" `
                        -Fallback "$($change.message)" `
                        -ExistingAttachment $attachment
                    
                }
            }

            $mainMessage = "GraphQL API has some changes."
            if($result.IsBreaking){
                    $mainMessage = "At least one of the GraphQL API changes are breaking previous version."
            } elseif($result.IsDangerous) {
                $mainMessage = "At least one of the GraphQL API changes are dangerouscompared to previous version."
            }
            
    
            $ignore = $attachment |
                New-SlackMessage -Channel "$slackChannel" -Text "$mainMessage" -IconEmoji :bomb: -Username "$bootname" |
                Send-SlackMessage -Token "$slackToken"
                      
            Write-Host "Slack integration finished"
        }else {
            Write-Host "Slack integration not configured"
        }
    }


} finally {
    Trace-VstsLeavingInvocation $MyInvocation
}
 