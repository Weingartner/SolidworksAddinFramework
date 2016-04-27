param(
[string]$targetfileName="a.dll"  
) 

#############################
# Setup tools
#############################

# .Net Framework version
$fmwk="v4.0.30319"
# GAC Assembly registerer
$regasm = "$env:windir\Microsoft.NET\Framework64\$fmwk\regasm"
$command = "$regasm /verbose /codebase $targetFileName"
echo "iex $command"
iex $command
    
