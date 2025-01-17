@echo off

REM Prompt the user to enter a training name
set /p RUN_ID=Enter training name (default: DefaultRun): 
if "%RUN_ID%"=="" set RUN_ID=DefaultRun

REM Set the path to the results directory
set LOGDIR=D:\Unity Projects\GitHub\Trip-And-Grip\Assets\results

REM Print the path being checked
echo Checking for results directory at: %LOGDIR%

REM Check if TensorBoard is installed
where tensorboard >nul
if %ERRORLEVEL%==1 (
    echo TensorBoard is not installed. Please ensure it is installed and added to the PATH.
    pause
    exit /b
)

REM Check if the results directory exists
if not exist "%LOGDIR%" (
    echo Results directory does not exist.
    echo Checked path: %LOGDIR%
    pause
    exit /b
)

REM Check if TensorBoard is already running
tasklist | findstr tensorboard >nul
if %ERRORLEVEL%==0 (
    echo TensorBoard is already running.
) else (
    start tensorboard --logdir="%LOGDIR%" --reload_interval=5
    echo TensorBoard started successfully with auto-refresh every 5 seconds.
)

REM Open the browser automatically
start http://localhost:6006

REM Start training with the provided name
echo Starting training with name: %RUN_ID%
mlagents-learn trainer_config.yaml --run-id=%RUN_ID%

pause
