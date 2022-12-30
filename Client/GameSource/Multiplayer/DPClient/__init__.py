from GameSource.Multiplayer.DPClient.API.Query import GetAllocatedTasks, GetTaskResults, GetTaskStatus
from GameSource.Multiplayer.DPClient.API.Tasks import CreateTask, FeedTaskArguments, UpdateTaskRuntime, QueryVar
from GameSource.Multiplayer.DPClient.API.Extension import APIHandler
from GameSource.Multiplayer.DPClient.Manager.Receiving import Incoming, QueryResponse, EventType as REType
from GameSource.Multiplayer.DPClient.Manager.Sending import InitializeTask, FeedTask, UpdateTask, QueryEntry, EventType as SEType
from GameSource.Multiplayer.DPClient.Manager.Stream import InitializeConnection