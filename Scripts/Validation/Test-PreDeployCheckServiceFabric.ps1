<#
.SYNOPSIS 
Test if Arcade Service Fabric cluster exists

#>

$connected = Test-ServiceFabricClusterConnection
if(-not $connected)
{
    Write-Error "Could not connect to Service Fabric Cluster."
    exit 1
}