# PDFium.DLLをビルドするためのスクリプト -- depot_toolsをインクルードする
# パラメーター
param (
    # オプション: x86 | x64
    [string]$Arch = 'x64',
    # 予約パラメーター
    [string]$Wrapper_Branch = ' '
)

# ビルドディレクトリー
$BuildDir = (Get-Location).path

# コンフィグ
Write-Host "Architecture: " $Arch
Write-Host "Wrapper branch: " $Pdfium_Branch
Write-Host "Directory to Build: " $BuildDir

# 環境変数を設定する
$env:Path = "$BuildDir/depot_tools;$env:Path"
$env:DEPOT_TOOLS_WIN_TOOLCHAIN = "0"
$env:DEPOT_TOOLS_UPDATE = "0"

# ディレクトリーを設定する
$WrapperDir = $BuildDir+'/Wrapper'

# ビルドテンポラリーパスを設定する
if ([System.IO.Directory]::Exists($WrapperDir)) {
    Set-Location $WrapperDir
}
else {
    New-Item -Path $WrapperDir -ItemType Directory
    Set-Location $WrapperDir
}

# Visual Studio MSI-Builder - コンパイラーを設定する
Write-Host "Locate VS 2017 MSBuilder.exe"
function buildVS {
    param (
        [parameter(Mandatory=$true)]
        [String] $path,

        [parameter(Mandatory=$false)]
        [bool] $clean = $true
    )
    process {
        $msBuildExe = Resolve-Path "${env:ProgramFiles(x86)}/Microsoft Visual Studio/2017/*/MSBuild/*/bin/msbuild.exe" -ErrorAction SilentlyContinue

        if ($clean) {
            Write-Host "Cleaning $($path)" -foregroundcolor green
            & "$($msBuildExe)" "$($path)" /t:Clean /m 
        }

        Write-Host "Building $($path)" -foregroundcolor green
        & "$($msBuildExe)" "$($path)" /t:Build /m /p:Configuration=Release,Platform=$Arch /v:n
    }
}

# GitHubプロジェクトを取得する
$Project_Name = 'SilkWrapperNET'
Write-Host "Getting Wrapper repository from github"

git clone -q --branch=master 'https://github.com/Test-Silk/SilkWrapperNET'

Set-Location $WrapperDir'/'$Project_Name

buildVS -path ./SilkWrapperNET.sln 

# DLLが存在するかチェックしWrapper/Libへコピーする
Write-Host "Checking for PDFium.DLL library..."
Set-Location $BuildDir'/pdfium'

if ($Arch -eq 'x64') {
    $OUT_DLL_DIR = $BuildDir + '/Lib/x64'
}
elseif ($Arch -eq 'x86') {
    $OUT_DLL_DIR = $BuildDir + '/Lib/x86'
}
else {
    Write-Host "Arch not defined or invalid..."
    Exit
}

# solutionをコピーする
Write-Host "Copy pdfium DLL to Wrapper solution project"

$Lib_Dir = $WrapperDir+"/"+$Project_Name+"/"+$Project_Name+"/lib/"+$Arch

if ([System.IO.Directory]::Exists( $Lib_Dir )) {
    Set-Location $Lib_Dir
}
else {
    New-Item -Path $Lib_Dir -ItemType Directory
    Set-Location $Lib_Dir
}

if (Test-Path -Path $OUT_DLL_DIR'/pdfium.dll') {
    Copy-Item $OUT_DLL_DIR'/pdfium.dll' -Destination $Lib_Dir
}

# NuGetパッケージを作成する
Write-Host "Make NuGet Package..."

Set-Location $WrapperDir"/"$Project_Name"/"$Project_Name
nuget pack SilkWrapperNET.csproj -properties "Configuration=Release;Platform=$Arch"

# ビルドのテンポラリーパスを設定する
$OUT_NUGET_DIR = $BuildDir+'/NuGet/'+$Arch

# 最終のNuGetパッケージを作成する
if ([System.IO.Directory]::Exists($OUT_NUGET_DIR)) {
    Set-Location $OUT_NUGET_DIR
}
else {
    New-Item -Path $OUT_NUGET_DIR -ItemType Directory
    Set-Location $OUT_NUGET_DIR
}

Write-Host 'Copy NuGet files output to: ' $OUT_NUGET_DIR

Copy-Item -Path "$WrapperDir/$Project_Name/$Project_Name/*.*.*.*.nupkg" -Destination $OUT_NUGET_DIR

Set-Location $BuildDir
