chcp 65001
ROBOCOPY /E  "%TestDeploymentDir%\AllConfigurations" "%TestDeploymentDir%\Config" "Base*.xml" "Stand_A*.xml"
echo ok