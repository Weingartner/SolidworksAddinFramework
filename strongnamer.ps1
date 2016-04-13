param(
[string]$inPath=".\", # the directory containing the build
[string]$targetfileName="a.dll"  # The name of the dll to be registered ( relative to $outpath )
) 

[char[]]$trimChars = '\\' 
function FixTerminatingSlash ($root) { 
    return $root.TrimEnd($trimChars)    
}

#################################################################
# Key generation tool. We generate a new key every time. Why not?
# All we care about is that each addin has a different identity. We
# are not really caring exactly what that identity is.
#################################################################
$keyFile = "$outPath\StrongName.snk"
$netfxpath = "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools"
$sn = "$netfxpath\sn.exe"
&sn -k 2048 $keyFile

#############################
# Setup tools
#############################

# .Net Framework version
$fmwk="v4.0.30319"
# GAC Assembly registerer
$regasm = "$env:windir\Microsoft.NET\Framework64\$fmwk\regasm"
# Strong name assembly signing tool stored in nuget package 
$strongNameSigner = "$(gci $PSScriptRoot\packages\Brutal.Dev.StrongNameSigner.*)\tools\StrongNameSigner.Console.exe"

echo $strongNameSigner


#############################
# Signing process
# 
# Copies the build to a new directory, signs all dlls and then registers 
# the main dll in the GAC
#############################
$outPath = "$(FixTerminatingSlash(Get-Item $inPath).FullName).Signed\"

echo "InPath is $inPath"
echo "OutPath is $outPath"

rmdir -Force -Recurse $outPath
copy -Recurse $inPath $outPath
&$strongNameSigner -in $inPath -out $outPath -k $keyFile


$targetPath = "$outPath$targetFileName"
cd $outPath
$command = "$regasm /verbose /codebase $targetFileName"
$command
    
