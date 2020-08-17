#====== Script destination
# creating deployment package with msbuild package task and copying
# db-migration scripts to package directory $packageDstDir

param(  
        [Parameter(Mandatory=$True)]
        [String]
        $Version
     )

function WriteVersionToServiceManifest($serviceManifestFile, $newVersion) {
    [xml]$xml = (Get-Content $serviceManifestFile)
    $xml.ServiceManifest.Version = $newVersion
    $xml.ServiceManifest.CodePackage.Version = $newVersion
    $xml.ServiceManifest.ConfigPackage.Version = $newVersion
    $xml.Save($serviceManifestFile)
}

function WriteVersionToApplicationManifest($applicationManifestFile, $newVersion) {
    [xml]$xml = (Get-Content $applicationManifestFile)
    $xml.ApplicationManifest.ApplicationTypeVersion =$Version
    
    WriteVersionToServiceManifestImport -xml $xml -serviceManifestName "CalendarIntegrationWebPkg" -newVersion $newVersion

    $xml.Save($applicationManifestFile)
}

function WriteVersionToServiceManifestImport($xml, $serviceManifestName, $newVersion) {
    $node = $xml.ApplicationManifest.ServiceManifestImport | where {$_.ServiceManifestRef.ServiceManifestName -eq $serviceManifestName}
    $node.ServiceManifestRef.ServiceManifestVersion = $newVersion
}

$baseDir = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent

$applicationManifestFile = "$baseDir/sf-package/ApplicationManifest.xml"

$CalendarIntegrationWebServiceManifestFile = "$baseDir/sf-package/CalendarIntegrationWebPkg/ServiceManifest.xml"

#start here
WriteVersionToApplicationManifest -applicationManifestFile $applicationManifestFile -newVersion $Version

WriteVersionToServiceManifest -serviceManifestFile $CalendarIntegrationWebServiceManifestFile -newVersion $Version