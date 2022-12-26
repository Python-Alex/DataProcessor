import time
import DPClient

DPS = DPClient.InitializeConnection()
DPClient.CreateTask(DPS.Socket, "IRC", "ImageRatioCompare")

DPClient.FeedTaskArguments(DPS.Socket, "IRC", {"A": [[0, 0, 0, 0], [0, 0, 0, 0]], 
                                               "B": [[0, 1, 1, 1, 0], [0, 0, 0, 0, 0]]}
)
DPClient.UpdateTaskRuntime(DPS.Socket, "IRC", PauseTask=True, StopTask=False, VerboseOutput=True, ResumeTask=False)

print(DPClient.APIHandler.TimeSinceLastSend())