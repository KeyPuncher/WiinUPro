$projDir = $args[0]
$revisionNum = [int]$(git rev-list --count HEAD) + 191 # 190 is the number of commits in SVN when migrating to Git, then +1 for this commit
$assemblyInfo = $projDir + "Properties\AssemblyInfo.cs"
echo "Updating $assemblyInfo to revision number $revisionNum"
$content = (Get-Content -Path "$assemblyInfo") -replace '(\d+\.\d+\.\d+)\.\d+', "`$1.$revisionNum"
Set-Content -Path "$assemblyInfo" -Value $content