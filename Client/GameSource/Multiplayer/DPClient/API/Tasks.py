import sys
import time
import socket

from GameSource.Multiplayer.DPClient.Manager.Sending import InitializeTask, FeedTask, UpdateTask, QueryEntry
from GameSource.Multiplayer.DPClient.API.Extension import APIHandler, SleepTimer

def CreateTask(connection: socket.socket, tag: str, module_name: str)->None:
    """ Initializes a new Task on the Processor

    Tag : str | Task Tag

    Arguments : dict[str, any] | {"name": value[any]}

    Returns:
        None
    """
    if(not connection): 
        return

    while(not APIHandler.Sendable()):
        time.sleep(SleepTimer)

    initializer = InitializeTask(tag, module_name)
    connection.send(initializer.SerializeJson())
    
    APIHandler.LastCall = time.time()

def FeedTaskArguments(connection: socket.socket, tag: str, arguments: dict)->None:
    """ Feeds Arguments to a Running Task

    Tag : str | Task Tag

    Arguments : dict[str, any] | {"name": value[any]}

    Returns:
        None
    """
    if(not connection):
        return

    while(not APIHandler.Sendable()):
        time.sleep(SleepTimer)
    
    feeder = FeedTask(tag, arguments)
    connection.send(feeder.SerializeJson())
    
    APIHandler.LastCall = time.time()


def UpdateTaskRuntime(connection: socket.socket, tag: str, **options: dict[str, bool])->None:
    """ Updates Task Runtime

    Tag : str | Task Tag

    Options : dict[str, bool] | {"name": value[any]}

    Returns:
        None
    """
    if(not connection):
        return
    
    while(not APIHandler.Sendable()):
        time.sleep(SleepTimer)

    updater = UpdateTask(tag, **options)
   
    connection.send(updater.SerializeJson())
    
    APIHandler.LastCall = time.time()

def QueryVar(connection: socket.socket, tag: str, var: str)->None:
    """ Queries a Task Variable

    Tag : str | Task Tag
    Var : str | Variable Name

    Returns:
        None
    """
    if(not connection):
        return

    while(not APIHandler.Sendable()):
        time.sleep(SleepTimer)

    query = QueryEntry(tag, var)
    
    connection.send(query.SerializeJson())

    APIHandler.LastCall = time.time()