::Create Dir link

set LINK_DIR=..\Snaker-Clone

rmdir %LINK_DIR%
mkdir %LINK_DIR%

rmdir %LINK_DIR%\Assets
rmdir %LINK_DIR%\ProjectSettings
rmdir %LINK_DIR%\Library

mklink /J %LINK_DIR%\Assets Assets
mklink /J %LINK_DIR%\ProjectSettings ProjectSettings
mklink /J %LINK_DIR%\Library Library