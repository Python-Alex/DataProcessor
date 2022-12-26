import enum
import socket

from DPClient.Manager.Sending import QueryEntry

def GetTaskResults(connection: socket.socket, tag: str)->None:
    outgoing = QueryEntry(tag, var)


def GetTaskStatus(connection: socket.socket, tag: str)->None:
    outgoing = QueryEntry(tag)


def GetAllocatedTasks(connection: socket.socket, tag: str)->None:
    outgoing = QueryEntry(tag)
