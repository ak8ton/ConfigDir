chcp 65001
ROBOCOPY /E  "%TestDeploymentDir%\AllConfigurations" "%TestDeploymentDir%\Config" "Base*.xml" "Stand_B*.xml"
echo ok