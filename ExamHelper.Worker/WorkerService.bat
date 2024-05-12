sc.exe create WithSeriLog binpath= "C:\LogsWorkers\win-x64\WorkerService1.exe"
sc.exe start WithSeriLog
sc.exe stop [name]
sc.exe delete [name]