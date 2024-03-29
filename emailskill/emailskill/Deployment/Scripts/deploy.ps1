#Requires -Version 6

Param(
    [string] $name,
	[string] $resourceGroup,
    [string] $location,
	[string] $appId,
    [string] $appPassword,
    [string] $luisAuthoringKey,
	[string] $luisAuthoringRegion,
    [string] $parametersFile,
	[string] $languages = "en-us",
	[string] $projDir = $(Get-Location),
	[string] $logFile = $(Join-Path $PSScriptRoot .. "deploy_log.txt")
)

# Reset log file
if (Test-Path $logFile) {
	Clear-Content $logFile -Force | Out-Null
}
else {
	New-Item -Path $logFile | Out-Null
}

if (-not (Test-Path (Join-Path $projDir 'appsettings.json')))
{
	Write-Host "! Could not find an 'appsettings.json' file in the current directory." -ForegroundColor DarkRed
	Write-Host "+ Please re-run this script from your project directory." -ForegroundColor Magenta
	Break
}

# Get mandatory parameters
if (-not $name) {
    $name = "AAA"
}

if (-not $resourceGroup) {
	$resourceGroup = "azurechatbot-s010-20190617"
}

if (-not $location) {
    $location = "Australia East"
}

if (-not $appPassword) {
    $appPassword = "kWA9dBd6DL0PaddWnJqBsqyF4gqK+GfpxeirZ10dN+8="
}

if (-not $luisAuthoringRegion) {
    $luisAuthoringRegion = "westus"
}

if (-not $luisAuthoringKey) {
	Switch ($luisAuthoringRegion) {
		"westus" { 
			$luisAuthoringKey = "cffde7a42bba4235bb98a5bb497a57c6"
			Break
		}
		"westeurope" {
		    $luisAuthoringKey = Read-Host "? LUIS Authoring Key (found at https://eu.luis.ai/user/settings)"
			Break
		}
		"australiaeast" {
			$luisAuthoringKey = Read-Host "? LUIS Authoring Key (found at https://au.luis.ai/user/settings)"
			Break
		}
		default {
			Write-Host "! $($luisAuthoringRegion) is not a valid LUIS authoring region." -ForegroundColor DarkRed
			Break
		}
	}

	if (-not $luisAuthoringKey) {
		Break
	}
}


#if (-not $appId) {
	# Create app registration
#	$app = (az ad app create `
#		--display-name $name `
#		--password $appPassword `
#		--available-to-other-tenants `
#		--reply-urls 'https://token.botframework.com/.auth/web/redirect')

	# Retrieve AppId
#	if ($app) {
		$appId = "06eaa0f5-7b30-4aec-8611-b28d1f775f82"
#	}


	if(-not $appId) {
		Write-Host "! Could not provision Microsoft App Registration automatically. Review the log for more information." -ForegroundColor DarkRed
		Write-Host "! Log: $($logFile)" -ForegroundColor DarkRed
		Write-Host "+ Provision an app manually in the Azure Portal, then try again providing the -appId and -appPassword arguments. See https://aka.ms/vamanualappcreation for more information." -ForegroundColor Magenta
		Break
	}
#}

# Get timestamp
$timestamp = Get-Date -f MMddyyyyHHmmss

# Create resource group
#Write-Host "> Creating resource group ..."
#(az group create --name $resourceGroup --location $location) 2>> $logFile | Out-Null

# Deploy Azure services (deploys LUIS, QnA Maker, Content Moderator, CosmosDB)
if ($parametersFile) {
	Write-Host "> Validating Azure deployment ..."
	$validation = az group deployment validate `
		--resource-group $resourcegroup `
		--template-file "$(Join-Path $PSScriptRoot '..' 'Resources' 'template.json')" `
		--parameters "@$($parametersFile)" `
		--parameters name=$name microsoftAppId=$appId microsoftAppPassword="`"$($appPassword)`""

	if ($validation) {
		$validation >> $logFile
		$validation = $validation | ConvertFrom-Json
	
		if (-not $validation.error) {
			Write-Host "> Deploying Azure services (this could take a while)..." -ForegroundColor Yellow
			$deployment = az group deployment create `
				--name $timestamp `
				--resource-group $resourceGroup `
				--template-file "$(Join-Path $PSScriptRoot '..' 'Resources' 'template.json')" `
				--parameters "@$($parametersFile)" `
				--parameters name=$name microsoftAppId=$appId microsoftAppPassword="`"$($appPassword)`""
		}
		else {
			Write-Host "! Template is not valid with provided parameters. Review the log for more information." -ForegroundColor DarkRed
			Write-Host "! Error: $($validation.error.message)"  -ForegroundColor DarkRed
			Write-Host "! Log: $($logFile)" -ForegroundColor DarkRed
			Write-Host "+ To delete this resource group, run 'az group delete -g $($resourceGroup) --no-wait'" -ForegroundColor Magenta
			Break
		}
	}
}
else {
	Write-Host "> Validating Azure deployment ..."
	$validation = az group deployment validate `
		--resource-group $resourcegroup `
		--template-file "$(Join-Path $PSScriptRoot '..' 'Resources' 'template.json')" `
		--parameters name=$name microsoftAppId=$appId microsoftAppPassword="`"$($appPassword)`""

	if ($validation) {
		$validation >> $logFile
		$validation = $validation | ConvertFrom-Json

		if (-not $validation.error) {
			Write-Host "> Deploying Azure services (this could take a while)..." -ForegroundColor Yellow
			$deployment = az group deployment create `
				--name $timestamp `
				--resource-group $resourceGroup `
				--template-file "$(Join-Path $PSScriptRoot '..' 'Resources' 'template.json')" `
				--parameters name=$name microsoftAppId=$appId microsoftAppPassword="`"$($appPassword)`""
		}
		else {
			Write-Host "! Template is not valid with provided parameters. Review the log for more information." -ForegroundColor DarkRed
			Write-Host "! Error: $($validation.error.message)"  -ForegroundColor DarkRed
			Write-Host "! Log: $($logFile)" -ForegroundColor DarkRed
			Write-Host "+ To delete this resource group, run 'az group delete -g $($resourceGroup) --no-wait'" -ForegroundColor Magenta
			Break
		}
	}
}

