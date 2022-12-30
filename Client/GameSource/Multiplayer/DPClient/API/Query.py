import enum
import time
import socket


from GameSource.Multiplayer.DPClient.Manager.Sending import QueryEntry
from GameSource.Multiplayer.DPClient.API.Extension import APIHandler, SleepTimer

def GetTaskResults(connection: socket.socket, tag: str)->None:
    """ Gets an Allocated Tasks' Results

    Connection : socket.socket | Master Connection Socket Object
    Tag : str | Task Tag

    Arguments : dict[str, any] | {"name": value[any]}

    Returns:
        None
    """
    if(not connection): 
        return

    while(not APIHandler.Sendable()):
        time.sleep(SleepTimer)

    outgoing = QueryEntry(tag, "TaskResults")
    connection.send(outgoing.SerializeJson())
    
    APIHandler.LastCall = time.time()

def GetTaskStatus(connection: socket.socket, tag: str)->None:
    """ Gets an Allocated Tasks' Status Report

    Connection : socket.socket | Master Connection Socket Object
    Tag : str | Task Tag

    Arguments : dict[str, any] | {"name": value[any]}

    Returns:
        None
    """
    if(not connection): 
        return

    while(not APIHandler.Sendable()):
        time.sleep(SleepTimer)

    outgoing = QueryEntry(tag, "TaskStatus")
    connection.send(outgoing.SerializeJson())
    
    APIHandler.LastCall = time.time()

def GetAllocatedTasks(connection: socket.socket)->None:
    """ Gets all Allocated Tasks

    Connection : socket.socket | Master Connection Socket Object
    Tag : str | Task Tag

    Arguments : dict[str, any] | {"name": value[any]}

    Returns:
        None
    """
    if(not connection): 
        return

    while(not APIHandler.Sendable()):
        time.sleep(SleepTimer)

    outgoing = QueryEntry("null", "AllocatedTasks")
    connection.send(outgoing.SerializeJson())
    
    APIHandler.LastCall = time.time()

