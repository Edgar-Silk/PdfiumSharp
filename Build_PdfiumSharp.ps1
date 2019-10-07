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
$Project_Name = 'PdfiumSharp'

buildVS -path "C:/projects/pdfiumsharp/PdfiumSharp.sln"

