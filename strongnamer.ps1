param(
[string]$inPath=".\", # the directory containing the build
[string]$targetfileName="a.dll"  # The name of the dll to be registered ( relative to $outpath )
) 

[char[]]$trimChars = '\\' 
function FixTerminatingSlash ($root) { 
    return $root.TrimEnd($trimChars)    
}

#############################
# Setup tools
#############################

# .Net Framework version
$fmwk="v4.0.30319"
# GAC Assembly registerer
$regasm = "$env:windir\Microsoft.NET\Framework64\$fmwk\regasm"
# Strong name assembly signing tool stored in nuget package 
$strongNameSigner = "$(gci $PSScriptRoot\packages\Brutal.Dev.StrongNameSigner.*)\tools\StrongNameSigner.Console.exe"
# Get the keyfile
$keyFile = "$PSScriptRoot\StrongNameKey.snk"

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
    
