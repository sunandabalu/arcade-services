[CmdletBinding()]
param(
    [Parameter(Mandatory=$True)] [string]$MaestroEndpoint,
    [Parameter(Mandatory=$True)] [string]$MaestroManifestPath,
    [int]$TimeoutInSecs = 300
)

try {
    Connect-ServiceFabricCluster -ConnectionEndpoint $MaestroEndpoint | Out-Null

    [xml]$Manifest = Get-Content $MaestroManifestPath 
    $Services = $Manifest.ApplicationManifest.DefaultServices.Service.Name | Select-Object $_

    Write-Host "Services found in the ApplicationManifest: "
    $Services | Where-Object { Write-Host "`t" $_ }
	Write-Host

    $RunningServices = Get-ServiceFabricService -ApplicationName fabric:/MaestroApplication 

    Write-Host "Currently running services: "
    $RunningServices | Where-Object { Write-Host "`t" $_.ServiceName }
	Write-Host

    $RunningServices | Where-Object {
        $ServiceUri = $_.ServiceName.AbsoluteUri

        $IndexOfLastSlash = $ServiceUri.LastIndexOf('/')
        $NonQualifiedName = $ServiceUri.Substring($IndexOfLastSlash + 1)
        $ShouldBeRemoved = ! ($NonQualifiedName -in $Services)

        Write-Host "Should '$NonQualifiedName' service be removed? $ShouldBeRemoved"

        if ($ShouldBeRemoved) {
            Write-Host -NoNewline "`t Removing '$NonQualifiedName' service ... "
            Remove-ServiceFabricService -ServiceName $_.ServiceName -Force -ForceRemove -TimeOutSec $TimeoutInSecs | Out-Null
            Write-Host "done."
        }
    }
}
catch {
	Write-Error "Problems while removing service fabric services."
	Write-Error $_
}
