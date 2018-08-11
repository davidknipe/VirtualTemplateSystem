md Package\lib\net40\
md Package\content\Views\VirtualTemplates\
md Package\tools\

del /Q Package\lib\net40\*.*
del /Q Package\content\Views\VirtualTemplates\*.*
copy ..\VirtualTemplates.UI\bin\Release\VirtualTemplates.*.dll Package\lib\net40 
xcopy ..\VirtualTemplates.UI\Views\VirtualTemplates\*.cshtml Package\content\Views\VirtualTemplates /S /Y
"..\Solution File\.nuget\nuget.exe" pack package\VirtualTemplateSystem.nuspec
xcopy /Y *.nupkg c:\project\nuget.local\*.*