# Get deployment outputs
$outputs = (az group deployment show `
	--name $timestamp `
	--resource-group $resourceGroup `
	--query properties.outputs) 2>> $logFile

# If it succeeded then we perform the remainder of the steps
if ($outputs)
{
	# Log and convert to JSON
	$outputs >> $logFile
	$outputs = $outputs | ConvertFrom-Json
	$outputMap = @{}
	$outputs.PSObject.Properties | Foreach-Object { $outputMap[$_.Name] = $_.Value }

	# Update appsettings.json
	Write-Host "> Updating appsettings.json ..."
	if (Test-Path $(Join-Path $projDir appsettings.json)) {
		$settings = Get-Content $(Join-Path $projDir appsettings.json) | ConvertFrom-Json
	}
	else {
		$settings = New-Object PSObject
	}

	$settings | Add-Member -Type NoteProperty -Force -Name 'microsoftAppId' -Value $appId
	$settings | Add-Member -Type NoteProperty -Force -Name 'microsoftAppPassword' -Value $appPassword
	foreach ($key in $outputMap.Keys) { $settings | Add-Member -Type NoteProperty -Force -Name $key -Value $outputMap[$key].value }
	$settings | ConvertTo-Json -depth 100 | Out-File $(Join-Path $projDir appsettings.json)
	
	if ($outputs.qnaMaker.value.key) { $qnaSubscriptionKey = $outputs.qnaMaker.value.key }

	# Delay to let QnA Maker finish setting up
	Start-Sleep -s 30

	# Deploy cognitive models
	Invoke-Expression "& '$(Join-Path $PSScriptRoot 'deploy_cognitive_models.ps1')' -name $($name) -luisAuthoringRegion $($luisAuthoringRegion) -luisAuthoringKey $($luisAuthoringKey) -luisAccountName $($outputs.luis.value.accountName) -luisAccountRegion $($outputs.luis.value.region) -luisSubscriptionKey $($outputs.luis.value.key) -resourceGroup $($resourceGroup) -qnaSubscriptionKey '$($qnaSubscriptionKey)' -outFolder '$($projDir)' -languages '$($languages)'"
	
	# Publish bot
	Invoke-Expression "& '$(Join-Path $PSScriptRoot 'publish.ps1')' -name $($outputs.botWebAppName.value) -resourceGroup $($resourceGroup) -projFolder '$($projDir)'"

	Write-Host "> Done."
}
else
{
	# Check for failed deployments
	$operations = (az group deployment operation list -g $resourceGroup -n $timestamp) 2>> $logFile | Out-Null 
	
	if ($operations) {
		$operations = $operations | ConvertFrom-Json
		$failedOperations = $operations | Where { $_.properties.statusmessage.error -ne $null }
		if ($failedOperations) {
			foreach ($operation in $failedOperations) {
				switch ($operation.properties.statusmessage.error.code) {
					"MissingRegistrationForLocation" {
						Write-Host "! Deployment failed for resource of type $($operation.properties.targetResource.resourceType). This resource is not avaliable in the location provided." -ForegroundColor DarkRed
						Write-Host "+ Update the .\Deployment\Resources\parameters.template.json file with a valid region for this resource and provide the file path in the -parametersFile parameter." -ForegroundColor Magenta
					}
					default {
						Write-Host "! Deployment failed for resource of type $($operation.properties.targetResource.resourceType)."
						Write-Host "! Code: $($operation.properties.statusMessage.error.code)."
						Write-Host "! Message: $($operation.properties.statusMessage.error.message)."
					}
				}
			}
		}
	}
	else {
		Write-Host "! Deployment failed. Please refer to the log file for more information." -ForegroundColor DarkRed
		Write-Host "! Log: $($logFile)" -ForegroundColor DarkRed
	}
	
	Write-Host "+ To delete this resource group, run 'az group delete -g $($resourceGroup) --no-wait'" -ForegroundColor Magenta
	Break
}