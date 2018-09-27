Param(
  $baseuri = "http://127.0.0.1:9200",
  $deleteIndex = "no",
  $filter = ""
)

Write-Host ""
Write-Host "---Parameters:---------------------"
Write-Host "ElasticSearch Url: $baseuri"
Write-Host "Delete Index: $deleteIndex"
Write-Host "Filter: $filter"
Write-Host "-----------------------------------"
Write-Host ""

# cf: http://serverfault.com/questions/592605/strange-behavior-from-invoke-webrequest-in-scheduled-task
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Ssl3;

if ($deleteIndex -eq "yes") {
  foreach ($dir in Get-ChildItem -Directory) {
    Write-Host "---Deletions:----------------------"
    Write-Host "Deleting index '$dir' ..."
    # Invoke-WebRequest requires Internet Explorer by default, prevent failing with
    # 'The response content cannot be parsed because the Internet Explorer engine is not available, or ...'
    # using '-UseBasicParsing'
    # cf.: http://stackoverflow.com/a/38054505/2592915
    $response = Invoke-WebRequest -UseBasicParsing -Uri "$baseuri/$dir" -ContentType "application/json; charset=utf-8" -Method DELETE
    Write-Host "Status Code: " $response.StatusCode
    Write-Host "-----------------------------------"
    Write-Host ""
  }
}

function Upload-Content() {

  Write-Host "---Uploads:--------------------------"
  $allowedCodes = @(400)
  foreach ($item in Get-ChildItem -File -Recurse -Include *.json | Sort-Object FullName) {
    $fileWOExt = [System.IO.Path]::GetFileNameWithoutExtension($item)
    if ($filter -ne "" -and -not $fileWOExt.StartsWith($filter) -and -not $fileWOExt.StartsWith("_")) { continue }

    $cutPos = 0
    $relative = (Resolve-Path $item -Relative)
    # We need a special treatment for relative path investigation on folder which start with "."
    if ($relative.StartsWith(".\")) {
      $cutPos = 2
    }
    $relative = (Resolve-Path $item -Relative).SubString($cutPos)
    $indexPostfix = $relative.SubString(0, $relative.length - 5).Replace("\", "/").Replace("_", "")

    $content = Get-Content -Path $item -Encoding UTF8
    $uri = "$baseuri/$indexPostfix"

    Write-Host "Uploading '$relative' ..."
    Write-Host "Endpoint '$uri'"

    try {
      # Invoke-WebRequest requires Internet Explorer by default, prevent failing with
      # 'The response content cannot be parsed because the Internet Explorer engine is not available, or ...'
      # using '-UseBasicParsing'
      # cf.: http://stackoverflow.com/a/38054505/2592915
      $header = New-Object 'system.collections.generic.dictionary[string,string]'
      if ($PSVersionTable.PSVersion.Major -ge 4) {
        $header.Add("expect", "") # Using PowerShell >=4, we can use the headers
      }
      $response = Invoke-WebRequest -UseBasicParsing -Uri $uri -ContentType "application/json; charset=utf-8" -Method PUT -Body $content -Headers $header
      Write-Host "Status Code: " $response.StatusCode
    }
    catch [System.Net.WebException] {
      $response = $_.Exception.Response
      if ($allowedCodes -contains $response.StatusCode) {Write-Host -BackgroundColor:Black -ForegroundColor:Yellow $_.Exception.Message }
      else {throw}
    }
    finally {
      Write-Host "-----------------------------------"
    }
  }
}

try {
  Upload-Content
  exit 0
}
catch [Exception] {
  Write-Host -BackgroundColor:Black -ForegroundColor:Red $_.Exception.Message
  exit 1
}
finally{
  Write-Host ""
}
