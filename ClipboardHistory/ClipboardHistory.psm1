
$path = (Split-Path $MyInvocation.MyCommand.Path -Parent)

$path = Join-Path $path "csharp_dll"

$PSversion = $PSversiontable.PSVersion

$src = ""

# windows only
if ($PSVersion.Major -eq 5 -and $PSVersion.Minor -ge 1) {
    $path = Join-Path $path "netstandard2.0"
} else {
    # not supported powershell
    # throw Error
}

$src = Join-Path $path "OtogawaKatsutoshi.Powershell.Commands.dll"
# load csharp library.
Add-Type -Path $src

function Get-ClipboardHistory {

    # 使える組み合わせ。
    # first, last
    # index
    # skipfast
    # skiplast
    param (
        [Parameter(Mandatory=$true)]
        [Int32] $Index
    )


    # [OtogawaKatsutoshi.Powershell.Commands.GetClipboardHistoryCommand]::GetClipboardHistoryContentAsText(0, $true)
    if ($PSversiontable.PSVersion.Major -eq 5 -and $PSversiontable.PSVersion.Major -ge 1) {
        [lib.Math]::add_x86_64_pc_windows_msvc($x, $y)
    } elseif ( $PSversiontable.OS -match "Windows" -and $global:os_type -eq [SystemArch]::X64) {
        [lib.Math]::add_x86_64_pc_windows_msvc($x, $y)
    } elseif ( $PSversiontable.OS -match "Windows" -and $global:os_type -eq [SystemArch]::Aarch64) {
        [lib.Math]::add_aarch64_pc_windows_msvc($x, $y)
    } elseif ( $PSversiontable.OS -match "Darwin" -and $global:os_type -eq [SystemArch]::X64) {
        [lib.Math]::add_x86_64_pc_apple_darwin($x, $y)
    } elseif ( $PSversiontable.OS -match "Darwin" -and $global:os_type -eq [SystemArch]::Aarch64) {
        [lib.Math]::add_aarch64_pc_apple_darwin($x, $y)
    } elseif ( $PSversiontable.OS -match "Linux" -and $global:os_type -eq [SystemArch]::X64) {
        [lib.Math]::add_x86_64_unknown_linux_gnu($x, $y)
    } elseif ( $PSversiontable.OS -match "Linux" -and $global:os_type -eq [SystemArch]::Aarch64) {
        [lib.Math]::add_aarch64_unknown_linux_gnu($x, $y)
    }
}

function Get-ClipboardHistoryPolicy {
   $policy = (Get-ItemProperty -Path HKCU:\Software\Microsoft\Clipboard -Name EnableClipboardHistory).EnableClipboardHistory

   if ($policy -eq 1) {
    Write-Output "Enable"
   } elseif ($policy -eq 0) {
    Write-Output "Disable"
   }
}

function Enable-ClipboardHistoryPolicy {
   Set-ItemProperty -Path HKCU:\Software\Microsoft\Clipboard -Name EnableClipboardHistory -Value 1 -Type DWord
}

function Disable-ClipboardHistoryPolicy {
   Set-ItemProperty -Path HKCU:\Software\Microsoft\Clipboard -Name EnableClipboardHistory -Value 0 -Type DWord
}
