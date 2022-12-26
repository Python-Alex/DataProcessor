

import json
import enum

class Packet(object):

    def SerializeJson(self) -> bytes:
        """Serializes class into json bytes sendable over the socket

        Returns:
            bytes: serialized bytes
        """
        return json.dumps(self.__dict__).encode()


class EventType(enum.Enum):

    INITIALIZE = 0
    FEED = 1
    UPDATE = 2
    QUERY = 3

class InitializeTask(Packet):

    Event : int
    Tag : str
    ModuleName : str

    def __init__(self, tag: str, module_name: str)->None:
        super().__init__()

        self.Event = EventType.INITIALIZE.value
        self.Tag = tag
        self.ModuleName = module_name

class FeedTask(Packet):

    Event : int
    Tag : str
    Arguments : dict[str, any]

    def __init__(self, tag: str, arguments : dict = {})->None:
        super().__init__()

        self.Event = EventType.FEED.value
        self.Tag = tag

        self.Arguments = arguments

    def AddArgument(self, name: str, value: any)->None:
        self.Arguments[name] = value
    
    def RemoveArgument(self, name: str)->None:
        del self.Arguments[name]

class UpdateTask(Packet):

    Event : int
    Tag : int
    VerboseOutput : bool
    StopTask : bool
    PauseTask : bool
    ResumeTask : bool

    def __init__(self, tag: str, **options: dict)->None:
        super().__init__()

        self.Event = EventType.UPDATE.value
        self.Tag = tag

        self.VerboseOutput = options.setdefault("VerboseOutput", False)
        self.StopTask = options.setdefault("StopTask", False)
        self.PauseTask = options.setdefault("PauseTask", False)
        self.ResumeTask = options.setdefault("ResumeTask", False)

    def ToggleVerbosity(self)->None:
        """ Toggles Verbosity (SERVER-SIDE ONLY)""" 
        self.VerboseOutput = True if(not self.VerboseOutput) else False
        
    def SetStopTask(self)->None:
        """ Stop Task from Running, Clears task from Server Registry """
        self.StopTask = True if(not self.StopTask) else False

    def SetPauseTask(self)->None:
        """ Pauses Task until Resumed """
        self.PauseTask = True if(not self.PauseTask) else False

    def SetResumeTask(self)->None:
        """ Resumes Paused Task """
        self.ResumeTask = True if(not self.ResumeTask) else False

class QueryEntry(Packet):

    Event : int
    Tag : int
    Var : str

    def __init__(self, tag: str, var: str)->None:
        super().__init__()

        self.Event = EventType.QUERY.value
        self.Tag = tag
        self.Var = var

    def UpdateVar(self, var: str)->None:
        """ Set Var Name """
        self.Var = var