@echo off
echo Removing .idea, obj, and bin directories...

for /d /r %%D in (.idea obj bin) do (
    if exist "%%D" (
        echo Deleting: %%D
        rmdir /s /q "%%D"
    )
)

echo Done.
pause
