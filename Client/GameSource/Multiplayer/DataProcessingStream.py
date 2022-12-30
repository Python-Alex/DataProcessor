from GameSource.Multiplayer import DPClient


ProcessStream = DPClient.InitializeConnection()
DPClient.CreateTask(ProcessStream.Socket, "IRC", "ImageRatioCompare")

DPClient.FeedTaskArguments(ProcessStream.Socket, "IRC", {"A": [[0, 0, 0, 0], [0, 0, 0, 0]], 
                                               "B": [[0, 1, 1, 1, 0], [0, 0, 0, 0, 0]]}
)
DPClient.UpdateTaskRuntime(ProcessStream.Socket, "IRC", **{"PauseTask": True, "StopTask": False, "VerboseOutput": True, "ResumeTask": False})
DPClient.GetAllocatedTasks(ProcessStream.Socket)

