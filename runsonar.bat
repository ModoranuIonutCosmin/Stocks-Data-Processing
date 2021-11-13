dotnet sonarscanner begin /k:"stocks123" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="0637419cec872d60fede2ff0e3fcdbadaf6dcce5"
dotnet build
dotnet sonarscanner end /d:sonar.login="0637419cec872d60fede2ff0e3fcdbadaf6dcce5"